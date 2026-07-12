FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos del proyecto y restaurar dependencias
COPY ["Notificaciones.Service/Notificaciones.Service.csproj", "Notificaciones.Service/"]
COPY ["Shared.Contracts/Shared.Contracts.csproj", "Shared.Contracts/"]
RUN dotnet restore "Notificaciones.Service/Notificaciones.Service.csproj"

# Copiar el resto de archivos y compilar
COPY . .
WORKDIR "/src/Notificaciones.Service"
RUN dotnet build "Notificaciones.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notificaciones.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final (sin exponer puertos, es un background worker)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notificaciones.Service.dll"]
