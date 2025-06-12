FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy .csproj file (build context từ trong folder FoodieHub.MVC)
COPY ["FoodieHub.MVC.csproj", "./"]

# Nếu có reference đến project khác, uncomment và sửa đường dẫn
# COPY ["../FoodieHub.Data/FoodieHub.Data.csproj", "FoodieHub.Data/"]

# Restore packages
RUN dotnet restore "FoodieHub.MVC.csproj"

# Copy toàn bộ source code
COPY . .

# Build project
RUN dotnet build "FoodieHub.MVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodieHub.MVC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodieHub.MVC.dll"]
