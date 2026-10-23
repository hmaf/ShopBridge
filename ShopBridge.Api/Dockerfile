#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ShopBridge.Api/ShopBridge.Api.csproj", "ShopBridge.Api/"]
COPY ["ShopBridge.Application/ShopBridge.Application.csproj", "ShopBridge.Application/"]
COPY ["ShopBridge.Domain/ShopBridge.Domain.csproj", "ShopBridge.Domain/"]
COPY ["ShopBridge.Infra/ShopBridge.Infra.csproj", "ShopBridge.Infra/"]
RUN dotnet restore "./ShopBridge.Api/ShopBridge.Api.csproj"
COPY . .
WORKDIR "/src/ShopBridge.Api"
RUN dotnet build "./ShopBridge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShopBridge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShopBridge.Api.dll"]