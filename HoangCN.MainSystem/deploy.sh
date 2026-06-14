#!/bin/bash

# --- Configuration ---
SERVER_USER="root"
SERVER_HOST="hoangcn.com"
SERVER_PATH="/hoangcn/main_system"
COMPOSE_FILE="docker-compose.yaml"

echo "1. remove old image"
docker rmi main_system

echo "2. build new image"
docker build -t main_system -f Dockerfile ..
if [ $? -ne 0 ]; then
    echo "Docker build failed."
    exit 1
fi

echo "3. saving .tar"
IMAGE="main_system:latest"
TAR="new.tar"

docker save -o "$TAR" "$IMAGE"
if [ $? -ne 0 ]; then
    echo "ERROR: failed to save $IMAGE"
    echo "Listing recent images for debugging:"
    docker images --format "table {{.Repository}}:{{.Tag}}\t{{.ID}}\t{{.Size}}"
    exit 1
fi

echo "4. upload tar and config files"
scp "$TAR" "$COMPOSE_FILE" run.sh stop.sh commit.sh "${SERVER_USER}@${SERVER_HOST}:${SERVER_PATH}/"
if [ $? -ne 0 ]; then
    echo "SCP of files failed"
    exit 1
fi

echo "5. ssh and run"
ssh "${SERVER_USER}@${SERVER_HOST}" "cd ${SERVER_PATH} && chmod +x *.sh && ./run.sh"

echo "DONE"
