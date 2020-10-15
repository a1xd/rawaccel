#include <array>
#include <charconv>
#include <filesystem>
#include <fstream>
#include <iostream>
#include <optional>
#include <sstream>
#include <string>

#include <rawaccel-settings.h>

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace Newtonsoft::Json;

namespace ra = rawaccel;
namespace fs = std::filesystem;

const wchar_t* IA_SETTINGS_NAME = L"settings.txt";
const wchar_t* IA_PROFILE_EXT = L".profile";

enum IA_MODES_ENUM { IA_QL, IA_NAT, IA_LOG };

constexpr std::array<std::string_view, 3> IA_MODES = {
    "QuakeLive", "Natural", "Logarithmic"
};

// trim from start (in place)
static inline void ltrim(std::string& s) {
    s.erase(s.begin(), std::find_if(s.begin(), s.end(), [](unsigned char ch) {
        return !std::isspace(ch);
    }));
}

// trim from end (in place)
static inline void rtrim(std::string& s) {
    s.erase(std::find_if(s.rbegin(), s.rend(), [](unsigned char ch) {
        return !std::isspace(ch);
    }).base(), s.end());
}

// trim from both ends (in place)
static inline void trim(std::string& s) {
    ltrim(s);
    rtrim(s);
}

bool ask(std::string_view question) {
    std::cout << question << " (Y/N)" << std::endl;
    wchar_t ch;
    bool yes;
    do
    {
        ch = towupper(_getwch());
        yes = ch == 'Y';
    } while (ch != 'N' && !yes);
    return yes;
}

using ia_settings_t = std::vector<std::pair<std::string, double>>;

ia_settings_t parse_ia_settings(const fs::path fp) {
    ia_settings_t kv_pairs;

    std::ifstream ifs(fp);
    std::string line;

    while (std::getline(ifs, line)) {
        if (line.empty()) continue;

        auto delim_pos = line.find('=');
        if (delim_pos == std::string::npos) continue;

        std::string key(line.substr(0, delim_pos));
        trim(key);

        auto val_pos = line.find_first_not_of(" \t", delim_pos + 1);
        if (val_pos == std::string::npos) continue;

        double val = 0;

        auto [p, ec] = std::from_chars(&line[val_pos], &line[0] + line.size(), val);

        if (ec == std::errc()) {
            kv_pairs.emplace_back(key, val);
        }
        else if (key == "AccelMode") {
            std::string mode_val = line.substr(val_pos);
            rtrim(mode_val);
            auto it = std::find(IA_MODES.begin(), IA_MODES.end(), mode_val);
            if (it != IA_MODES.end()) {
                val = static_cast<double>(std::distance(IA_MODES.begin(), it));
                kv_pairs.emplace_back(key, val);
            }
        }
    }

    return kv_pairs;
}

auto make_extractor(const ia_settings_t& ia_settings) {
    return [&](auto... keys) -> std::optional<double> {
        auto it = std::find_if(ia_settings.begin(), ia_settings.end(), [=](auto&& p) {
            return ((p.first == keys) || ...);
        });
        if (it == ia_settings.end()) return std::nullopt;
        return it->second;
    };
}

ra::accel_args convert_natural(const ia_settings_t& ia_settings) {
    auto get = make_extractor(ia_settings);

    double accel = get("Acceleration").value_or(0);
    double cap = get("SensitivityCap").value_or(0);
    double sens = get("Sensitivity").value_or(1);
    double prescale = get("Pre-ScaleX").value_or(1);

    ra::accel_args args;
    args.limit = 1 + std::abs(cap - sens) / sens;
    args.accel = accel * prescale / sens;
    return args;
}

