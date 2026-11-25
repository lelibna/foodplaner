FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY shared/foodplaner.Shared/foodplaner.Shared.csproj shared/foodplaner.Shared/
COPY backend/foodplaner.Api/foodplaner.Api.csproj backend/foodplaner.Api/
RUN dotnet restore backend/foodplaner.Api/foodplaner.Api.csproj

COPY shared/ shared/
COPY backend/ backend/
RUN dotnet publish backend/foodplaner.Api/foodplaner.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "foodplaner.Api.dll"]