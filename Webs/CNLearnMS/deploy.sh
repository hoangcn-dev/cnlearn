#!/bin/bash

# --- Configuration ---
SERVER_USER="root"
SERVER_HOST="hoangcn.com"
SERVER_PATH="/hoangcn/cnlearnms"

echo "1. Build Vue 3 app locally (Vite)..."
npm run build-only
if [ $? -ne 0 ]; then
    echo "Local build failed."
    exit 1
fi

echo "2. Ensure target directory exists on VPS..."
ssh "${SERVER_USER}@${SERVER_HOST}" "mkdir -p ${SERVER_PATH}"

echo "3. Upload compiled static files to VPS..."
scp -r dist/* "${SERVER_USER}@${SERVER_HOST}:${SERVER_PATH}/"
if [ $? -ne 0 ]; then
    echo "Upload failed."
    exit 1
fi

echo "4. Set permissions for Nginx reader..."
ssh "${SERVER_USER}@${SERVER_HOST}" "chmod -R 755 ${SERVER_PATH}"

echo "DONE!"
