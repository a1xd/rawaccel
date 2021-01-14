#pragma once

#include <ntddk.h>
#include <kbdmou.h>
#include <wdf.h>

#include "rawaccel-settings.h"

#if DBG
#define DebugPrint(_x_) DbgPrint _x_
#else
#define DebugPrint(_x_)
#endif

#define NTDEVICE_NAME         L"\\Device\\rawaccel"
#define SYMBOLIC_NAME_STRING  L"\\DosDevices\\rawaccel"

using counter_t = long long;

typedef struct _DEVICE_EXTENSION {
    counter_t counter;
    vec2d carry;
    CONNECT_DATA UpperConnectData;
    WCHAR dev_id[MAX_DEV_ID_LEN];
} DEVICE_EXTENSION, *PDEVICE_EXTENSION;

WDF_DECLARE_CONTEXT_TYPE_WITH_NAME(DEVICE_EXTENSION, FilterGetData)

EXTERN_C_START

DRIVER_INITIALIZE DriverEntry;
EVT_WDF_DRIVER_DEVICE_ADD EvtDeviceAdd;
EVT_WDF_IO_QUEUE_IO_INTERNAL_DEVICE_CONTROL EvtIoInternalDeviceControl;
EVT_WDF_IO_QUEUE_IO_DEVICE_CONTROL RawaccelControl;

VOID RawaccelCallback(
    IN PDEVICE_OBJECT DeviceObject,
    IN PMOUSE_INPUT_DATA InputDataStart,
    IN PMOUSE_INPUT_DATA InputDataEnd,
    IN OUT PULONG InputDataConsumed
);

EXTERN_C_END

VOID CreateControlDevice(WDFDRIVER Driver);

VOID DispatchPassThrough(
    _In_ WDFREQUEST Request,
    _In_ WDFIOTARGET Target
);
