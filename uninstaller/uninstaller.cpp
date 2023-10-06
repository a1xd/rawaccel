#include <iostream>

#include <utility-install.hpp>

int main() {
    try {
        bool reboot_required = set_registry_filter<0>();

        fs::path target = expand(DRV_DST_PATH);
        fs::path tmp = make_temp_path(target);

        if (fs::exists(target)) {
            reboot_required = true;

            // schedule tmp to be deleted if rename target -> tmp is successful
            if (MoveFileExW(target.c_str(), tmp.c_str(), MOVEFILE_REPLACE_EXISTING)) {
                MoveFileExW(tmp.c_str(), NULL, MOVEFILE_DELAY_UNTIL_REBOOT);
            }
            else { // tmp is in use and delete is already scheduled
                fs::remove(target);
            }
        }

        if (reboot_required || fs::exists(tmp)) {
            std::cout << "Removal complete, change will take effect after restart.\n";
        }
        else {
            std::cout << "No installed driver found.\n";
        }

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
