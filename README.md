## rawaccel
rawaccel is a mouse acceleration driver for Windows 10 (64-bit)

### build
[Install the WDK for Windows 10](https://docs.microsoft.com/en-us/windows-hardware/drivers/download-the-wdk)

### install
From an elevated prompt run `bcdedit /set testsigning on`, then run *installer.exe* and reboot.

### usage
*rawaccel.exe* is a console program that will apply your accel settings, for usage and options see `rawaccel help`.

[precompiled test-signed binary available here](https://github.com/a1xd/rawaccel/releases)
