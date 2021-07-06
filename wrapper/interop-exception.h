#pragma once

#include <exception>

public ref struct InteropException : System::Exception {
    InteropException(System::String^ what) :
        Exception(what) {}
    InteropException(const char* what) :
        Exception(gcnew System::String(what)) {}
    InteropException(const std::exception& e) :
        InteropException(e.what()) {}
};

public ref struct RawInputInteropException : InteropException {
    RawInputInteropException(System::String^ what) :
        InteropException(what) {}
    RawInputInteropException(const char* what) :
        InteropException(what) {}
    RawInputInteropException(const std::exception& e) :
        InteropException(e) {}
};
