# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY ShopApp.slnx ./
COPY Directory.Build.props ./
COPY src/ShopApp.Domain/ShopApp.Domain.csproj ./src/ShopApp.Domain/
COPY src/ShopApp.Application/ShopApp.Application.csproj ./src/ShopApp.Application/
COPY src/ShopApp.Infrastructure/ShopApp.Infrastructure.csproj ./src/ShopApp.Infrastructure/
COPY src/ShopApp.API/ShopApp.API.csproj ./src/ShopApp.API/

RUN dotnet restore src/ShopApp.API/ShopApp.API.csproj

COPY src/ ./src/

RUN dotnet publish src/ShopApp.API/ShopApp.API.csproj -c Release -o /publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ShopApp.API.dll"]
