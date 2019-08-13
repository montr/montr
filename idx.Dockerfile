FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine AS base
WORKDIR /app
EXPOSE 5050

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS build
WORKDIR /src
COPY ./src .
WORKDIR /src/Idx
# RUN dotnet build "Idx.csproj" --configuration Release --output /app

FROM build AS publish
RUN dotnet publish "Idx.csproj" --configuration Release --output /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Idx.dll"]
