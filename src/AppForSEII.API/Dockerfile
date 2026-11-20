
# Etapa 1: compilación de la API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos el proyecto y restauramos dependencias
COPY src/AppForSEII.API/AppForSEII.API.csproj ./AppForSEII.API/
RUN dotnet restore ./AppForSEII.API/AppForSEII.API.csproj

# Copiamos el código de la API
COPY src/AppForSEII.API/ ./AppForSEII.API/

# Publicamos la aplicación
RUN dotnet publish ./AppForSEII.API/AppForSEII.API.csproj -c Release -o /app/publish --no-restore

# Etapa 2: ejecución de la API
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

# Puerto HTTP dentro del contenedor
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AppForSEII.API.dll"]
