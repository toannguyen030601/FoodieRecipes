FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy .csproj files trước để tận dụng Docker layer caching
COPY ["FoodieHub.API.csproj", "./"]
# Copy các project dependencies nếu có
# COPY ["../FoodieHub.Data/FoodieHub.Data.csproj", "FoodieHub.Data/"]

# Restore packages
RUN dotnet restore "FoodieHub.API.csproj"

# Copy toàn bộ source code (bao gồm cả wwwroot)
COPY . .

# Build project
RUN dotnet build "FoodieHub.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodieHub.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodieHub.API.dll"]
