#include "driver.h"

#include <rawaccel.hpp>
#include <rawaccel-io-def.h>
#include <rawaccel-version.h>


#ifdef ALLOC_PRAGMA
#pragma alloc_text (INIT, DriverEntry)
#pragma alloc_text (INIT, RawaccelInit)
#pragma alloc_text (INIT, CreateControlDevice)
#pragma alloc_text (PAGE, EvtDeviceAdd)
#pragma alloc_text (PAGE, EvtIoInternalDeviceControl)
#pragma alloc_text (PAGE, RawaccelControl)
#pragma alloc_text (PAGE, DeviceCleanup)
#pragma alloc_text (PAGE, DeviceSetup)
#pragma alloc_text (PAGE, WriteDelay)
#endif

using milliseconds = double;

struct {
    WDFCOLLECTION device_collection;
    WDFWAITLOCK collection_lock;
    ra::device_config default_dev_cfg;
    unsigned device_data_size;
    unsigned driver_data_size;
    ra::device_settings* device_data;
    ra::driver_settings* driver_data;
    milliseconds tick_interval;
    ra::modifier modifier_data[ra::DRIVER_CAPACITY];
} global = {};

extern "C" PULONG InitSafeBootMode;

bool init_failed() 
{ 
    return global.driver_data_size == 0; 
};

__declspec(guard(ignore))
VOID
RawaccelCallback(
    IN PDEVICE_OBJECT DeviceObject,
    IN PMOUSE_INPUT_DATA InputDataStart,
    IN PMOUSE_INPUT_DATA InputDataEnd,
    IN OUT PULONG InputDataConsumed
)
/*++

Routine Description:

    Called when there are mouse packets to report to the RIT.

Arguments:

    DeviceObject - Context passed during the connect IOCTL

    InputDataStart - First packet to be reported

    InputDataEnd - One past the last packet to be reported.  Total number of
                   packets is equal to InputDataEnd - InputDataStart

    InputDataConsumed - Set to the total number of packets consumed by the RIT
                        (via the function pointer we replaced in the connect
                        IOCTL)

--*/
{
    WDFDEVICE hDevice = WdfWdmDeviceGetWdfDeviceHandle(DeviceObject);
    PDEVICE_EXTENSION devExt = FilterGetData(hDevice);

    auto num_packets = InputDataEnd - InputDataStart;

    if (num_packets > 0 &&
        !(InputDataStart->Flags & MOUSE_MOVE_ABSOLUTE) &&
        devExt->enable) {
        
        milliseconds time;
        if (devExt->keep_time) {
            counter_t now = KeQueryPerformanceCounter(NULL).QuadPart;
            counter_t ticks = now - devExt->counter;
            devExt->counter = now;
            milliseconds raw = ticks * global.tick_interval / num_packets;
            time = ra::clampsd(raw, devExt->clamp.min, devExt->clamp.max);
        }
        else {
            time = devExt->clamp.min;
        }

        auto it = InputDataStart;
        do {
            if (devExt->set_extra_info) {
                union {
                    short input[2];
                    ULONG data;
                } u = { short(it->LastX), short(it->LastY) };

                it->ExtraInformation = u.data;
            }

            if (it->LastX || it->LastY) {
                vec2d input = {
                    static_cast<double>(it->LastX),
                    static_cast<double>(it->LastY)
                };

                devExt->mod_ptr->modify(input, *devExt->drv_ptr, devExt->dpi_factor, time);

                double carried_result_x = input.x + devExt->carry.x;
                double carried_result_y = input.y + devExt->carry.y;

                LONG out_x = static_cast<LONG>(carried_result_x);
                LONG out_y = static_cast<LONG>(carried_result_y);

                double carry_x = carried_result_x - out_x;
                double carry_y = carried_result_y - out_y;

                if (!ra::infnan(carry_x + carry_y)) {
                    devExt->carry.x = carry_x;
                    devExt->carry.y = carry_y;
                    it->LastX = out_x;
                    it->LastY = out_y;
                }
            }
        } while (++it != InputDataEnd);

    }

    (*(PSERVICE_CALLBACK_ROUTINE)devExt->UpperConnectData.ClassService)(
        devExt->UpperConnectData.ClassDeviceObject,
        InputDataStart,
        InputDataEnd,
        InputDataConsumed
        );
}