ra::accel_args convert_quake(const ia_settings_t& ia_settings, bool legacy) {
    auto get = make_extractor(ia_settings);

    double power = get("Power").value_or(2);
    double accel = get("Acceleration").value_or(0);
    double cap = get("SensitivityCap").value_or(0);
    double sens = get("Sensitivity").value_or(1);
    double prescale = get("Pre-ScaleX").value_or(1);
    double offset = get("Offset").value_or(0);

    ra::accel_args args;

    double accel_b = std::pow(accel * prescale, power - 1) / sens;
    double accel_e = 1 / (power - 1);
    args.accel = std::pow(accel_b, accel_e);
    args.exponent = power;
    args.legacy_offset = legacy;
    args.offset = offset;
    
    double cap_converted = cap / sens;

    if (legacy || cap_converted <= 1) {
        args.scale_cap = cap_converted;
    }
    else {
        double b = (cap_converted - 1) / power;
        double e = 1 / (power - 1);
        args.gain_cap = offset + (1 / accel) * std::pow(b, e);
    }

    return args;
}

bool try_convert(const ia_settings_t& ia_settings) {
    auto get = make_extractor(ia_settings);

    ra::settings ra_settings;

    ra_settings.degrees_rotation = get("Angle", "AngleAdjustment").value_or(0);
    ra_settings.sens = {
        get("Post-ScaleX").value_or(1) * get("Pre-ScaleX").value_or(1),
        get("Post-ScaleY").value_or(1) * get("Pre-ScaleY").value_or(1)
    };

    double mode = get("AccelMode").value_or(IA_QL);

    switch (static_cast<IA_MODES_ENUM>(mode)) {
    case IA_QL: {
        bool legacy = !ask("We recommend trying out our new cap and offset styles.\n"
                           "Use new cap and offset?");
        ra_settings.modes.x = ra::accel_mode::classic;
        ra_settings.argsv.x = convert_quake(ia_settings, legacy);
        break;
    }
    case IA_NAT: {
        bool nat_gain = ask("Raw Accel offers a new mode that you might like more than Natural.\n"
                            "Wanna try it out now?");
        ra_settings.modes.x = nat_gain ? ra::accel_mode::naturalgain : ra::accel_mode::natural;
        ra_settings.argsv.x = convert_natural(ia_settings);
        break;
    }
    case IA_LOG: {
        std::cout << "Logarithmic accel mode is not supported.\n";
        return true;
    }
    default: return false;
    }

    DriverSettings^ new_settings = Marshal::PtrToStructure<DriverSettings^>((IntPtr)&ra_settings);
    auto errors = DriverInterop::GetSettingsErrors(new_settings);

    if (!errors->Empty()) {
        Console::WriteLine("Bad settings: " + errors->x->ToArray()[0]);
        return false;
    }

    Console::Write("Sending to driver... ");
    DriverInterop::Write(new_settings);
    Console::WriteLine("done");

    Console::Write("Generating settings.json... ");
    auto json = JsonConvert::SerializeObject(new_settings, Formatting::Indented);
    System::IO::File::WriteAllText("settings.json", json);
    Console::WriteLine("done");

    return true;
}

int main()
{
    std::optional<fs::path> opt_path;

    if (fs::exists(IA_SETTINGS_NAME)) {
        opt_path = IA_SETTINGS_NAME;
    }
    else {
        for (auto&& entry : fs::directory_iterator(".")) {
            if (fs::is_regular_file(entry) &&
                entry.path().extension() == IA_PROFILE_EXT) {
                opt_path = entry;
                break;
            }    
        }
    }
    
    if (opt_path) {
        std::string path = opt_path->filename().generic_string();
        std::stringstream ss;
        ss << "Found " << path << 
            "\n\nConvert and send settings generated from " << path << '?';
        if (ask(ss.str())) {
            try {
                if (!try_convert(parse_ia_settings(opt_path.value())))
                    std::cout << "Unable to convert settings.\n";
            }
            catch (DriverNotInstalledException^) {
                Console::WriteLine("\nDriver is not installed.");
            }
            catch (Exception^ e) {
                Console::WriteLine("\nError: " + e->ToString());
            }
            catch (const std::exception& e) {
                std::cout << "Error: " << e.what() << '\n';
            }
        }
    }
    else {
        std::cout << "Drop your InterAccel settings/profile into this directory.\n"
            "Then run this program to generate the equivalent Raw Accel settings.\n";
    }

    std::cout << "Press any key to close this window . . ." << std::endl;
    _getwch();
}
