@echo off
cd /d "%~dp0"

set "PYTHON_EXE=python"
py -3 --version >nul 2>&1
IF NOT ERRORLEVEL 1 (
    set "PYTHON_EXE=py -3"
)

echo Checking Python installation...
%PYTHON_EXE% --version >nul 2>&1
IF ERRORLEVEL 1 (
    echo Python is not installed or not in PATH.
    echo Please install Python 3.10+ from https://www.python.org/
    pause
    exit /b
)

IF DEFINED TRANSLATION_DIR (
    echo Using translations folder override: %TRANSLATION_DIR%
) ELSE (
    echo Translation folder will be auto-discovered relative to this script.
)

echo Updating pip...
%PYTHON_EXE% -m pip install --upgrade pip

echo Checking and installing required packages...

%PYTHON_EXE% -m pip show torch >nul 2>&1 || %PYTHON_EXE% -m pip install torch --index-url https://download.pytorch.org/whl/cpu

%PYTHON_EXE% -m pip show transformers >nul 2>&1 || %PYTHON_EXE% -m pip install transformers
%PYTHON_EXE% -m pip show tqdm >nul 2>&1 || %PYTHON_EXE% -m pip install tqdm
%PYTHON_EXE% -m pip show sentencepiece >nul 2>&1 || %PYTHON_EXE% -m pip install sentencepiece
%PYTHON_EXE% -m pip show sacremoses >nul 2>&1 || %PYTHON_EXE% -m pip install sacremoses
%PYTHON_EXE% -m pip show protobuf >nul 2>&1 || %PYTHON_EXE% -m pip install protobuf
%PYTHON_EXE% -m pip show safetensors >nul 2>&1 || %PYTHON_EXE% -m pip install safetensors
%PYTHON_EXE% -m pip show huggingface_hub >nul 2>&1 || %PYTHON_EXE% -m pip install huggingface_hub
%PYTHON_EXE% -m pip show hf-xet >nul 2>&1 || %PYTHON_EXE% -m pip install hf-xet

echo Running translation script...
IF DEFINED TRANSLATION_DIR (
    %PYTHON_EXE% translate.py --output-dir "%TRANSLATION_DIR%" --quality auto
) ELSE (
    %PYTHON_EXE% translate.py --quality auto
)

echo.
pause
