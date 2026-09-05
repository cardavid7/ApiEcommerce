# ApiEcommerce

Aplicación e-commerce full-stack compuesta por una API REST en **.NET 10** y un frontend en **Angular 21**. Incluye autenticación con JWT, roles (Admin / User), versionado de API (v1 / v2), carga de imágenes de productos y despliegue configurado para [Render](https://render.com).

## Estructura del repositorio

```
ApiEcommerce/
├── backend/     # API REST en ASP.NET Core (.NET 10) + PostgreSQL
├── frontend/    # SPA en Angular 21 + Tailwind CSS
└── render.yaml  # Configuración de despliegue (Render Blueprint)
```

## Backend (`/backend`)

**Stack:** ASP.NET Core 10, Entity Framework Core (Npgsql/PostgreSQL), ASP.NET Identity, JWT Bearer, Mapster (mapeo de DTOs), Asp.Versioning, Swagger/OpenAPI, BCrypt.

### Características principales

- Autenticación y autorización con JWT e Identity, con roles `Admin` y `User`.
- Versionado de API: `api/v1/...` y `api/v2/...` (por ejemplo, `Categories` tiene ambas versiones).
- CRUD de **Productos**, **Categorías** y **Usuarios** (registro y login).
- Subida de imágenes de productos (servidas como archivos estáticos desde `wwwroot`).
- Cache de respuestas configurado por perfiles.
- Seed automático de datos (roles, usuarios, categorías y productos) al iniciar la aplicación.
- Migraciones de EF Core aplicadas automáticamente al arrancar.

### Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- PostgreSQL en ejecución (local o remoto)

### Configuración

Editar `backend/appsettings.json` (o `appsettings.Development.json`) con tu cadena de conexión y clave secreta:

```json
{
  "EnableSwagger": true,
  "ApiSettings": { "SecretKey": "clave-secreta-larga" },
  "ConnectionStrings": {
    "ConexionSql": "Host=localhost;Port=5432;Database=dotnet_apiecommerce;Username=postgres;Password=123456"
  },
  "Cors": { "AllowedOrigins": ["http://localhost:4200"] }
}
```

### Ejecutar en local

```bash
cd backend
dotnet restore
dotnet run
```

Al iniciar, la app aplica las migraciones pendientes y siembra datos de ejemplo automáticamente. Con `EnableSwagger: true` la documentación queda disponible en `/swagger` (o `/openapi/v1.json`, `/openapi/v2.json`).

Usuarios sembrados por defecto (ver `Data/DataSeeder.cs`):

| Email               | Password     | Rol          |
|---------------------|--------------|--------------|
| admin@email.com     | Admin123!    | Admin        |
| user@email.com      | User123!     | User         |
| david@email.com     | David123!    | Admin + User |

### Migraciones de EF Core

```bash
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

### Docker

```bash
cd backend
docker build -t apiecommerce-backend .
docker run -p 8080:8080 apiecommerce-backend
```

## Frontend (`/frontend`)

**Stack:** Angular 21 (standalone components), Tailwind CSS 4, RxJS, Vitest.

### Estructura

```
src/app/
├── core/        # guards, interceptors, modelos y servicios (auth, categorías, productos, usuarios)
├── features/
│   ├── auth/      # login, registro
│   ├── catalog/   # listado y detalle de productos
│   ├── admin/     # gestión de productos, categorías y usuarios (rol Admin)
│   └── profile/   # perfil de usuario
├── layout/
└── shared/
```

Incluye guards de rutas (`auth`, `guest`, `role`) e interceptores HTTP para adjuntar el token JWT y manejar errores.

### Requisitos

- [Node.js](https://nodejs.org/) y npm

### Ejecutar en local

```bash
cd frontend
npm ci
npm start
```

La aplicación queda disponible en `http://localhost:4200`. Asegúrate de que el backend esté corriendo y que `http://localhost:4200` esté en `Cors:AllowedOrigins` del backend.

### Build de producción

```bash
npm run build
```

Los artefactos se generan en `dist/apiecommerce-frontend/browser`.

### Tests

```bash
npm test
```

## Despliegue

El archivo [`render.yaml`](render.yaml) define un *Blueprint* de Render con dos servicios:

- **`apiecommerce-backend`**: servicio web Docker (usa `backend/Dockerfile`).
- **`apiecommerce-frontend`**: sitio estático (build con `npm ci && npm run build`, con *rewrite* de rutas SPA a `index.html`).

Variables sensibles (`ApiSettings__SecretKey`, `ConnectionStrings__ConexionSql`) deben configurarse manualmente en el dashboard de Render (marcadas como `sync: false`).

## Licencia

Uso educativo / personal.
