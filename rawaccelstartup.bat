@echo off

set path=%~dp0
echo @echo off > "%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\rawaccel.bat"
echo cd "%path%" >> "%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\rawaccel.bat"
echo rawaccel.exe >> "%USERPROFILE%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\rawaccel.bat"
