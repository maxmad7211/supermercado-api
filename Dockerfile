FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Supermercado.Api/Supermercado.Api.csproj src/Supermercado.Api/
RUN dotnet restore src/Supermercado.Api/Supermercado.Api.csproj
COPY src/Supermercado.Api/ src/Supermercado.Api/
RUN dotnet publish src/Supermercado.Api/Supermercado.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
USER app

# La base de datos (MySQL en Aiven) se configura en appsettings.Production.json.
# Render envía el tráfico al puerto 10000 por defecto y termina HTTPS en su proxy;
# ASPNETCORE_FORWARDEDHEADERS_ENABLED hace que la app respete X-Forwarded-Proto
# para que la documentación apunte a https y no a http.
ENV ASPNETCORE_HTTP_PORTS=10000 \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
EXPOSE 10000

ENTRYPOINT ["dotnet", "Supermercado.Api.dll"]
