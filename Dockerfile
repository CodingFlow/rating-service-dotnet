# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /source

# create local nuget packages
COPY nuget.config ./
COPY Service.Application.Common/. ./Service.Application.Common/
COPY Service.Api.Common/. ./Service.Api.Common/
COPY AsyncApiBindingsGenerator/. ./AsyncApiBindingsGenerator/
RUN dotnet publish -c release -o ./local-nuget-feed ./Service.Application.Common/
COPY local-nuget-feed/. ./local-nuget-feed/
RUN dotnet publish -c release -o ./local-nuget-feed ./Service.Api.Common/
COPY local-nuget-feed/. ./local-nuget-feed/
RUN dotnet publish -c release -o ./local-nuget-feed ./AsyncApiBindingsGenerator/
COPY local-nuget-feed/. ./local-nuget-feed/

# copy csproj and restore as distinct layers
COPY *.sln .
COPY RatingService.Api/*.csproj ./RatingService.Api/
COPY RatingService.Application/*.csproj ./RatingService.Application/
RUN dotnet restore

# copy everything else and build app
COPY RatingService.Api/. ./RatingService.Api/
COPY RatingService.Application/. ./RatingService.Application/
WORKDIR /source/RatingService.Api
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "RatingService.Api.dll"]