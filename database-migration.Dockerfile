# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /source

# create local nuget packages
COPY nuget.config ./
COPY Service.Abstractions/ ./Service.Abstractions/

RUN mkdir local-nuget-feed

RUN dotnet pack -c release -o ./local-nuget-feed ./Service.Abstractions/

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# copy csproj and restore as distinct layers
COPY RatingService.Domain/*.csproj ./RatingService.Domain/
COPY RatingService.Infrastructure/*.csproj ./RatingService.Infrastructure/
COPY RatingService.Infrastructure.DesignTime/*.csproj ./RatingService.Infrastructure.DesignTime/
RUN dotnet restore RatingService.Domain/
RUN dotnet restore RatingService.Infrastructure/
RUN dotnet restore RatingService.Infrastructure.DesignTime/

# copy everything else and build app?
COPY RatingService.Domain/. ./RatingService.Domain/
COPY RatingService.Infrastructure/. ./RatingService.Infrastructure/
COPY RatingService.Infrastructure.DesignTime/. ./RatingService.Infrastructure.DesignTime/

ENTRYPOINT ["dotnet", "ef", "database", "update", \
    "-p", "./RatingService.Infrastructure/", \
    "--startup-project", "./RatingService.Infrastructure.DesignTime/", \
    "--context", "RatingContext"]