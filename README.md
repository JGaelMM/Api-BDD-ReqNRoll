# API de Categorías y Productos

API REST en **.NET 10** para administrar categorías y sus productos, desarrollada con **BDD** (Gherkin + ReqNRoll): primero se escriben los escenarios de negocio y después el código que los hace pasar.

> Estado: en desarrollo. Este README se irá completando conforme avance el proyecto.

## Stack

| Área | Tecnología |
| --- | --- |
| API | ASP.NET Core 10 (controllers) |
| ORM | Entity Framework Core |
| Base de datos | SQL Server 2022 Express (Docker) |
| BDD | ReqNRoll + xUnit |
| Pruebas unitarias | xUnit |

## Estructura

```
CategoriesProducts.slnx
src/
  CategoriesProducts.Api/          API (controllers, servicios, EF Core)
tests/
  CategoriesProducts.Specs/        Escenarios Gherkin (.feature) y step definitions de ReqNRoll
  CategoriesProducts.UnitTests/    Pruebas unitarias (TDD)
```

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (en Windows requiere WSL2)

## Cómo correr

1. Levantar SQL Server Express en Docker (la primera vez descarga la imagen):

   ```bash
   docker compose up -d
   ```

   La contraseña de `sa` por defecto es solo para desarrollo local; se puede cambiar creando un archivo `.env` con `MSSQL_SA_PASSWORD=<otra contraseña>`.

2. Correr la API:

   ```bash
   dotnet run --project src/CategoriesProducts.Api
   ```

   Al arrancar, la API crea la base de datos `CategoriesProducts` y aplica las migraciones pendientes de Entity Framework.

### Crear una migración nueva

`dotnet-ef` está registrado como herramienta local del repositorio (`dotnet-tools.json`):

```bash
dotnet tool restore
dotnet ef migrations add <Nombre> --project src/CategoriesProducts.Api --output-dir Data/Migrations
```

La API queda en `http://localhost:5071`.

## Cómo correr las pruebas

Docker Desktop debe estar corriendo: los escenarios BDD levantan su propio SQL Server en un contenedor temporal (Testcontainers), independiente del de `docker compose`.

```bash
dotnet test
```

## Reglas de negocio

**Categorías**
- Nombre requerido; descripción opcional.
- El nombre no se puede repetir, sin importar mayúsculas/minúsculas.
- Al editar aplican las mismas reglas.
- Se pueden listar (INDEX) y ver en detalle (SHOW).
- No se puede eliminar una categoría con productos asignados.

**Productos**
- Nombre y categoría requeridos; descripción opcional.
- Se puede editar, incluida su categoría.
- Al ver un producto se muestra el ID y el nombre de su categoría.
- Al eliminar todos los productos de una categoría, la categoría ya se puede eliminar.

## Decisiones

- **SQL Server en Docker**: es la base recomendada por la prueba y con Docker se levanta en cualquier máquina sin instalar SQL Server.
- **Arquitectura simple por capas** (Controllers → Services → EF Core) en un solo proyecto de API: el dominio es pequeño y una arquitectura con más capas agregaría ceremonia sin beneficio.
- **Escenarios BDD contra la API real**: los steps de ReqNRoll hacen peticiones HTTP a la API levantada en memoria (`WebApplicationFactory`) con un SQL Server real en contenedor (Testcontainers). Así los escenarios validan también códigos HTTP y mensajes.
- **Unicidad case-insensitive en la aplicación**: se guarda un nombre normalizado con índice único en lugar de depender de la collation de la base de datos.
- **Borrado restringido en la base de datos**: la llave foránea de productos a categorías usa `ON DELETE RESTRICT`, así que aunque falle la validación de la API, SQL Server tampoco permite borrar una categoría con productos.
- **Migraciones al arrancar**: la API aplica las migraciones al iniciar para que correrla sea un solo paso. En un sistema con varias instancias se aplicarían en el pipeline de despliegue.

## Supuestos

- Se ignoran espacios al inicio y al final del nombre: `" Bebidas "` y `"bebidas"` se consideran el mismo nombre.
- Los nombres de producto sí se pueden repetir (la regla de unicidad solo aplica a categorías).
- Crear o editar un producto con una categoría inexistente responde `400 Bad Request`.
- Longitud máxima: nombre 100 caracteres, descripción 500.
- Los listados (INDEX) son paginados (`page`, `pageSize`; por defecto 1 y 20, máximo 100), ya que devolver todos los registros no escala.

## Uso de IA

_Pendiente: se documentará al final del desarrollo._
