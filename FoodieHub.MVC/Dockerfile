FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy .csproj files trước để tận dụng Docker layer caching
COPY ["FoodieHub.MVC.csproj", "FoodieHub.MVC/"]
# Copy các project dependencies nếu có
# COPY ["../FoodieHub.Data/FoodieHub.Data.csproj", "FoodieHub.Data/"]

# Restore packages
RUN dotnet restore "FoodieHub.MVC/FoodieHub.MVC.csproj"

# Copy toàn bộ source code từ thư mục cha
COPY ../ .

# Build project
WORKDIR "/src/FoodieHub.MVC"
RUN dotnet build "FoodieHub.MVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodieHub.MVC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodieHub.MVC.dll"]
