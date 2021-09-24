#include "input.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

[StructLayout(LayoutKind::Sequential, CharSet = CharSet::Unicode)]
public ref struct RawInputDevice {
    System::IntPtr handle;

    [MarshalAs(UnmanagedType::ByValTStr, SizeConst = MAX_NAME_LEN)]
    System::String^ name;

    [MarshalAs(UnmanagedType::ByValTStr, SizeConst = MAX_DEV_ID_LEN)]
    System::String^ id;
};

static int CompareByID(RawInputDevice^ x, RawInputDevice^ y)
{
    return String::Compare(x->id, y->id);
}

public ref struct MultiHandleDevice {
    System::String^ name;
    System::String^ id;
    List<System::IntPtr>^ handles;

    // Returned list represents the current connected raw input devices,
    // where each device has a distinct device id
    // https://docs.microsoft.com/en-us/windows-hardware/drivers/install/device-ids
    static IList<MultiHandleDevice^>^ GetList()
    {
        return ListMaker::MakeList()->AsReadOnly();
    }

    ref class ListMaker {
        List<RawInputDevice^>^ devices = gcnew List<RawInputDevice^>();

        delegate void NativeDevHandler(rawinput_device&);

        void Add(rawinput_device& dev)
        {
            devices->Add(Marshal::PtrToStructure<RawInputDevice^>(IntPtr(&dev)));
        }

        ListMaker() {}
    public:
        static List<MultiHandleDevice^>^ MakeList()
        {
            auto maker = gcnew ListMaker();
            NativeDevHandler^ del = gcnew NativeDevHandler(maker, &Add);
            GCHandle gch = GCHandle::Alloc(del);
            auto fp = static_cast<void (*)(rawinput_device&)>(
                Marshal::GetFunctionPointerForDelegate(del).ToPointer());
            rawinput_foreach(fp);
            gch.Free();

            auto ret = gcnew List<MultiHandleDevice^>();
            auto count = maker->devices->Count;
            auto first = 0;
            auto last = 0;

            if (count > 0) {
                maker->devices->Sort(gcnew Comparison<RawInputDevice^>(&CompareByID));
                while (++last != count) {
                    if (!String::Equals(maker->devices[first]->id, maker->devices[last]->id)) {
                        auto range = maker->devices->GetRange(first, last - first);
                        ret->Add(gcnew MultiHandleDevice(range));
                        first = last;
                    }
                }
                auto range = maker->devices->GetRange(first, last - first);
                ret->Add(gcnew MultiHandleDevice(range));
            }

            return ret;
        }
    };

private:
    MultiHandleDevice(IEnumerable<RawInputDevice^>^ seq)
    {
        auto it = seq->GetEnumerator();
        if (it->MoveNext()) {
            name = it->Current->name;
            id = it->Current->id;
            handles = gcnew List<IntPtr>();
            do handles->Add(it->Current->handle); while (it->MoveNext());
        }
    }
};
