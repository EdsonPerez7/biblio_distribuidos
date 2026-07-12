FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos del proyecto y restaurar dependencias
COPY ["NeoLibro.WebAPI/NeoLibro.WebAPI.csproj", "NeoLibro.WebAPI/"]
COPY ["Shared.Contracts/Shared.Contracts.csproj", "Shared.Contracts/"]
RUN dotnet restore "NeoLibro.WebAPI/NeoLibro.WebAPI.csproj"

# Copiar el resto de archivos y compilar
COPY . .
WORKDIR "/src/NeoLibro.WebAPI"
RUN dotnet build "NeoLibro.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NeoLibro.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "NeoLibro.WebAPI.dll"]
