
# Etapa 1: compilar la aplicación con .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos el proyecto y restauramos dependencias
COPY src/AppForSEII.LogViewer/AppForSEII.LogViewer.csproj ./AppForSEII.LogViewer/
RUN dotnet restore ./AppForSEII.LogViewer/AppForSEII.LogViewer.csproj

# Copiamos el código y la configuración
COPY src/AppForSEII.LogViewer/*.cs ./AppForSEII.LogViewer/
COPY src/AppForSEII.LogViewer/appsettings.json ./AppForSEII.LogViewer/

# Publicamos la aplicación
RUN dotnet publish ./AppForSEII.LogViewer/AppForSEII.LogViewer.csproj -c Release -o /app/publish --no-restore

# Etapa 2: imagen para ejecutar LogViewer
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AppForSEII.LogViewer.dll"]
