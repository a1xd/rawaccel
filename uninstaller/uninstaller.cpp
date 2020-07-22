#include <iostream>

#include <utility-install.hpp>

int main() {
    try {
        modify_upper_filters([](std::vector<std::wstring>& filters) {
            std::erase(filters, DRIVER_NAME);
        });

        fs::path target = get_target_path();
        fs::path tmp = make_temp_path(target);

        // schedule tmp to be deleted if rename target -> tmp is successful
        if (MoveFileExW(target.c_str(), tmp.c_str(), MOVEFILE_REPLACE_EXISTING)) {
            MoveFileExW(tmp.c_str(), NULL, MOVEFILE_DELAY_UNTIL_REBOOT);
        }
        else { // tmp is in use and delete is already scheduled
            if (fs::exists(target)) fs::remove(target);
        }
        std::cout << "Removal complete, change will take effect after restart.\n";
    }
    catch (std::exception e) {
        std::cerr << "Error: " << e.what() << '\n';
    }

    std::cout << "Press any key to close this window . . .\n";
    _getwch();
}
