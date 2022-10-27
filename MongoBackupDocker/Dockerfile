#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-jammy-amd64 AS base

# MongoDB Properties
ENV MONGO_HOST_ADDRESS "127.0.0.1"
ENV MONGO_HOST_PORT "27017"
ENV MONGO_DBNAME "admin"

# File System Properties
ENV SFTP_HOST_ADDRESS "127.0.0.1"
ENV SFTP_HOST_PORT "22"
ENV SFTP_USERNAME "admin"
ENV SFTP_PASSWORD "password"
ENV FILE_SYSTEM_ROOT "/files"

# Default cron schedule - 2:00 AM every Tuesday and Thursday.
ENV CRON_SCHEDULE "0 2 * * 2,4"

# install mongodb tools (for mongodump)
RUN apt update
RUN apt install -y wget

# Install mongodb tools.
RUN wget https://fastdl.mongodb.org/tools/db/mongodb-database-tools-debian11-x86_64-100.6.0.deb
RUN apt install ./mongodb-database-tools-debian11-x86_64-100.6.0.deb
RUN rm ./mongodb-database-tools-debian11-x86_64-100.6.0.deb

# Install the task scheduler.
RUN apt install -y cron

# Microsoft-built dockerfile operations.
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["MongoBackupDocker/MongoBackupDocker.csproj", "MongoBackupDocker/"]
RUN dotnet restore "MongoBackupDocker/MongoBackupDocker.csproj"
COPY . .
WORKDIR "/src/MongoBackupDocker"
RUN dotnet build "MongoBackupDocker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MongoBackupDocker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT "/app/ShellScripts/init.sh"
#ENTRYPOINT ["dotnet", "MongoBackupDocker.dll"]