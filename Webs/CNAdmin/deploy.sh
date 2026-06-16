#!/bin/bash

# --- Configuration ---
SERVER_USER="root"
SERVER_HOST="hoangcn.com"
SERVER_PATH="/hoangcn/cnadmin"

echo "1. Build Vue 3 app locally (Vite)..."
npm run build-only
if [ $? -ne 0 ]; then
    echo "Local build failed."
    exit 1
fi

echo "2. Ensure target directory exists on VPS..."
ssh "${SERVER_USER}@${SERVER_HOST}" "mkdir -p ${SERVER_PATH}"

echo "3. Upload compiled static files to VPS..."
# Upload toan bo noi dung trong thu muc dist cuc bo vao /hoangcn/cnadmin tren VPS
scp -r dist/* "${SERVER_USER}@${SERVER_HOST}:${SERVER_PATH}/"
if [ $? -ne 0 ]; then
    echo "Upload failed."
    exit 1
fi

echo "4. Set permissions for Nginx reader..."
# Dam bao Nginx co du quyen doc thu muc nay de tranh loi 403 Forbidden
ssh "${SERVER_USER}@${SERVER_HOST}" "chmod -R 755 ${SERVER_PATH}"

echo "DONE!"
