#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WBSA.CurrencyExchangeApp.API/WBSA.CurrencyExchangeApp.API.csproj", "WBSA.CurrencyExchangeApp.API/"]
COPY ["WBSA.CurrencyExchangeApp.Data/WBSA.CurrencyExchangeApp.Data.csproj", "WBSA.CurrencyExchangeApp.Data/"]
COPY ["WBSA.CurrencyExchangeApp.Services/WBSA.CurrencyExchangeApp.Services.csproj", "WBSA.CurrencyExchangeApp.Services/"]
RUN dotnet restore "./WBSA.CurrencyExchangeApp.API/./WBSA.CurrencyExchangeApp.API.csproj"
COPY . .
WORKDIR "/src/WBSA.CurrencyExchangeApp.API"
RUN dotnet build "./WBSA.CurrencyExchangeApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WBSA.CurrencyExchangeApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WBSA.CurrencyExchangeApp.API.dll"]