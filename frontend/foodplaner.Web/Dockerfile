FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY shared/foodplaner.Shared/foodplaner.Shared.csproj shared/foodplaner.Shared/
COPY frontend/foodplaner.Web/foodplaner.Web.csproj frontend/foodplaner.Web/
RUN dotnet restore frontend/foodplaner.Web/foodplaner.Web.csproj

COPY shared/ shared/
COPY frontend/ frontend/
RUN dotnet publish frontend/foodplaner.Web/foodplaner.Web.csproj -c Release -o /app/publish --no-restore

FROM nginx:alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY frontend/foodplaner.Web/nginx.conf /etc/nginx/conf.d/default.conf