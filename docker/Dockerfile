# docker build -t sql-dev
# docker container run --rm --name sql -d -p 1433:1433 sql-dev
FROM mcr.microsoft.com/mssql/server:2019-GA-ubuntu-16.04

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=!123Senha
ENV MSSQL_PID=Express

EXPOSE 1433
