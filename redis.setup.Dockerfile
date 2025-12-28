# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /source

# copy csproj and restore as distinct layers
COPY RatingService.Redis.Setup/*.slnx .
COPY RatingService.Redis.Setup/RatingService.Redis.Setup/*.csproj ./RatingService.Redis.Setup/
RUN dotnet restore

# copy everything else and build app
COPY RatingService.Redis.Setup/RatingService.Redis.Setup/. ./RatingService.Redis.Setup/
WORKDIR /source/RatingService.Redis.Setup
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "RatingService.Redis.Setup.dll"]