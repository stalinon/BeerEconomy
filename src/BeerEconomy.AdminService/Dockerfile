FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BeerEconomy.AdminService/BeerEconomy.AdminService.csproj", "src/BeerEconomy.AdminService/"]
COPY ["src/BeerEconomy.Common/BeerEconomy.Common.csproj", "src/BeerEconomy.Common/"]
RUN dotnet restore "src/BeerEconomy.AdminService/BeerEconomy.AdminService.csproj"
COPY . .
WORKDIR "/src/src/BeerEconomy.AdminService"
RUN dotnet build "BeerEconomy.AdminService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BeerEconomy.AdminService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeerEconomy.AdminService.dll"]
