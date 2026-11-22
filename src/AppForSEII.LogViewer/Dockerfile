
# Etapa 1: compilar la aplicación con .NET 10
# Esta imagen contiene las herramientas necesarias para restaurar dependencias y compilar el proyecto.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build# Establecemos /src como directorio de trabajo dentro del contenedor.

# Establecemos /src como directorio de trabajo dentro del contenedor.
WORKDIR /src

# Copiamos el proyecto (.csproj) al contenedor y restauramos dependencias
COPY src/AppForSEII.LogViewer/AppForSEII.LogViewer.csproj ./AppForSEII.LogViewer/
RUN dotnet restore ./AppForSEII.LogViewer/AppForSEII.LogViewer.csproj

# Copiamos los archivos de código C# (.cs) al contenedor.
COPY src/AppForSEII.LogViewer/*.cs ./AppForSEII.LogViewer/
# Copiamos el archivo de configuración de la aplicación.
COPY src/AppForSEII.LogViewer/appsettings.json ./AppForSEII.LogViewer/

# Compilamos y publicamos la aplicación en modo Release.
# El resultado se guarda en /app/publish.
# --no-restore evita restaurar de nuevo las dependencias porque ya lo hicimos antes.RUN dotnet publish ./AppForSEII.LogViewer/AppForSEII.LogViewer.csproj -c Release -o /app/publish --no-restore
RUN dotnet publish ./AppForSEII.LogViewer/AppForSEII.LogViewer.csproj -c Release -o /app/publish --no-restore


# ETAPA 2: Creamos la imagen final utilizando únicamente el entorno de ejecución de .NET 10.
# No necesitamos el SDK porque la aplicación ya está compilada.FROM mcr.microsoft.com/dotnet/runtime:10.0
FROM mcr.microsoft.com/dotnet/runtime:10.0

# Establecemos /app como directorio de trabajo de la imagen final.
WORKDIR /app

# Copiamos los archivos publicados en la primera etapa a la imagen final.
# --from=build indica que los archivos proceden de la etapa llamada build.
COPY --from=build /app/publish .

# Indicamos el comando que se ejecutará al iniciar el contenedor.
# Ejecuta la aplicación LogViewer mediante el entorno de .NET.
ENTRYPOINT ["dotnet", "AppForSEII.LogViewer.dll"]
