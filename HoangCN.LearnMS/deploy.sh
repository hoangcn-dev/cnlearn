#!/bin/bash

# --- Configuration ---
SERVER_USER="root"
SERVER_HOST="hoangcn.com"
SERVER_PATH="/hoangcn/learnms"
COMPOSE_FILE="docker-compose.yaml"

echo "1. remove old image"
docker rmi learnms

echo "2. build new image"
docker build -t learnms -f Dockerfile ..
if [ $? -ne 0 ]; then
    echo "Docker build failed."
    exit 1
fi

echo "3. saving .tar"
IMAGE="learnms:latest"
TAR="new.tar"

docker save -o "$TAR" "$IMAGE"
if [ $? -ne 0 ]; then
    echo "ERROR: failed to save $IMAGE"
    exit 1
fi

echo "4. upload tar and config files"
scp "$TAR" "$COMPOSE_FILE" run.sh stop.sh "${SERVER_USER}@${SERVER_HOST}:${SERVER_PATH}/"
if [ $? -ne 0 ]; then
    echo "SCP of files failed"
    exit 1
fi

echo "5. ssh and run"
ssh "${SERVER_USER}@${SERVER_HOST}" "cd ${SERVER_PATH} && chmod +x *.sh && ./run.sh"

echo "DONE"