#pragma warning(push)
#pragma warning(disable:28118) // this callback will run at IRQL=PASSIVE_LEVEL
_Use_decl_annotations_
VOID
RawaccelControl(
    WDFQUEUE         Queue,
    WDFREQUEST       Request,
    size_t           OutputBufferLength,
    size_t           InputBufferLength,
    ULONG            IoControlCode
)
/*++
Routine Description:
    This event is called when the framework receives IRP_MJ_DEVICE_CONTROL
    requests from the system.
Arguments:
    Queue - Handle to the framework queue object that is associated
            with the I/O request.
    Request - Handle to a framework request object.
    OutputBufferLength - length of the request's output buffer,
                        if an output buffer is available.
    InputBufferLength - length of the request's input buffer,
                        if an input buffer is available.
    IoControlCode - the driver-defined or system-defined I/O control code
                    (IOCTL) that is associated with the request.
Return Value:
   VOID
--*/
{
    NTSTATUS status;
    void* buffer;

    size_t bytes_out = 0;

    UNREFERENCED_PARAMETER(Queue);
    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);
    PAGED_CODE();

    DebugPrint(("Ioctl received into filter control object.\n"));

    if (init_failed()) {
        WdfRequestCompleteWithInformation(Request, STATUS_CANCELLED, 0);
        return;
    }

    switch (IoControlCode) {
    case ra::READ:
        status = WdfRequestRetrieveOutputBuffer(
            Request,
            sizeof(ra::io_t),
            &buffer,
            NULL
        );
        if (!NT_SUCCESS(status)) {
            DebugPrint(("RetrieveOutputBuffer failed: 0x%x\n", status));
        }
        else {
            ra::io_t& output = *static_cast<ra::io_t*>(buffer);

            output.default_dev_cfg = global.default_dev_cfg;

            output.device_data_size = global.device_data_size;
            output.driver_data_size = global.driver_data_size;

            RtlCopyMemory(output.device_data, global.device_data, sizeof(output.device_data));
            RtlCopyMemory(output.driver_data, global.driver_data, sizeof(output.driver_data));

            bytes_out = sizeof(ra::io_t);
        }
        break;
    case ra::WRITE:
        status = WdfRequestRetrieveInputBuffer(
            Request,
            sizeof(ra::io_t),
            &buffer,
            NULL
        );
        if (!NT_SUCCESS(status)) {
            DebugPrint(("RetrieveInputBuffer failed: 0x%x\n", status));
        }
        else {
            WriteDelay();

            ra::io_t& input = *static_cast<ra::io_t*>(buffer);

            if (input.driver_data_size == 0) {
                status = STATUS_CANCELLED;
                break;
            }

            WdfWaitLockAcquire(global.collection_lock, NULL);

            global.default_dev_cfg = input.default_dev_cfg;

            global.device_data_size = ra::min(input.device_data_size, ra::DEVICE_CAPACITY);
            global.driver_data_size = ra::min(input.driver_data_size, ra::DRIVER_CAPACITY);

            RtlCopyMemory(global.device_data, input.device_data, sizeof(input.device_data));
            RtlCopyMemory(global.driver_data, input.driver_data, sizeof(input.driver_data));

            for (auto i = 0u; i < global.driver_data_size; i++) {
                global.modifier_data[i] = { global.driver_data[i] };
            }

            auto count = WdfCollectionGetCount(global.device_collection);

            for (auto i = 0u; i < count; i++) {
                DeviceSetup(WdfCollectionGetItem(global.device_collection, i));
            }

            WdfWaitLockRelease(global.collection_lock);
        }

        break;
    case ra::GET_VERSION:
        status = WdfRequestRetrieveOutputBuffer(
            Request,
            sizeof(ra::version_t),
            &buffer,
            NULL
        );
        if (!NT_SUCCESS(status)) {
            DebugPrint(("RetrieveOutputBuffer failed: 0x%x\n", status));
        }
        else {
            *static_cast<ra::version_t*>(buffer) = ra::version;
            bytes_out = sizeof(ra::version_t);
        }
        break;
    default:
        status = STATUS_INVALID_DEVICE_REQUEST;
        break;
    }

    WdfRequestCompleteWithInformation(Request, status, bytes_out);

}
#pragma warning(pop) // enable 28118 again

