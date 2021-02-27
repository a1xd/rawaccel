#include <iostream>

#include <utility-install.hpp>
#include <VersionHelpers.h>

void add_service(SC_HANDLE srv_manager) {
    SC_HANDLE srv = CreateServiceW(
        srv_manager,               // SCM database 
        DRIVER_NAME.c_str(),       // name of service 
        DRIVER_NAME.c_str(),       // service name to display 
        SERVICE_ALL_ACCESS,        // desired access 
        SERVICE_KERNEL_DRIVER,     // service type 
        SERVICE_DEMAND_START,      // start type 
        SERVICE_ERROR_NORMAL,      // error control type 
        DRIVER_ENV_PATH.c_str(),   // path to service's binary 
        NULL,                      // no load ordering group 
        NULL,                      // no tag identifier 
        NULL,                      // no dependencies 
        NULL,                      // LocalSystem account 
        NULL                       // no password 
    );

    if (srv) CloseServiceHandle(srv);
    else throw sys_error("CreateService failed");
}

BOOL update_service(SC_HANDLE srv) {
    return ChangeServiceConfigW(
        srv,                       // service handle 
        SERVICE_KERNEL_DRIVER,     // service type 
        SERVICE_DEMAND_START,      // start type 
        SERVICE_ERROR_NORMAL,      // error control type 
        DRIVER_ENV_PATH.c_str(),   // path to service's binary 
        NULL,                      // no load ordering group 
        NULL,                      // no tag identifier 
        NULL,                      // no dependencies 
        NULL,                      // LocalSystem account 
        NULL,                      // no password
        DRIVER_NAME.c_str()        // service name to display
    );
}

int main() {
    SC_HANDLE srv_manager = NULL;

    try {
        if (!IsWindows10OrGreater()) {
            throw std::runtime_error("OS not supported, you need at least Windows 10");
        }

        fs::path source = fs::path(L"driver") / DRIVER_FILE_NAME;

        if (!fs::exists(source)) {
            throw std::runtime_error(source.generic_string() + " does not exist");
        }

        fs::path target = expand(DRIVER_ENV_PATH);
        
        if (fs::exists(target)) {
            std::cout << "Driver already installed. Removing previous installation.\n";

            fs::path tmp = make_temp_path(target);

            // schedule tmp to be deleted if rename target -> tmp is successful
            if (MoveFileExW(target.c_str(), tmp.c_str(), MOVEFILE_REPLACE_EXISTING)) {
                MoveFileExW(tmp.c_str(), NULL, MOVEFILE_DELAY_UNTIL_REBOOT);
            }
        }

        if (!fs::copy_file(source, target, fs::copy_options::overwrite_existing)) {
            throw sys_error("copy_file failed");
        }

        srv_manager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);
        if (srv_manager == NULL) throw sys_error("OpenSCManager failed");

        SC_HANDLE srv = OpenServiceW(srv_manager, DRIVER_NAME.c_str(), SC_MANAGER_ALL_ACCESS);

        if (srv != NULL) {
            BOOL success = update_service(srv);
            CloseServiceHandle(srv);
            if (!success) throw sys_error("ChangeServiceConfig failed");
        }
        else {
            auto error_code = GetLastError();
            if (error_code == ERROR_SERVICE_DOES_NOT_EXIST) {
                add_service(srv_manager);
            }
            else {
                throw sys_error("OpenService failed", error_code);
            }
        }

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

    if (srv_manager) CloseServiceHandle(srv_manager);

    std::cout << "Press any key to close this window . . .\n";
    _getwch();
}
