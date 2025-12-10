# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY RatingService.Api/*.csproj ./RatingService.Api/
COPY RatingService.Application/*.csproj ./RatingService.Application/
COPY AsyncApiBindingsGenerator/*.csproj ./AsyncApiBindingsGenerator/
COPY AsyncApiBindingsGenerator.UnitTests/*.csproj ./AsyncApiBindingsGenerator.UnitTests/
COPY TestLibrary/*.csproj ./TestLibrary/
COPY Service.Api.Common/*.csproj ./Service.Api.Common/
COPY Service.Application.Common/*.csproj ./Service.Application.Common/
RUN dotnet restore

# copy everything else and build app
COPY RatingService.Api/. ./RatingService.Api/
COPY RatingService.Application/. ./RatingService.Application/
COPY AsyncApiBindingsGenerator/. ./AsyncApiBindingsGenerator/
COPY AsyncApiBindingsGenerator.UnitTests/. ./AsyncApiBindingsGenerator.UnitTests/
COPY TestLibrary/. ./TestLibrary/
COPY Service.Api.Common/. ./Service.Api.Common/
COPY Service.Application.Common/. ./Service.Application.Common/
WORKDIR /source/RatingService.Api
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app ./

# EXPOSE 8080

ENTRYPOINT ["dotnet", "RatingService.Api.dll"]