VOID
RawaccelInit(WDFDRIVER driver)
{
    NTSTATUS status;

    if (*InitSafeBootMode > 0) return;

    status = CreateControlDevice(driver);

    if (!NT_SUCCESS(status)) {
        DebugPrint(("CreateControlDevice failed with status 0x%x\n", status));
        return;
    }

    status = WdfCollectionCreate(
        WDF_NO_OBJECT_ATTRIBUTES,
        &global.device_collection
    );

    if (!NT_SUCCESS(status)) {
        DebugPrint(("WdfCollectionCreate failed with status 0x%x\n", status));
        return;
    }

    status = WdfWaitLockCreate(
        WDF_NO_OBJECT_ATTRIBUTES,
        &global.collection_lock
    );

    if (!NT_SUCCESS(status)) {
        DebugPrint(("WdfWaitLockCreate failed with status 0x%x\n", status));
        return;
    }

    void* paged_p = ExAllocatePoolWithTag(PagedPool, ra::POOL_SIZE, 'g');
    if (paged_p) {
        RtlZeroMemory(paged_p, ra::POOL_SIZE);
        global.device_data = static_cast<ra::device_settings*>(paged_p);
    }
    else {
        DebugPrint(("ExAllocatePoolWithTag (PagedPool) failed"));
        return;
    }

    void* nonpaged_p = ExAllocatePoolWithTag(NonPagedPool, ra::POOL_SIZE, 'g');
    if (nonpaged_p) {
        RtlZeroMemory(nonpaged_p, ra::POOL_SIZE);
        global.driver_data = static_cast<ra::driver_settings*>(nonpaged_p);
        global.driver_data->prof.domain_weights = { 1, 1 };
        global.driver_data->prof.range_weights = { 1, 1 };
        global.driver_data->prof.sensitivity = 1;
        global.driver_data->prof.yx_sens_ratio = 1;
        global.driver_data_size = 1;
    }
    else {
        DebugPrint(("ExAllocatePoolWithTag (NonPagedPool) failed"));
        return;
    }

    LARGE_INTEGER freq;
    KeQueryPerformanceCounter(&freq);
    global.tick_interval = 1e3 / freq.QuadPart;
}

VOID
DeviceSetup(WDFOBJECT hDevice) 
{
    auto* devExt = FilterGetData(hDevice);

    auto set_ext_from_cfg = [devExt](const ra::device_config& cfg) {
        devExt->enable = !cfg.disable;
        devExt->set_extra_info = cfg.set_extra_info;
        devExt->keep_time = cfg.polling_rate <= 0;
        devExt->dpi_factor = (cfg.dpi > 0) ? (1000.0 / cfg.dpi) : 1;

        if (devExt->keep_time) {
            devExt->clamp = cfg.clamp;
        }
        else {
            milliseconds interval = 1000.0 / cfg.polling_rate;
            devExt->clamp = { interval, interval };
        }
    };

    set_ext_from_cfg(global.default_dev_cfg);
    devExt->counter = 0;
    devExt->carry = {};
    devExt->drv_ptr = global.driver_data;
    devExt->mod_ptr = global.modifier_data;

    for (auto i = 0u; i < global.device_data_size; i++) {
        auto& dev_settings = global.device_data[i];

        if (wcsncmp(devExt->dev_id, dev_settings.id, ra::MAX_DEV_ID_LEN) == 0) {
            set_ext_from_cfg(dev_settings.config);

            if (dev_settings.profile[0] != L'\0') {
                for (auto j = 0u; j < global.driver_data_size; j++) {
                    auto& profile = global.driver_data[j].prof;

                    if (wcsncmp(dev_settings.profile, profile.name, ra::MAX_NAME_LEN) == 0) {
                        devExt->drv_ptr = &global.driver_data[j];
                        devExt->mod_ptr = &global.modifier_data[j];
                        return;
                    }
                }
            }

            return;
        }
    }
}

