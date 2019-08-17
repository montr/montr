FROM node:12-alpine AS node
WORKDIR /ui
COPY ./src/ui .
RUN npm install && npm run build-prod

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS build
WORKDIR /src
COPY ./src .
WORKDIR /src/Host
# RUN dotnet build "Host.csproj" --configuration Release --output /app

FROM build AS publish
RUN dotnet publish "Host.csproj" --configuration Release --output /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=node /Host/wwwroot/assets ./wwwroot/assets
ENTRYPOINT ["dotnet", "Host.dll"]
