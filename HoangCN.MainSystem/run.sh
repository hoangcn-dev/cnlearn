docker compose down
docker rmi main_system
docker load -i new.tar
docker compose up -d