VOID
DeviceCleanup(WDFOBJECT hDevice)
{
    PAGED_CODE();
    DebugPrint(("Removing device from collection\n"));

    WdfWaitLockAcquire(global.collection_lock, NULL);
    WdfCollectionRemove(global.device_collection, hDevice);
    WdfWaitLockRelease(global.collection_lock);
}

VOID 
WriteDelay()
{
    LARGE_INTEGER interval;
    interval.QuadPart = static_cast<LONGLONG>(ra::WRITE_DELAY) * -10000;
    KeDelayExecutionThread(KernelMode, FALSE, &interval);
}

NTSTATUS
DriverEntry(
    IN  PDRIVER_OBJECT  DriverObject,
    IN  PUNICODE_STRING RegistryPath
)
/*++
Routine Description:

     Installable driver initialization entry point.
    This entry point is called directly by the I/O system.

--*/
{

    WDF_DRIVER_CONFIG config;
    NTSTATUS status;
    WDFDRIVER driver;

    DebugPrint(("km accel filter.\n"));
    DebugPrint(("Built %s %s\n", __DATE__, __TIME__));

    // Initialize driver config to control the attributes that
    // are global to the driver. Note that framework by default
    // provides a driver unload routine. If you create any resources
    // in the DriverEntry and want to be cleaned in driver unload,
    // you can override that by manually setting the EvtDriverUnload in the
    // config structure. In general xxx_CONFIG_INIT macros are provided to
    // initialize most commonly used members.

    WDF_DRIVER_CONFIG_INIT(
        &config,
        EvtDeviceAdd
    );

    //
    // Create a framework driver object to represent our driver.
    //
    status = WdfDriverCreate(DriverObject,
        RegistryPath,
        WDF_NO_OBJECT_ATTRIBUTES,
        &config,
        &driver);


    if (NT_SUCCESS(status)) {
        RawaccelInit(driver);
    }
    else {
        DebugPrint(("WdfDriverCreate failed with status 0x%x\n", status));
    }

    return status;
}


