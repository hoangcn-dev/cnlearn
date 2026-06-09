@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

rem --- Configuration ---
set "SERVER_USER=root"
set "SERVER_HOST=hoangcn.com"
set "SERVER_PATH=/hoangcn/cnadmin"

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
rem Upload toan bo noi dung trong thu muc dist cuc bo vao /hoangcn/cnadmin tren VPS
scp -r dist/* %SERVER_USER%@%SERVER_HOST%:%SERVER_PATH%/
if %ERRORLEVEL% neq 0 (
    echo Upload failed.
    pause
    exit /b 1
)

echo 4. Set permissions for Nginx reader...
rem Dam bao Nginx co du quyen doc thu muc nay de tranh loi 403 Forbidden
ssh %SERVER_USER%@%SERVER_HOST% "chmod -R 755 %SERVER_PATH%"

echo DONE!
pause
