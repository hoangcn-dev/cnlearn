docker compose down
docker rmi cnadmin_web
docker load -i cnadmin.tar
docker compose up -d
