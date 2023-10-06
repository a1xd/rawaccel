#include <iostream>

#include <utility-install.hpp>
#include <VersionHelpers.h>

void add_service(SC_HANDLE srv_manager) {
    SC_HANDLE srv = CreateServiceW(
        srv_manager,               // SCM database 
        DRV_NAME,                  // name of service 
        DRV_NAME,                  // service name to display 
        SERVICE_ALL_ACCESS,        // desired access 
        SERVICE_KERNEL_DRIVER,     // service type 
        SERVICE_DEMAND_START,      // start type 
        SERVICE_ERROR_NORMAL,      // error control type 
        DRV_DST_PATH,              // path to service's binary 
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
        DRV_DST_PATH,              // path to service's binary 
        NULL,                      // no load ordering group 
        NULL,                      // no tag identifier 
        NULL,                      // no dependencies 
        NULL,                      // LocalSystem account 
        NULL,                      // no password
        DRV_NAME                   // service name to display
    );
}

int main() {
    SC_HANDLE srv_manager = NULL;

    try {
        if (!IsWindows10OrGreater()) {
            throw std::runtime_error("OS not supported, you need at least Windows 10");
        }

        const fs::path bin = DRV_SRC_PATH;

        if (!fs::exists(bin)) {
            throw std::runtime_error("Can't find driver binary");
        }

        fs::path target = expand(DRV_DST_PATH);

        if (fs::exists(target)) {
            std::cout << "Driver already installed. Removing previous installation.\n";

            fs::path tmp = make_temp_path(target);

            // schedule tmp to be deleted if rename target -> tmp is successful
            if (MoveFileExW(target.c_str(), tmp.c_str(), MOVEFILE_REPLACE_EXISTING)) {
                MoveFileExW(tmp.c_str(), NULL, MOVEFILE_DELAY_UNTIL_REBOOT);
            }
        }

        if (!fs::copy_file(bin, target, fs::copy_options::overwrite_existing)) {
            throw sys_error("copy_file failed");
        }

        srv_manager = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);
        if (srv_manager == NULL) throw sys_error("OpenSCManager failed");

        SC_HANDLE srv = OpenServiceW(srv_manager, DRV_NAME, SC_MANAGER_ALL_ACCESS);

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

        set_registry_filter();

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