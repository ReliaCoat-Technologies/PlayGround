#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LinuxDockerContainerExecute/LinuxDockerContainerExecute.csproj", "LinuxDockerContainerExecute/"]
RUN dotnet restore "LinuxDockerContainerExecute/LinuxDockerContainerExecute.csproj"
COPY . .
WORKDIR "/src/LinuxDockerContainerExecute"
RUN dotnet build "LinuxDockerContainerExecute.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LinuxDockerContainerExecute.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinuxDockerContainerExecute.dll"]