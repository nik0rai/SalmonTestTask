FROM creatio/creatio:7.18.5.1501_Studio_Linux_Softkey_PostgreSQL_ENU AS base
EXPOSE 5000 5002 5008

WORKDIR /app
COPY . ./
FROM base AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT Development
ENV TZ Europe/Moscow
ENV COMPlus_ThreadPool_ForceMinWorkerThreads 100

ENTRYPOINT ["dotnet", "Terrasoft.WebHost.dll"]