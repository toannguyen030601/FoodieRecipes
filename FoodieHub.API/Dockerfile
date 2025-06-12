FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FoodieHub.API.csproj", "./"]
RUN dotnet restore "FoodieHub.API.csproj"
COPY . .
RUN dotnet build "FoodieHub.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodieHub.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FoodieHub.API.dll"]