NTSTATUS
CreateControlDevice(WDFDRIVER Driver)
/*++
Routine Description:
    This routine is called to create a control device object so that application
    can talk to the filter driver directly instead of going through the entire
    device stack. This kind of control device object is useful if the filter
    driver is underneath another driver which prevents ioctls not known to it
    or if the driver's dispatch routine is owned by some other (port/class)
    driver and it doesn't allow any custom ioctls.
Arguments:
    Driver - Handle to wdf driver object.
Return Value:
    WDF status code
--*/
{
    PWDFDEVICE_INIT             pInit = NULL;
    WDFDEVICE                   controlDevice = NULL;
    WDF_IO_QUEUE_CONFIG         ioQueueConfig;
    NTSTATUS                    status;
    WDFQUEUE                    queue;
    DECLARE_CONST_UNICODE_STRING(ntDeviceName, NTDEVICE_NAME);
    DECLARE_CONST_UNICODE_STRING(symbolicLinkName, SYMBOLIC_NAME_STRING);

    DebugPrint(("Creating Control Device\n"));

    //
    //
    // In order to create a control device, we first need to allocate a
    // WDFDEVICE_INIT structure and set all properties.
    //
    pInit = WdfControlDeviceInitAllocate(
        Driver,
        &SDDL_DEVOBJ_SYS_ALL_ADM_RWX_WORLD_RW_RES_R
    );

    if (pInit == NULL) {
        status = STATUS_INSUFFICIENT_RESOURCES;
        goto Error;
    }

    //
    // Set exclusive to false so that more than one app can talk to the
    // control device simultaneously.
    //
    WdfDeviceInitSetExclusive(pInit, FALSE);

    status = WdfDeviceInitAssignName(pInit, &ntDeviceName);

    if (!NT_SUCCESS(status)) {
        goto Error;
    }

    status = WdfDeviceCreate(&pInit,
        WDF_NO_OBJECT_ATTRIBUTES,
        &controlDevice);

    if (!NT_SUCCESS(status)) {
        goto Error;
    }

    //
    // Create a symbolic link for the control object so that usermode can open
    // the device.
    //

    status = WdfDeviceCreateSymbolicLink(controlDevice, &symbolicLinkName);

    if (!NT_SUCCESS(status)) {
        goto Error;
    }

    //
    // Configure the default queue associated with the control device object
    // to be Serial so that request passed to RawaccelControl are serialized.
    //

    WDF_IO_QUEUE_CONFIG_INIT_DEFAULT_QUEUE(&ioQueueConfig,
        WdfIoQueueDispatchSequential);

    ioQueueConfig.EvtIoDeviceControl = RawaccelControl;

    //
    // Framework by default creates non-power managed queues for
    // filter drivers.
    //
    status = WdfIoQueueCreate(controlDevice,
        &ioQueueConfig,
        WDF_NO_OBJECT_ATTRIBUTES,
        &queue // pointer to default queue
    );
    if (!NT_SUCCESS(status)) {
        goto Error;
    }

    //
    // Control devices must notify WDF when they are done initializing.   I/O is
    // rejected until this call is made.
    //
    WdfControlFinishInitializing(controlDevice);

    return STATUS_SUCCESS;

Error:

    if (pInit != NULL) WdfDeviceInitFree(pInit);

    if (controlDevice != NULL) {
        //
        // Release the reference on the newly created object, since
        // we couldn't initialize it.
        //
        WdfObjectDelete(controlDevice);
    }

    DebugPrint(("CreateControlDevice failed with status code 0x%x\n", status));

    return status;
}

NTSTATUS
EvtDeviceAdd(
    IN WDFDRIVER        Driver,
    IN PWDFDEVICE_INIT  DeviceInit
    )
