@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

rem --- Configuration ---
set "SERVER_USER=root"
set "SERVER_HOST=hoangcn.com"
set "SERVER_PATH=/hoangcn/cnlearnms"

echo 1. Build Vue 3 app locally (Vite)...
call npm run build-only
if %ERRORLEVEL% neq 0 (
    echo Local build failed.
    pause
    exit /b 1
)

echo 2. Ensure target directory exists on VPS...
ssh %SERVER_USER%@%SERVER_HOST% "mkdir -p %SERVER_PATH%"

echo 3. Upload compiled static files to VPS...
scp -r dist/* %SERVER_USER%@%SERVER_HOST%:%SERVER_PATH%/
if %ERRORLEVEL% neq 0 (
    echo Upload failed.
    pause
    exit /b 1
)

echo 4. Set permissions for Nginx reader...
ssh %SERVER_USER%@%SERVER_HOST% "chmod -R 755 %SERVER_PATH%"

echo DONE!
pause
