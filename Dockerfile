# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY "*.sln" "./"
COPY "BookEShop/*.csproj" "./BookEShop/"
COPY "BookEShop.Application/*.csproj" "./BookEShop.Application/"
COPY "BookEShop.Domain/*.csproj" "./BookEShop.Domain/"
COPY "BookEShop.Test/*.csproj" "./BookEShop.Test/"
COPY "BookEShop.OrderService/*.csproj" "./BookEShop.OrderService/"
COPY "BookEShop.CatalogService/*.csproj" "./BookEShop.CatalogService/"
COPY "BookEShop.CartService/*.csproj" "./BookEShop.CartService/"
COPY "BookEShop.AuthService/*.csproj" "./BookEShop.AuthService/"
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "BookEShop.dll"]