FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos del proyecto y restaurar dependencias
COPY ["Catalogo.Service/Catalogo.Service.csproj", "Catalogo.Service/"]
RUN dotnet restore "Catalogo.Service/Catalogo.Service.csproj"

# Copiar el resto de archivos y compilar
COPY . .
WORKDIR "/src/Catalogo.Service"
RUN dotnet build "Catalogo.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Catalogo.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Catalogo.Service.dll"]
