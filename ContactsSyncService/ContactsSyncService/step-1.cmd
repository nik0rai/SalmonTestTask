echo off
echo "********** BUILDING PROJECT ***************"
docker compose up -d  || (echo Error & goto :end)
echo "**************** DONE *********************"
:end
pause