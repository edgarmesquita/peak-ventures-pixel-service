FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StorageService.Worker/StorageService.Worker.csproj", "StorageService.Worker/"]
RUN dotnet restore "StorageService.Worker/StorageService.Worker.csproj"
COPY . .
WORKDIR "/src/StorageService.Worker"
RUN dotnet build "StorageService.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StorageService.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PeakVentures.UserTrack.StorageService.Worker.dll"]
