#!/bin/bash
docker load -i new.tar
docker compose down
docker compose up -d
