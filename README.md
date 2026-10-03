# Supermercado API

API REST en .NET 10 para administrar las categorías y los productos de un supermercado.
Usa Entity Framework Core (MySQL en producción, SQLite en local) y pruebas BDD (Gherkin) con Reqnroll.

## Demo en línea

**https://supermercado-api-1m3u.onrender.com**

Abre la URL en el navegador para entrar a la documentación interactiva (Scalar), donde puedes probar cada endpoint.

Los datos se guardan en MySQL (Aiven), así que persisten entre reinicios.

> Está en el plan gratuito de Render. Después de 15 minutos sin uso el servicio se duerme y la primera petición tarda cerca de un minuto.

## Requisitos

- .NET SDK 10

## Ejecutar

```bash
dotnet tool restore
dotnet run --project src/Supermercado.Api
```

Por defecto usa SQLite: la base `supermercado.db` se crea y migra sola al arrancar.

## Base de datos

El motor se elige con configuración (variables de entorno o `appsettings.json`):

| Variable                          | Valor                                              |
|-----------------------------------|----------------------------------------------------|
| `Database__Provider`              | `Sqlite` (por defecto) o `MySql`                   |
| `ConnectionStrings__Supermercado` | Cadena de conexión del motor elegido               |

Cada motor tiene sus propias migraciones en `src/Supermercado.Api/Data/Migrations/` y se aplican solas al arrancar.
En `src/Supermercado.Api/Supermercado.Api.http` hay peticiones de ejemplo.

## Pruebas BDD

```bash
dotnet test
```

Los escenarios están en `tests/Supermercado.Specs/Features` y se escribieron en español (`# language: es`).
Cada escenario levanta la API en memoria con su propia base SQLite en memoria.

## Endpoints

| Método | Ruta                   | Descripción                                          |
|--------|------------------------|------------------------------------------------------|
| GET    | `/api/categories`      | Lista las categorías                                 |
| GET    | `/api/categories/{id}` | Muestra una categoría con sus productos              |
| POST   | `/api/categories`      | Crea una categoría (`name` es requerido y no se repite, sin importar mayúsculas) |
| PUT    | `/api/categories/{id}` | Edita una categoría (con las mismas reglas)          |
| DELETE | `/api/categories/{id}` | Elimina una categoría; responde 409 si tiene productos |
| GET    | `/api/products`        | Lista los productos con el id y el nombre de su categoría |
| GET    | `/api/products/{id}`   | Muestra un producto con el id y el nombre de su categoría |
| POST   | `/api/products`        | Crea un producto (`name` y `categoryId` son obligatorios) |
| PUT    | `/api/products/{id}`   | Edita un producto, incluida su categoría             |
| DELETE | `/api/products/{id}`   | Elimina un producto                                  |
