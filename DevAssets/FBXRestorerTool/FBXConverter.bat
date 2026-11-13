@echo off
setlocal
set SCRIPT_DIR=%~dp0
"C:\Program Files\Blender Foundation\Blender 5.1\blender.exe" -b -P "%SCRIPT_DIR%MassFBXConverter.py"
endlocal