/*++
Routine Description:

    EvtDeviceAdd is called by the framework in response to AddDevice
    call from the PnP manager. Here you can query the device properties
    using WdfFdoInitWdmGetPhysicalDevice/IoGetDeviceProperty and based
    on that, decide to create a filter device object and attach to the
    function stack.

    If you are not interested in filtering this particular instance of the
    device, you can just return STATUS_SUCCESS without creating a framework
    device.

Arguments:

    Driver - Handle to a framework driver object created in DriverEntry

    DeviceInit - Pointer to a framework-allocated WDFDEVICE_INIT structure.

Return Value:

    NTSTATUS

--*/   
{
    WDF_OBJECT_ATTRIBUTES deviceAttributes;
    NTSTATUS status;
    WDFDEVICE hDevice;
    WDF_IO_QUEUE_CONFIG ioQueueConfig;

    UNREFERENCED_PARAMETER(Driver);

    PAGED_CODE();

    DebugPrint(("Enter FilterEvtDeviceAdd \n"));

    if (init_failed()) {
        return STATUS_SUCCESS;
    }

    //
    // Tell the framework that you are filter driver. Framework
    // takes care of inherting all the device flags & characterstics
    // from the lower device you are attaching to.
    //
    WdfFdoInitSetFilter(DeviceInit);

    WdfDeviceInitSetDeviceType(DeviceInit, FILE_DEVICE_MOUSE);

    WDF_OBJECT_ATTRIBUTES_INIT_CONTEXT_TYPE(&deviceAttributes,
        DEVICE_EXTENSION);

    deviceAttributes.EvtCleanupCallback = DeviceCleanup;

    //
    // Create a framework device object.  This call will in turn create
    // a WDM deviceobject, attach to the lower stack and set the
    // appropriate flags and attributes.
    //
    status = WdfDeviceCreate(&DeviceInit, &deviceAttributes, &hDevice);
    if (!NT_SUCCESS(status)) {
        DebugPrint(("WdfDeviceCreate failed with status code 0x%x\n", status));
        return status;
    }

    //
    // get device id from bus driver
    //
    DEVICE_OBJECT* pdo = WdfDeviceWdmGetPhysicalDevice(hDevice);
    
    KEVENT ke;
    KeInitializeEvent(&ke, NotificationEvent, FALSE);
    IO_STATUS_BLOCK iosb = {};
    PIRP Irp = IoBuildSynchronousFsdRequest(IRP_MJ_PNP,
        pdo, NULL, 0, NULL, &ke, &iosb);
    Irp->IoStatus.Status = STATUS_NOT_SUPPORTED;
    PIO_STACK_LOCATION stack = IoGetNextIrpStackLocation(Irp);
    stack->MinorFunction = IRP_MN_QUERY_ID;
    stack->Parameters.QueryId.IdType = BusQueryDeviceID;

    NTSTATUS tmp = IoCallDriver(pdo, Irp);

    if (tmp == STATUS_PENDING) {
        KeWaitForSingleObject(&ke, Executive, KernelMode, FALSE, NULL);
        tmp = iosb.Status;
    }

    auto* devExt = FilterGetData(hDevice);

    if (NT_SUCCESS(tmp)) {
        auto* id_ptr = reinterpret_cast<WCHAR*>(iosb.Information);
        wcsncpy(devExt->dev_id, id_ptr, ra::MAX_DEV_ID_LEN);
        ExFreePool(id_ptr);
    }
    else {
        DebugPrint(("IoCallDriver failed with status 0x%x\n", tmp));
        *devExt->dev_id = L'\0';
    }

    WdfWaitLockAcquire(global.collection_lock, NULL);

    DeviceSetup(hDevice);

    tmp = WdfCollectionAdd(global.device_collection, hDevice);

    if (!NT_SUCCESS(tmp)) {
        DebugPrint(("WdfCollectionAdd failed with status 0x%x\n", tmp));
    }

    WdfWaitLockRelease(global.collection_lock);

    //
    // Configure the default queue to be Parallel. Do not use sequential queue
    // if this driver is going to be filtering PS2 ports because it can lead to
    // deadlock. The PS2 port driver sends a request to the top of the stack when it
    // receives an ioctl request and waits for it to be completed. If you use a
    // a sequential queue, this request will be stuck in the queue because of the 
    // outstanding ioctl request sent earlier to the port driver.
    //
    WDF_IO_QUEUE_CONFIG_INIT_DEFAULT_QUEUE(&ioQueueConfig,
                             WdfIoQueueDispatchParallel);

    //
    // Framework by default creates non-power managed queues for
    // filter drivers.
    //
    ioQueueConfig.EvtIoInternalDeviceControl = EvtIoInternalDeviceControl;

    status = WdfIoQueueCreate(hDevice,
                            &ioQueueConfig,
                            WDF_NO_OBJECT_ATTRIBUTES,
                            WDF_NO_HANDLE // pointer to default queue
                            );
    if (!NT_SUCCESS(status)) {
        DebugPrint( ("WdfIoQueueCreate failed 0x%x\n", status));
        return status;
    }

    return status;
}


VOID
EvtIoInternalDeviceControl(
    IN WDFQUEUE      Queue,
    IN WDFREQUEST    Request,
    IN size_t        OutputBufferLength,
    IN size_t        InputBufferLength,
    IN ULONG         IoControlCode
    )
