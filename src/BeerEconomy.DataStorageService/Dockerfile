FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BeerEconomy.DataStorageService/BeerEconomy.DataStorageService.csproj", "src/BeerEconomy.DataStorageService/"]
COPY ["src/BeerEconomy.Common/BeerEconomy.Common.csproj", "src/BeerEconomy.Common/"]
RUN dotnet restore "src/BeerEconomy.DataStorageService/BeerEconomy.DataStorageService.csproj"
COPY . .
WORKDIR "/src/src/BeerEconomy.DataStorageService"
RUN dotnet build "BeerEconomy.DataStorageService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BeerEconomy.DataStorageService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeerEconomy.DataStorageService.dll"]
