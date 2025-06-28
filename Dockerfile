#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Imagen base 
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
# USER app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Imagen para build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia el archivo del proyecto y restaura las dependencias
COPY ["NutriCheck.Backend.csproj", "./"]
RUN dotnet restore "NutriCheck.Backend.csproj"

# Copia el resto del código fuente
COPY . .

# Compilar el proyecto
RUN dotnet build "NutriCheck.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar para producción
FROM build AS publish
RUN dotnet publish "NutriCheck.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "NutriCheck.Backend.dll"]