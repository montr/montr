FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ./src .
# RUN dotnet restore "Montr.sln"
WORKDIR /src/Idx
RUN dotnet build "Idx.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Idx.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Idx.dll"]
