#include <iostream>

#include <utility-install.hpp>
#include <VersionHelpers.h>

void add_service(const fs::path& target) {
    SC_HANDLE schSCManager = OpenSCManager(
        NULL,                    // local computer
        NULL,                    // ServicesActive database 
        SC_MANAGER_ALL_ACCESS    // full access rights 
    );

    if (schSCManager == NULL) throw std::runtime_error("OpenSCManager failed");

    SC_HANDLE schService = CreateService(
        schSCManager,              // SCM database 
        DRIVER_NAME.c_str(),       // name of service 
        DRIVER_NAME.c_str(),       // service name to display 
        SERVICE_ALL_ACCESS,        // desired access 
        SERVICE_KERNEL_DRIVER,     // service type 
        SERVICE_DEMAND_START,      // start type 
        SERVICE_ERROR_NORMAL,      // error control type 
        target.c_str(),            // path to service's binary 
        NULL,                      // no load ordering group 
        NULL,                      // no tag identifier 
        NULL,                      // no dependencies 
        NULL,                      // LocalSystem account 
        NULL                       // no password 
    );

    if (schService) {
        CloseServiceHandle(schService);
        CloseServiceHandle(schSCManager);
        return;
    }

    if (auto err = GetLastError(); err != ERROR_SERVICE_EXISTS) {
        CloseServiceHandle(schSCManager);
        throw std::runtime_error("CreateService failed");
    }
}

int main() {
    try {
        if (!IsWindows10OrGreater()) {
            throw std::runtime_error("OS not supported, you need at least Windows 10");
        }
        fs::path source = fs::path(L"driver") / DRIVER_FILE_NAME;

        if (!fs::exists(source)) {
            throw std::runtime_error(source.generic_string() + " does not exist");
        }

        fs::path target = get_target_path();

        if (fs::exists(target)) {
            std::cout << "Driver already installed. Removing previous installation.\n";
        }

        add_service(target);

        fs::path tmp = make_temp_path(target);

        // schedule tmp to be deleted if rename target -> tmp is successful
        if (MoveFileExW(target.c_str(), tmp.c_str(), MOVEFILE_REPLACE_EXISTING)) {
            MoveFileExW(tmp.c_str(), NULL, MOVEFILE_DELAY_UNTIL_REBOOT);
        }

        fs::copy_file(source, target, fs::copy_options::overwrite_existing);

        modify_upper_filters([](std::vector<std::wstring>& filters) {
            auto driver_pos = std::find(filters.begin(), filters.end(), DRIVER_NAME);

            if (driver_pos != filters.end()) return;

            auto mouclass_pos = std::find(filters.begin(), filters.end(), L"mouclass");
            filters.insert(mouclass_pos, DRIVER_NAME);
        });

        std::cout << "Install complete, change will take effect after restart.\n";
    }
    catch (const std::system_error& e) {
        std::cerr << "Error: " << e.what() << ' ' << e.code() << '\n';
    }
    catch (const std::exception& e) {
        std::cerr << "Error: " << e.what() << '\n';
    }

    std::cout << "Press any key to close this window . . .\n";
    _getwch();
}
