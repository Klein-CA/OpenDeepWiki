﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/KoalaWiki/KoalaWiki.csproj", "src/KoalaWiki/"]
RUN dotnet restore "src/KoalaWiki/KoalaWiki.csproj"
COPY . .
WORKDIR "/src/src/KoalaWiki"
RUN dotnet build "./KoalaWiki.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KoalaWiki.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KoalaWiki.dll"]

