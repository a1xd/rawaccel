#include <rawaccel.hpp>
#include <rawaccel-io-def.h>
#include <rawaccel-version.h>

#include "driver.h"

#ifdef ALLOC_PRAGMA
#pragma alloc_text (INIT, DriverEntry)
#pragma alloc_text (PAGE, EvtDeviceAdd)
#pragma alloc_text (PAGE, EvtIoInternalDeviceControl)
#pragma alloc_text (PAGE, RawaccelControl)
#endif

namespace ra = rawaccel;

using milliseconds = double;
using lut_value_t = ra::si_pair;

struct {
    ra::settings args;
    milliseconds tick_interval = 0; // set in DriverEntry
    ra::mouse_modifier modifier;
    vec2<lut_value_t*> lookups = {};
} global;

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

    bool any = num_packets > 0;
    bool rel_move = !(InputDataStart->Flags & MOUSE_MOVE_ABSOLUTE);
    bool dev_match = global.args.device_id[0] == 0 ||
        wcsncmp(devExt->dev_id, global.args.device_id, MAX_DEV_ID_LEN) == 0;

    if (any && rel_move && dev_match) {
        // if IO is backed up to the point where we get more than 1 packet here
        // then applying accel is pointless as we can't get an accurate timing
        bool enable_accel = num_packets == 1;

        auto it = InputDataStart;
        do {
            vec2d input = {
                static_cast<double>(it->LastX),
                static_cast<double>(it->LastY)
            };

            global.modifier.apply_rotation(input);
            global.modifier.apply_angle_snap(input);

            if (enable_accel) {
                auto time_supplier = [=] {
                    counter_t now = KeQueryPerformanceCounter(NULL).QuadPart;
                    counter_t ticks = now - devExt->counter;
                    devExt->counter = now;
                    milliseconds time = ticks * global.tick_interval;
                    return clampsd(time, global.args.time_min, 100);
                };

                global.modifier.apply_acceleration(input, time_supplier);
            }

            global.modifier.apply_sensitivity(input);

            double carried_result_x = input.x + devExt->carry.x;
            double carried_result_y = input.y + devExt->carry.y;

            LONG out_x = static_cast<LONG>(carried_result_x);
            LONG out_y = static_cast<LONG>(carried_result_y);

            devExt->carry.x = carried_result_x - out_x;
            devExt->carry.y = carried_result_y - out_y;

            it->LastX = out_x;
            it->LastY = out_y;

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

    switch (IoControlCode) {
    case RA_READ:
        status = WdfRequestRetrieveOutputBuffer(
            Request,
            sizeof(ra::settings),
            &buffer,
            NULL
        );
        if (!NT_SUCCESS(status)) {
            DebugPrint(("RetrieveOutputBuffer failed: 0x%x\n", status));
        }
        else {
            *reinterpret_cast<ra::settings*>(buffer) = global.args;
            bytes_out = sizeof(ra::settings);
        }
        break;
    case RA_WRITE:
        status = WdfRequestRetrieveInputBuffer(
            Request,
            sizeof(ra::settings),
            &buffer,
            NULL
        );
        if (!NT_SUCCESS(status)) {
            DebugPrint(("RetrieveInputBuffer failed: 0x%x\n", status));
        }
        else {
            LARGE_INTEGER interval;
            interval.QuadPart = static_cast<LONGLONG>(ra::WRITE_DELAY) * -10000;
            KeDelayExecutionThread(KernelMode, FALSE, &interval);

            ra::settings new_settings = *reinterpret_cast<ra::settings*>(buffer);

            if (new_settings.time_min <= 0 || _isnanf(static_cast<float>(new_settings.time_min))) {
                new_settings.time_min = ra::settings{}.time_min;
            }

            global.args = new_settings;
            global.modifier = { global.args, global.lookups };
        }
        break;
    case RA_GET_VERSION:
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
            *reinterpret_cast<ra::version_t*>(buffer) = { RA_VER_MAJOR, RA_VER_MINOR, RA_VER_PATCH };
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
        LARGE_INTEGER freq;
        KeQueryPerformanceCounter(&freq);
        global.tick_interval = 1e3 / freq.QuadPart;

        auto make_lut = [] {
            const size_t POOL_SIZE = sizeof(lut_value_t) * ra::LUT_SIZE;

            auto pool = ExAllocatePoolWithTag(NonPagedPool, POOL_SIZE, 'AR');

            if (!pool) {
                DebugPrint(("RA - failed to allocate LUT\n"));
            }
            else {
                RtlZeroMemory(pool, POOL_SIZE);
            }

            return reinterpret_cast<lut_value_t*>(pool);
        };

        global.lookups = { make_lut(), make_lut() };

        CreateControlDevice(driver);
    }
    else {
        DebugPrint(("WdfDriverCreate failed with status 0x%x\n", status));
    }

    return status;
}


inline
VOID
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

    return;

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

    //
    // Tell the framework that you are filter driver. Framework
    // takes care of inherting all the device flags & characterstics
    // from the lower device you are attaching to.
    //
    WdfFdoInitSetFilter(DeviceInit);

    WdfDeviceInitSetDeviceType(DeviceInit, FILE_DEVICE_MOUSE);

    WDF_OBJECT_ATTRIBUTES_INIT_CONTEXT_TYPE(&deviceAttributes,
        DEVICE_EXTENSION);


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

    NTSTATUS nts = IoCallDriver(pdo, Irp);

    if (nts == STATUS_PENDING) {
        KeWaitForSingleObject(&ke, Executive, KernelMode, FALSE, NULL);
    }

    if (NT_SUCCESS(nts)) {
        auto* id_ptr = reinterpret_cast<WCHAR*>(iosb.Information); 
        wcsncpy(FilterGetData(hDevice)->dev_id, id_ptr, MAX_DEV_ID_LEN);
        DebugPrint(("Device ID = %ws\n", id_ptr));
        ExFreePool(id_ptr);
    }

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

        devExt->counter = 0;
        devExt->carry = {};
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


inline
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
