FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sửa đường dẫn vì build từ root
COPY ["FoodieHub.API/FoodieHub.API.csproj", "FoodieHub.API/"]

# Restore packages
RUN dotnet restore "FoodieHub.API/FoodieHub.API.csproj"

# Copy toàn bộ source code của API
COPY FoodieHub.API/ FoodieHub.API/
WORKDIR /src/FoodieHub.API

# Build project
RUN dotnet build "FoodieHub.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodieHub.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodieHub.API.dll"]
