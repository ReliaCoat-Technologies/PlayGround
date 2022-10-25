#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base

# set working directory
WORKDIR /app

# install mongodb tools (for mongodump)
RUN apt update
RUN apt install -y wget
RUN wget https://fastdl.mongodb.org/tools/db/mongodb-database-tools-debian11-x86_64-100.6.0.deb
RUN apt install ./mongodb-database-tools-debian11-x86_64-100.6.0.deb
RUN rm ./mongodb-database-tools-debian11-x86_64-100.6.0.deb

RUN chmod +x /usr/bin/mongodump
RUN chown root:root /usr/bin/mongodump

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
ENTRYPOINT ["dotnet", "MongoBackupDocker.dll"]