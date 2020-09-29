#include <array>
#include <charconv>
#include <filesystem>
#include <fstream>
#include <iostream>
#include <optional>
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

bool yes() {
    bool b;
    while (wchar_t c = _getwch()) {
        if (c == 'y') {
            b = true;
            break;
        }
        else if (c == 'n') {
            b = false;
            break;
        }
    }
    return b;
}

bool try_convert(const fs::path& fp) {
    std::vector<std::pair<std::string, double>> kv_pairs;

    {
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
    }

    auto get = [&](auto... keys) -> std::optional<double> {
        auto it = std::find_if(kv_pairs.begin(), kv_pairs.end(), [=](auto&& p) {
            return ((p.first == keys) || ...);
        });
        if (it == kv_pairs.end()) return std::nullopt;
        return it->second;
    };

    ra::settings ra_settings;
    ra::accel_args& args = ra_settings.argsv.x;
    
    auto opt_mode = get("AccelMode");
    if (!opt_mode) return false;

    double accel = std::max(get("Acceleration").value_or(0), 0.0);
    double sens = get("Sensitivity").value_or(1);
    double cap = get("SensitivityCap").value_or(0);

    std::cout << "We recommend trying out our new cap and offset styles.\n"
        "Use new cap and offset? (y|n)\n";

    args.legacy_offset = !yes();
    args.offset = get("Offset").value_or(0);
    args.accel = accel / sens * get("Pre-ScaleX").value_or(1);
    args.limit = 1 + std::abs(cap - sens);

    ra_settings.degrees_rotation = get("Angle", "AngleAdjustment").value_or(0);
    ra_settings.sens = {
        get("Post-ScaleX").value_or(1),
        get("Post-ScaleY").value_or(1)
    };

    switch (static_cast<IA_MODES_ENUM>(opt_mode.value())) {
    case IA_QL: {
        auto opt_pow = get("Power");
        if (!opt_pow || opt_pow.value() <= 1) return false;
        args.exponent = opt_pow.value();
        
        if (args.legacy_offset || cap <= 1) {
            args.scale_cap = cap;
        }
        else {
            double b = (cap - 1) / args.exponent;
            double e = 1 / args.exponent - 1;
            args.gain_cap = args.offset + (1 / accel) * std::pow(b, e);
        }
        ra_settings.modes.x = ra::accel_mode::classic;
        break;
    }
    case IA_NAT: {
        std::cout << "Raw Accel offers a new mode that you might like more than Natural.\n"
            "Wanna try it out now? (y|n)\n";
        ra_settings.modes.x = yes() ? ra::accel_mode::naturalgain : ra::accel_mode::natural;
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

    if (errors->Empty()) {
        Console::Write("Sending to driver... ");
        DriverInterop::Write(new_settings);
        Console::WriteLine("done");

        Console::Write("Generating settings.json... ");
        auto json = JsonConvert::SerializeObject(new_settings, Formatting::Indented);
        System::IO::File::WriteAllText("settings.json", json);
        Console::WriteLine("done");
    }
    else {
        Console::WriteLine("Bad settings:\n" + errors->x->ToArray()[0]);
    }

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
        std::cout << "Found " << opt_path->filename() <<
          "\n\nConvert and send settings generated from " << 
            opt_path->filename() << "? (y|n)\n";
        if (!yes()) return 0;
        try {
            if (!try_convert(opt_path.value())) 
                std::cout << "Unable to convert settings.\n";
        }
        catch (DriverNotInstalledException^) {
            std::cout << "\nDriver is not installed\n";
        }
        catch (const std::exception& e) {
            std::cout << "Error: \n" << e.what() << '\n';
        }
    }
    else {
        std::cout << "Drop your InterAccel settings/profile into this directory.\n"
            "Then run this program to generate the equivalent Raw Accel settings.\n";
    }
  
    std::cout << "Press any key to close this window . . .\n";
    _getwch();
}
