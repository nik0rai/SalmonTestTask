echo off
echo "****** DB INIT ***************"
docker compose stop task-web
docker exec -it task-postgres mkdir /tmp/dbrestore  || (echo Error & goto :end)
docker cp data/. task-postgres:/tmp/dbrestore  || (echo Error & goto:end)
docker exec -it task-postgres psql --host=localhost --port=5432 --username=postgres --file=/tmp/dbrestore/create_db.sql  || (echo Error & goto:end)
docker exec -it task-postgres pg_restore --host localhost --port 5432 --username=creatio_user --dbname=creatio --no-owner --no-privileges --verbose /tmp/dbrestore/db.backup  || (echo Error & goto:end)
docker exec -it task-postgres psql --host=localhost --port=5432 --username=creatio_user --dbname=creatio --file=/tmp/dbrestore/create_types.sql  || (echo Error & goto:end)
docker exec -it task-postgres psql --host=localhost --port=5432 --username=creatio_user --dbname=creatio --file=/tmp/dbrestore/update_settings.sql  || (echo Error & goto:end)
docker compose start task-web
echo "****** DB RESTORED ***********"
:end
pause