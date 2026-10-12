# API de Categorías y Productos

API REST en **.NET 10** con **Entity Framework Core** y **SQL Server** para administrar categorías y sus productos, desarrollada con **BDD** (Gherkin + ReqNRoll) y **TDD**: primero se escribieron los escenarios de negocio y las pruebas, y después el código que los hace pasar.

- **84 escenarios BDD** y **66 pruebas unitarias**, todos en verde y ejecutados en cada push por GitHub Actions.
- Se levanta completa con `docker compose up -d --build` y se prueba desde el navegador con Swagger UI.

## Contenido

- [Inicio rápido](#inicio-rápido)
- [Stack y estructura](#stack-y-estructura)
- [Cómo correr](#cómo-correr)
- [Cómo correr las pruebas](#cómo-correr-las-pruebas)
- [Endpoints](#endpoints)
- [Códigos HTTP y errores](#códigos-http-y-errores)
- [Cómo se trabajó](#cómo-se-trabajó)
- [Decisiones](#decisiones)
- [Supuestos](#supuestos)
- [Cómo aprendí BDD y ReqNRoll](#cómo-aprendí-bdd-y-reqnroll)
- [Uso de IA](#uso-de-ia)

## Inicio rápido

Solo se necesita [Docker Desktop](https://www.docker.com/products/docker-desktop/):

```bash
git clone https://github.com/JGaelMM/Api-BDD-ReqNRoll.git
cd Api-BDD-ReqNRoll
docker compose up -d --build
```

Abrir **http://localhost:8080**: redirige a Swagger UI, donde se pueden probar todos los endpoints.

## Stack y estructura

| Área | Tecnología |
| --- | --- |
| API | ASP.NET Core 10 (controllers) |
| ORM | Entity Framework Core 10 (migraciones) |
| Base de datos | SQL Server 2022 Express en Docker |
| BDD | ReqNRoll 3 + xUnit, escenarios en Gherkin en español |
| Pruebas unitarias | xUnit + SQLite en memoria |
| Pruebas de integración | `WebApplicationFactory` + Testcontainers (SQL Server real) |
| Documentación | OpenAPI nativo de .NET 10 + Swagger UI |
| CI | GitHub Actions |

```
CategoriesProducts.slnx
docker-compose.yml             SQL Server Express + API
Dockerfile                     Imagen de la API (multi-stage)
src/
  CategoriesProducts.Api/
    Controllers/               Endpoints HTTP
    Services/                  Reglas de negocio y validaciones
    Data/                      AppDbContext y migraciones de EF Core
    Entities/                  Category y Product
    Dtos/                      Contratos de entrada y salida
    Errors/                    Excepciones de negocio y su traducción a ProblemDetails
tests/
  CategoriesProducts.Specs/
    Features/                  Escenarios Gherkin (.feature)
    StepDefinitions/           Código que conecta cada paso con la API
    Hooks/                     Arranque de SQL Server (Testcontainers) y de la API
    Support/                   Estado compartido entre pasos
  CategoriesProducts.UnitTests/  Pruebas unitarias de entidades, validaciones y servicios
```

## Cómo correr

### Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (en Windows requiere WSL2).
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), solo para la opción B y para correr las pruebas.

### Opción A: todo en Docker

```bash
docker compose up -d --build
```

Levanta SQL Server Express y la API. La API queda en **http://localhost:8080** (Swagger UI).

Para detenerlo: `docker compose down`. Los datos se conservan en el volumen `sqlserver-data`; `docker compose down -v` los borra.

### Opción B: la API con el SDK de .NET (para desarrollo)

1. Levantar solo SQL Server Express:

   ```bash
   docker compose up -d sqlserver
   ```

2. Correr la API:

   ```bash
   dotnet run --project src/CategoriesProducts.Api
   ```

   La API queda en **http://localhost:5071** (Swagger UI en `/swagger`). El archivo [`CategoriesProducts.Api.http`](src/CategoriesProducts.Api/CategoriesProducts.Api.http) tiene peticiones de ejemplo para VS Code o Visual Studio.

En ambas opciones la API crea la base de datos `CategoriesProducts` y aplica las migraciones de Entity Framework al arrancar.

La contraseña de `sa` por defecto es solo para desarrollo local; se puede cambiar creando un archivo `.env` con `MSSQL_SA_PASSWORD=<otra contraseña>`.

### Ver la base de datos

Con cualquier cliente de SQL Server (por ejemplo la extensión **SQL Server (mssql)** de VS Code o SSMS):

| Campo | Valor |
| --- | --- |
| Servidor | `localhost,1433` |
| Usuario / contraseña | `sa` / `CategoriesProducts_Dev1` |
| Base de datos | `CategoriesProducts` |
| Opciones | Trust server certificate |

### Crear una migración nueva

`dotnet-ef` está registrado como herramienta local del repositorio (`dotnet-tools.json`):

```bash
dotnet tool restore
dotnet ef migrations add <Nombre> --project src/CategoriesProducts.Api --output-dir Data/Migrations
```

## Cómo correr las pruebas

Requiere el .NET 10 SDK y Docker Desktop corriendo: los escenarios BDD levantan su propio SQL Server en un contenedor temporal (Testcontainers), independiente del de `docker compose`, y lo eliminan al terminar.

```bash
dotnet test
```

Resultado esperado:

```
Correctas! - Con error: 0, Superado: 66, Total: 66 - CategoriesProducts.UnitTests.dll
Correctas! - Con error: 0, Superado: 84, Total: 84 - CategoriesProducts.Specs.dll
```

Para correr solo una característica, por ejemplo eliminar categorías:

```bash
dotnet test tests/CategoriesProducts.Specs --filter "FullyQualifiedName~EliminarCategoria"
```

En cada push y pull request a `main`, [GitHub Actions](.github/workflows/ci.yml) compila la solución, corre todas las pruebas y construye la imagen Docker de la API. Los resultados quedan como artefacto descargable en cada ejecución.

## Endpoints

| Método | Ruta | Descripción | Respuestas |
| --- | --- | --- | --- |
| `GET` | `/api/categories?page=1&pageSize=20` | Lista paginada, ordenada por nombre | 200, 400 |
| `GET` | `/api/categories/{id}` | Detalle | 200, 404 |
| `POST` | `/api/categories` | Crear | 201, 400, 409 |
| `PUT` | `/api/categories/{id}` | Editar | 200, 400, 404, 409 |
| `DELETE` | `/api/categories/{id}` | Eliminar (solo si no tiene productos) | 204, 404, 409 |
| `GET` | `/api/products?page=1&pageSize=20` | Lista paginada, ordenada por nombre | 200, 400 |
| `GET` | `/api/products/{id}` | Detalle | 200, 404 |
| `POST` | `/api/products` | Crear | 201, 400 |
| `PUT` | `/api/products/{id}` | Editar, incluida su categoría | 200, 400, 404 |
| `DELETE` | `/api/products/{id}` | Eliminar | 204, 404 |

Ejemplo de producto: siempre incluye el id **y el nombre** de su categoría.

```json
{
  "id": 1,
  "name": "Coca-Cola",
  "description": "Refresco de cola",
  "category": { "id": 1, "name": "Bebidas" }
}
```

Ejemplo de listado paginado:

```json
{
  "items": [ { "id": 1, "name": "Bebidas", "description": "Refrescos y jugos" } ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 1
}
```

## Códigos HTTP y errores

Todos los errores responden con el formato estándar [ProblemDetails (RFC 9457)](https://www.rfc-editor.org/rfc/rfc9457), con un mensaje útil en `detail`:

```json
{
  "title": "Conflicto",
  "status": 409,
  "detail": "Esta categoría no se puede eliminar porque tiene productos asignados."
}
```

| Código | Cuándo | Ejemplos de `detail` |
| --- | --- | --- |
| `200 OK` | Consulta o edición exitosa | |
| `201 Created` | Creación exitosa; incluye el header `Location` | |
| `204 No Content` | Eliminación exitosa | |
| `400 Bad Request` | Datos inválidos | "El nombre es requerido.", "El nombre no puede tener más de 100 caracteres.", "La categoría es requerida.", "La categoría indicada no existe.", "La página debe ser mayor o igual a 1." |
| `404 Not Found` | El recurso no existe | "No se encontró la categoría.", "No se encontró el producto." |
| `409 Conflict` | La operación choca con los datos actuales | "Ya existe una categoría con el nombre 'BEBIDAS'.", "Esta categoría no se puede eliminar porque tiene productos asignados." |
| `500` | Error inesperado | Mensaje genérico, sin exponer detalles internos |


## Cómo se trabajó

### Ciclo BDD / TDD

1. **Escenarios primero.** Los 84 escenarios se escribieron **antes que cualquier código de la API**. En ese momento los 84 fallaban.
2. **Infraestructura de pruebas.** Antes de implementar funcionalidad se montó el entorno donde corren los escenarios: la API vacía levantada en memoria y un SQL Server en contenedor.
3. **Por cada bloque funcional:**
   - **Rojo:** se implementan los steps del bloque y las pruebas unitarias del servicio, y se comprueba que fallan.
   - **Verde:** se escribe el código mínimo para que pasen.
   - **Refactor:** se limpia con la red de seguridad de las pruebas.
4. **Verificación manual.** Al cerrar cada bloque también se probó la API real contra el SQL Server de `docker compose`.

Avance de los escenarios a lo largo del historial:

| Momento | Escenarios en verde |
| --- | --- |
| Escenarios escritos | 0 / 84 |
| Crear categorías | 13 / 84 |
| Consultar y editar categorías | 40 / 84 |
| Crear, consultar, editar y eliminar productos | 78 / 84 |
| Eliminar categorías | **84 / 84** |

### Planeación y división de commits

El desarrollo se planeó por fases antes de escribir código. La primera versión del plan tenía alrededor de 40 commits (uno por archivo o por regla), pero resultaba demasiado granular; se reagrupó en **bloques funcionales**. Cada commit agrega una capacidad completa (sus steps, sus pruebas unitarias y su código) y deja todas sus pruebas en verde, para que el historial cuente la historia del proyecto sin commits gigantes ni commits triviales.

| Fase | Commits | Contenido |
| --- | --- | --- |
| 1. Proyecto y escenarios | `first commit` … `test(bdd): add delete category and product scenarios` | Proyecto generado con plantillas sin modificar, limpieza, README inicial y **todos los escenarios Gherkin**, divididos por característica |
| 2. Infraestructura | `chore: add docker compose…`, `feat: add EF Core data model…`, `test: run specs against the API…` | SQL Server en Docker, modelo de datos y migración, entorno de pruebas con Testcontainers |
| 3. Categorías | `feat: create categories…`, `feat: list and show categories…`, `feat: update categories` | Un commit por operación |
| 4. Productos | `feat: create and view products…`, `feat: update and delete products`, `feat: delete categories without assigned products` | Eliminar categorías va al final porque sus escenarios necesitan productos |
| 5. Entrega | `feat: add Swagger UI…`, `ci: run build and tests…`, `docs: complete README` | Swagger, Docker de la API, CI y documentación |

Los mensajes siguen [Conventional Commits](https://www.conventionalcommits.org/) (`feat`, `test`, `chore`, `ci`, `docs`) para que el tipo de cambio se lea de un vistazo.

## Decisiones

- **SQL Server en Docker:** es la base recomendada por la prueba, y con Docker se levanta en cualquier máquina sin instalar SQL Server. La misma imagen se usa en desarrollo, en las pruebas y en CI.
- **Arquitectura simple por capas** (Controllers → Services → EF Core) en un solo proyecto de API: el dominio es pequeño y una arquitectura con más capas agregaría ceremonia sin beneficio. Los controllers solo traducen HTTP; las reglas viven en los servicios.
- **Escenarios BDD contra la API real:** los steps hacen peticiones HTTP a la API en memoria con un SQL Server real (Testcontainers). Así validan también rutas, códigos HTTP, mensajes y el comportamiento real de la base de datos (collation, índices, llaves foráneas), que una base en memoria no reproduce.
- **Pruebas unitarias con SQLite en memoria:** las pruebas de servicios necesitan una base relacional (índices únicos, llaves foráneas), pero deben ser rápidas y no depender de Docker. SQLite en memoria cumple ambas cosas; lo que depende de SQL Server se cubre en los escenarios BDD.
- **Errores como excepciones de negocio + ProblemDetails:** los servicios lanzan `ValidationException` (400), `NotFoundException` (404) o `ConflictException` (409), y un `IExceptionHandler` global las convierte en ProblemDetails. Los controllers quedan limpios y todas las respuestas de error tienen el mismo formato.
- **Unicidad case-insensitive en la aplicación:** se guarda un `NormalizedName` (sin espacios a los lados y en mayúsculas) con índice único, en lugar de depender de la collation de la base de datos. Si dos peticiones con el mismo nombre llegan al mismo tiempo, el índice rechaza la segunda y se responde igual que un duplicado (409).
- **Borrado restringido también en la base de datos:** la llave foránea de productos a categorías usa `ON DELETE RESTRICT`. Si se asigna un producto justo después de validar, SQL Server rechaza el borrado y la API responde con el mismo mensaje.
- **DTOs separados de las entidades:** la API nunca expone las entidades de EF; el producto se devuelve con un resumen de su categoría (`id` y `name`) obtenido con un `JOIN`, así siempre muestra el nombre actual.
- **Migraciones al arrancar:** la API aplica las migraciones al iniciar para que correrla sea un solo paso. En un sistema con varias instancias se aplicarían en el pipeline de despliegue.
- **Swagger en todos los entornos:** para que la API se pueda probar desde el navegador también en Docker y en el deploy.

## Supuestos

Puntos que no estaban especificados y cómo se resolvieron:

- Se ignoran los espacios al inicio y al final del nombre: `" Bebidas "` se guarda como `"Bebidas"` y se considera igual a `"bebidas"`.
- Una descripción vacía o de solo espacios se guarda como "sin descripción" (`null`).
- Los nombres de producto **sí** se pueden repetir; la regla de unicidad solo aplica a categorías.
- Crear o editar un producto con una categoría inexistente responde `400 Bad Request`, porque es un dato inválido del cuerpo de la petición, no un recurso de la URL.
- Longitudes máximas: nombre 100 caracteres, descripción 500.
- Los listados (INDEX) son paginados (`page` y `pageSize`; por defecto 1 y 20, máximo 100), ya que devolver todos los registros no escala. Se ordenan por nombre.
- Editar es un reemplazo completo (`PUT`): si no se envía descripción, la descripción se quita.
- Una categoría puede renombrarse a su mismo nombre con otras mayúsculas ("Bebidas" → "BEBIDAS").

## Cómo aprendí BDD y ReqNRoll

No había usado BDD ni ReqNRoll antes de esta prueba. Lo que fui aprendiendo:

- **Qué es un escenario:** es un ejemplo concreto de comportamiento (Dado / Cuando / Entonces) que cualquiera puede leer y que además se ejecuta. Funciona como criterio de aceptación y como prueba al mismo tiempo.
- **ReqNRoll en .NET:** se instala con el paquete de plantillas `Reqnroll.Templates.DotNet`; cada `.feature` se convierte en una clase de pruebas de xUnit, y cada paso se conecta a un método con `[Given]`, `[When]` o `[Then]` mediante *Cucumber Expressions* (`{string}`, `{int}`).
- **Gherkin en español:** con `# language: es` se usan `Característica`, `Escenario`, `Esquema del escenario`, `Antecedentes`, `Dado`, `Cuando` y `Entonces`. Las tablas de `Ejemplos` recortan los espacios de las celdas, por eso los casos de "nombre de solo espacios" son escenarios individuales.
- **Hooks e inyección de dependencias:** `[BeforeTestRun]` arranca SQL Server y la API una sola vez; `[BeforeScenario]` limpia la base. ReqNRoll crea una instancia nueva de los steps y del estado compartido por cada escenario.
- **Detalles que encontré en el camino:**
  - Si un escenario tiene pasos sin definir, ReqNRoll ni siquiera ejecuta los hooks; para validar la infraestructura usé un escenario temporal.
  - El cuerpo de una respuesta HTTP del servidor de pruebas solo se puede leer una vez; se guarda para que varios pasos "Entonces" lo revisen.
  - Los escenarios comparten una base de datos, así que se desactivó la ejecución en paralelo.
  - SQLite ordena texto por bytes y SQL Server por collation ("Lácteos" queda en posiciones distintas); por eso el orden con acentos se valida en los escenarios contra SQL Server.

## Uso de IA

Usé IA (Claude Code) como apoyo en la **planeación** y el **desarrollo**:

- **Planeación:** definir el stack, las fases del proyecto y la división en commits, y redactar los escenarios Gherkin a partir de los requerimientos.
- **Desarrollo:** generar el código de los steps, las pruebas unitarias y la implementación de cada bloque, siguiendo el ciclo rojo → verde.

**Cómo revisé lo que me daba:** yo orquesté el proceso parte por parte. La IA no hizo commits: cada commit lo hice yo después de verificar esa fase del proyecto:

- **Pruebas continuas:** en cada fase corrí `dotnet test` y comprobé que el resultado coincidiera con lo esperado (escenarios en rojo antes del código y en verde después, sin romper los anteriores).
- **Lectura del código** generado, para entender qué hacía y confirmar que respetara los requerimientos y las decisiones del proyecto.
- **Pruebas manuales** de la API real (archivo `.http`, Swagger y la base de datos) para confirmar el funcionamiento de punta a punta.
- **Ajustes de rumbo:** pedí cambios cuando algo no era del todo claro o podia simplificarse; por ejemplo, reagrupar el plan de commits o uso de swager para pruebas con ui.
