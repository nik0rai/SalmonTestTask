docker compose build
docker compose up -d  || (echo Error & goto:end)
echo "****** CONTAINERS INSTALLED ***********"
:end
pause