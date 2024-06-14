echo off
echo "********** CONNECTING CONTAINERS ***************"
docker network create mergedNetwork  || (echo Error & goto :end)
docker network connect mergedNetwork contacts-sync-service  || (echo Error & goto :end)
docker network connect mergedNetwork task-postgres  || (echo Error & goto :end)
docker inspect mergedNetwork
echo "**************** CONNECTED *********************"
:end
pause