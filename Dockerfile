# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /source

# create local nuget packages
COPY nuget.config ./
COPY Service.Abstractions/ ./Service.Abstractions/
COPY Service.Libraries.Redis/ ./Service.Libraries.Redis/
COPY Service.AppHost.Common/ ./Service.AppHost.Common/
COPY Service.Application.Common/. ./Service.Application.Common/
COPY Service.Api.Common/. ./Service.Api.Common/
COPY AsyncApiBindingsGenerator/. ./AsyncApiBindingsGenerator/
COPY AsyncApiApplicationSupportGenerator/. ./AsyncApiApplicationSupportGenerator/

RUN mkdir local-nuget-feed

RUN dotnet pack -c release -o ./local-nuget-feed ./Service.Abstractions/
RUN dotnet pack -c release -o ./local-nuget-feed ./Service.Libraries.Redis/
RUN dotnet pack -c release -o ./local-nuget-feed ./Service.AppHost.Common/
RUN dotnet pack -c release -o ./local-nuget-feed ./Service.Application.Common/
RUN dotnet pack -c release -o ./local-nuget-feed ./Service.Api.Common/
RUN dotnet pack -c release -o ./local-nuget-feed ./AsyncApiBindingsGenerator/
RUN dotnet pack -c release -o ./local-nuget-feed ./AsyncApiApplicationSupportGenerator/

# copy csproj and restore as distinct layers
COPY *.slnx .
COPY RatingService.AppHost/*.csproj ./RatingService.AppHost/
COPY RatingService.Api/*.csproj ./RatingService.Api/
COPY RatingService.Application/*.csproj ./RatingService.Application/
COPY RatingService.Infrastructure/*.csproj ./RatingService.Infrastructure/
COPY RatingService.Infrastructure.DesignTime/*.csproj ./RatingService.Infrastructure.DesignTime/
COPY RatingService.Domain/*.csproj ./RatingService.Domain/
RUN dotnet restore

# copy everything else and build app
COPY RatingService.AppHost/. ./RatingService.AppHost/
COPY RatingService.Api/. ./RatingService.Api/
COPY RatingService.Application/. ./RatingService.Application/
COPY RatingService.Infrastructure/. ./RatingService.Infrastructure/
COPY RatingService.Domain/. ./RatingService.Domain/
WORKDIR /source/RatingService.AppHost
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "RatingService.AppHost.dll"]