/*++

Routine Description:

    This routine is the dispatch routine for internal device control requests.
    There are two specific control codes that are of interest:
    
    IOCTL_INTERNAL_MOUSE_CONNECT:
        Store the old context and function pointer and replace it with our own.
        This makes life much simpler than intercepting IRPs sent by the RIT and
        modifying them on the way back up.
                                      
    IOCTL_INTERNAL_I8042_HOOK_MOUSE:
        Add in the necessary function pointers and context values so that we can
        alter how the ps/2 mouse is initialized.
                                            
    NOTE:  Handling IOCTL_INTERNAL_I8042_HOOK_MOUSE is *NOT* necessary if 
           all you want to do is filter MOUSE_INPUT_DATAs.  You can remove
           the handling code and all related device extension fields and 
           functions to conserve space.
                                         

--*/
{
    
    PDEVICE_EXTENSION devExt;
    PCONNECT_DATA connectData;
    NTSTATUS status = STATUS_SUCCESS;
    WDFDEVICE hDevice;
    size_t length;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    PAGED_CODE();

    hDevice = WdfIoQueueGetDevice(Queue);
    devExt = FilterGetData(hDevice);

    switch (IoControlCode) {

    //
    // Connect a mouse class device driver to the port driver.
    //
    case IOCTL_INTERNAL_MOUSE_CONNECT: {
        //
        // Only allow one connection.
        //
        if (devExt->UpperConnectData.ClassService != NULL) {
            status = STATUS_SHARING_VIOLATION;
            break;
        }

        //
        // Copy the connection parameters to the device extension.
        //
        status = WdfRequestRetrieveInputBuffer(Request,
            sizeof(CONNECT_DATA),
            reinterpret_cast<PVOID*>(&connectData),
            &length);
        if (!NT_SUCCESS(status)) {
            DebugPrint(("WdfRequestRetrieveInputBuffer failed %x\n", status));
            break;
        }

        devExt->UpperConnectData = *connectData;

        //
        // Hook into the report chain.  Everytime a mouse packet is reported to
        // the system, RawaccelCallback will be called
        //

        connectData->ClassDeviceObject = WdfDeviceWdmGetDeviceObject(hDevice);
        connectData->ClassService = RawaccelCallback;

        break;
    }
    //
    // Disconnect a mouse class device driver from the port driver.
    //
    case IOCTL_INTERNAL_MOUSE_DISCONNECT: 
        //
        // Clear the connection parameters in the device extension.
        //
        // devExt->UpperConnectData.ClassDeviceObject = NULL;
        // devExt->UpperConnectData.ClassService = NULL;
        status = STATUS_NOT_IMPLEMENTED;
        break;
    case IOCTL_MOUSE_QUERY_ATTRIBUTES:
    default:
        break;
    }

    if (!NT_SUCCESS(status)) {
        WdfRequestComplete(Request, status);
        return;
    }

    DispatchPassThrough(Request, WdfDeviceGetIoTarget(hDevice));
}


VOID
DispatchPassThrough(
    _In_ WDFREQUEST Request,
    _In_ WDFIOTARGET Target
)
/*++
Routine Description:

    Passes a request on to the lower driver.


--*/
{
    //
    // Pass the IRP to the target
    //

    WDF_REQUEST_SEND_OPTIONS options;
    BOOLEAN ret;
    NTSTATUS status = STATUS_SUCCESS;

    //
    // We are not interested in post processing the IRP so 
    // fire and forget.
    //
    WDF_REQUEST_SEND_OPTIONS_INIT(&options,
        WDF_REQUEST_SEND_OPTION_SEND_AND_FORGET);

    ret = WdfRequestSend(Request, Target, &options);

    if (ret == FALSE) {
        status = WdfRequestGetStatus(Request);
        DebugPrint(("WdfRequestSend failed: 0x%x\n", status));
        WdfRequestComplete(Request, status);
    }

    return;
}
