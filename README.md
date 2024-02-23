# PeakVentures User Track Services

Below are the services responsible for tracking users

## PixelService

This service is responsible for capturing user information such as UserAgent and IP Address.
When capturing this information, it is sent via message streaming to a Kafka topic.

Third-party services:
- Confluent Kafka

Building and running the docker file:
```bash
docker pull mcr.microsoft.com/dotnet/aspnet:8.0
docker build -f ./src/PixelService/PixelService.Api/Dockerfile -t pixelservice-api ./src/PixelService/
docker run -d -t -i \
-p 80:80 \
--name pixelservice-api pixelservice-api
```

## StorageService

This service is responsible for receiving streaming messages and persisting this information in a log file.

Building and running the docker file:
```bash
docker pull mcr.microsoft.com/dotnet/aspnet:8.0
docker build -f ./src/StorageService/StorageService.Worker/Dockerfile -t storageservice-worker ./src/StorageService
docker run -d -t -i \
-e Storage__LogFilePath='/tmp/visits.log' \
--name storageservice-worker storageservice-worker
```