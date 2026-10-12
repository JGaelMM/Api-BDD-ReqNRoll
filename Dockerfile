#compilar y publicar la API con el SDK de .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Se restauran los paquetes antes de copiar el código para aprovechar la caché de Docker
COPY src/CategoriesProducts.Api/CategoriesProducts.Api.csproj src/CategoriesProducts.Api/
RUN dotnet restore src/CategoriesProducts.Api/CategoriesProducts.Api.csproj

COPY src/ src/
RUN dotnet publish src/CategoriesProducts.Api/CategoriesProducts.Api.csproj -c Release -o /app/publish --no-restore

# imagen final solo con el runtime de ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "CategoriesProducts.Api.dll"]
