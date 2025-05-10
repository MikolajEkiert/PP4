FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BookStoreService/BookStoreService.csproj", "BookStoreService/"]
COPY ["BookStore_Application/BookStore_Application.csproj", "BookStore_Application/"]
COPY ["BookStore_Domain/BookStore_Domain.csproj", "BookStore_Domain/"]
RUN dotnet restore "BookStoreService/BookStoreService.csproj"
COPY . .
WORKDIR "/src/BookStoreService"
RUN dotnet build "BookStoreService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStoreService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookStoreService.dll"] 