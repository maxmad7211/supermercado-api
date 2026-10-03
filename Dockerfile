FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Supermercado.Api/Supermercado.Api.csproj src/Supermercado.Api/
RUN dotnet restore src/Supermercado.Api/Supermercado.Api.csproj
COPY src/Supermercado.Api/ src/Supermercado.Api/
RUN dotnet publish src/Supermercado.Api/Supermercado.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# La imagen corre con el usuario sin privilegios "app"; la BD SQLite vive en /data.
USER root
RUN mkdir /data && chown app /data
USER app

# Render envía el tráfico al puerto 10000 por defecto.
ENV ASPNETCORE_HTTP_PORTS=10000 \
    ConnectionStrings__Supermercado="Data Source=/data/supermercado.db"
EXPOSE 10000

ENTRYPOINT ["dotnet", "Supermercado.Api.dll"]
