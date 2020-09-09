FROM node:14-alpine AS node
WORKDIR /ui
COPY ./src/ui .
RUN npm install && npm run build-prod

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ./src .
COPY ./database /database
WORKDIR /src/Host
# RUN dotnet build "Host.csproj" --configuration Release --output /app

FROM build AS publish
RUN dotnet publish "Host.csproj" --configuration Release --output /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.7-alpine AS runtime
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /database ./database
COPY --from=node /ui/assets ./wwwroot/assets
ENTRYPOINT ["dotnet", "Host.dll"]
