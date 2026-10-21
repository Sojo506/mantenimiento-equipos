# Sistema de Gestión de Mantenimiento de Equipos

[![CI](https://github.com/Sojo506/mantenimiento-equipos/actions/workflows/ci.yml/badge.svg)](https://github.com/Sojo506/mantenimiento-equipos/actions/workflows/ci.yml)
![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![C# 13](https://img.shields.io/badge/C%23-13-239120?logo=csharp)
![EF Core 10](https://img.shields.io/badge/EF%20Core-10.0-6C2D82)
![Architecture](https://img.shields.io/badge/Architecture-Clean%204--Layers-007ACC)

Proyecto Académico para el curso **SC-701 Programación Avanzada Web**  
**Universidad Fidélitas** — **Avance 1 (Semana 7)**  
**Estudiante:** Fabián Sojo  

---

## 1. Descripción del Proyecto

El **Sistema de Gestión de Mantenimiento de Equipos** es una solución integral orientada al entorno industrial y hospitalario para la administración del ciclo de vida de maquinaria, planeación de mantenimientos preventivos y correctivos, control riguroso de inventario de repuestos con alertas de stock crítico y auditoría inmutable de eventos.

La solución está construida bajo una **Arquitectura Limpia desacoplada en 4 capas**, garantizando separación estricta de responsabilidades, alta testabilidad, concurrencia optimista y cumplimiento de las mejores prácticas de ingeniería de software.

---

## 2. Arquitectura de Software (Clean Architecture)

El backend de la solución (`MantenimientoApi`) está segmentado en 4 proyectos desacoplados con dependencias unidireccionales:

```
┌─────────────────────────────────────────────────────────────┐
│                       Capa de API                           │
│  (Controladores REST, Minimal APIs, Scalar Docs, RFC 7807)  │
└──────────────────────────────┬──────────────────────────────┘
                               │ referencia
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                      Capa de Flujo                          │
│     (Lógica de Negocio, Máquina de Estados, RN-01 a RN-09)  │
└──────────────────────────────┬──────────────────────────────┘
                               │ referencia
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                      Capa de DA                             │
│     (DbContext, Fluent API, Repositorios, Migraciones EF)   │
└──────────────────────────────┬──────────────────────────────┘
                               │ referencia
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                  Capa de Abstracciones                      │
│     (Entidades de Dominio, DTOs, Interfaces, Excepciones)   │
└─────────────────────────────────────────────────────────────┘
```

### Proyectos de la Solución

| Proyecto | Responsabilidad Arquitectural | Tecnologías Clave |
| :--- | :--- | :--- |
| **`Abstracciones`** | Núcleo transversal con entidades de dominio, contratos (`Interfaces`), DTOs de entrada y salida, atributos de validación y excepciones de negocio personalizadas. | .NET 10, C# 13, DataAnnotations |
| **`DA` (Data Access)** | Única capa con acceso directo a la base de datos SQL Server mediante Entity Framework Core. Mapeo estricto con Fluent API (3FN), repositorios especializados, semilla estática y Stored Procedure `sp_ReporteConsumoRepuestosPorPeriodo`. | EF Core 10, SQL Server, Migraciones |
| **`Flujo` (Business Logic)** | Orquestación de reglas de negocio (RN-01 a RN-09), máquina de estados de órdenes, cálculo automático de fechas límites según prioridad, validación de stock y control de concurrencia optimista. | Inyección de Dependencias, Options Pattern |
| **`API` (Presentation)** | Exposición de endpoints RESTful mediante controladores y Minimal APIs, documentación interactiva Scalar, middleware de errores RFC 7807 (`application/problem+json`), paginación (`X-Pagination`), caché HTTP 304 (`ETag`) y página de estado `wwwroot`. | ASP.NET Core 10, Scalar.AspNetCore, CORS |
| **`Flujo.Tests`** | Suite de pruebas unitarias automatizadas que cubren las reglas de negocio, concurrencia y restricciones operativas. | xUnit, Moq, Coverlet |

---

## 3. Modelo de Datos y Normalización (3FN)

El diseño relacional se encuentra normalizado en **Tercera Forma Normal (3FN)** y mapeado al 100% mediante Fluent API en `DA/Configuraciones`:

- **Catálogos Paramétricos**: `ESTADO_TB`, `TIPO_USUARIO_TB`, `TIPO_EQUIPO_TB`, `MARCA_TB`, `MODELO_TB`, `UBICACION_TB`, `ESPECIALIDAD_TB`, `TIPO_ORDEN_TB`, `TIPO_GRAVEDAD_TB`.
- **Entidades de Operación**: `USUARIO_TB`, `EQUIPO_TB`, `REPUESTO_TB`, `ORDEN_TB`, `ORDEN_REPUESTO_TB`, `BITACORA_ORDEN_TB`, `PLAN_MANTENIMIENTO_TB`, `DETALLE_PLAN_TB`, `FALLA_REGISTRADA_TB`.
- **Concurrencia Optimista**: Las tablas `REPUESTO_TB` y `ORDEN_TB` contienen un token nativo `ROW_VERSION` (`rowversion` / `byte[]`) configurado con `.IsRowVersion()` para prevenir sobrescritura concurrente de datos.
- **Auditoría Inmutable**: `BITACORA_ORDEN_TB` almacena el histórico cronológico de cada transición de estado de orden sin métodos de actualización o borrado en su repositorio.
- **Stored Procedure Integrado**: Migración inicial incluye `sp_ReporteConsumoRepuestosPorPeriodo` para reportería analítica de costos y cantidades consumidas.

---

## 4. Configuración Segura de Secretos (Zero Secrets)

Por estrictas normas de seguridad, **el repositorio no contiene credenciales de base de datos ni secretos en texto plano**.

La cadena de conexión se administra localmente mediante **.NET User Secrets** (asociado al UserSecretsId configurado en `API.csproj`):

### Configuración de la Conexión Local

Ejecuta el siguiente comando en la raíz del repositorio o dentro de la carpeta `MantenimientoApi`:

```bash
# Para SQL Server LocalDB (entornos de desarrollo estándar):
dotnet user-secrets set "ConnectionStrings:Default" "Server=(localdb)\mssqllocaldb;Database=MantenimientoEquiposDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" --project MantenimientoApi/API/API.csproj

# Para SQL Server Express:
dotnet user-secrets set "ConnectionStrings:Default" "Server=.\SQLEXPRESS;Database=MantenimientoEquiposDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" --project MantenimientoApi/API/API.csproj
```

---

## 5. Aplicación de Migraciones y Semilla

Una vez configurada la cadena en User Secrets, aplica la migración inicial contra tu servidor SQL Server:

```bash
dotnet ef database update --project MantenimientoApi/DA/DA.csproj --startup-project MantenimientoApi/API/API.csproj
```

Al ejecutarse, EF Core creará la base de datos `MantenimientoEquiposDb`, aplicará las restricciones de clave primaria y foránea en 3FN, registrará el Stored Procedure y poblará la semilla inicial (32 órdenes de trabajo, 6 equipos, 7 repuestos y 10 usuarios).

---

## 6. Usuarios de Prueba (Semilla Inicial)

Las contraseñas se encuentran cifradas con **PBKDF2 con HMAC-SHA256 (100,000 iteraciones y salt criptográfico de 128 bits)**:

| Identificación | Nombre Completo | Rol | Correo Electrónico | Contraseña de Prueba |
| :--- | :--- | :--- | :--- | :--- |
| `109990888` | Carlos Alvarado Mora | Administrador | `calvarado@mantenimiento.cr` | `Admin123*` |
| `111110222` | Laura Rojas Jiménez | Administrador | `lrojas@mantenimiento.cr` | `Admin123*` |
| `203330444` | Andrés Chaves Solís | Técnico (Mecánica) | `achaves@mantenimiento.cr` | `Tecnico123*` |
| `304440555` | Mariana Gómez Castro | Técnico (Electrónica) | `mgomez@mantenimiento.cr` | `Tecnico123*` |
| `405550666` | Javier Vargas Mora | Técnico (Refrigeración) | `jvargas@mantenimiento.cr` | `Tecnico123*` |
| `506660777` | Sofía Morales Soto | Técnico (Biomédica) | `smorales@mantenimiento.cr` | `Tecnico123*` |
| `607770888` | Roberto Jiménez Vega | Técnico (Hidráulica) | `rjimenez@mantenimiento.cr` | `Tecnico123*` |
| `112340567` | Juan Pérez Solano | Solicitante (Producción) | `jperez@mantenimiento.cr` | `Usuario123*` |
| `203450678` | Ana Castillo Blanco | Solicitante (Control Calidad) | `acastillo@mantenimiento.cr` | `Usuario123*` |
| `304560789` | Diego Monge Arias | Solicitante (Laboratorio) | `dmonge@mantenimiento.cr` | `Usuario123*` |

---

## 7. Ejecución y Pruebas

### Compilación Completa (0 advertencias, 0 errores)
```bash
dotnet build MantenimientoApi/MantenimientoApi.slnx
```

### Ejecución de Pruebas Unitarias
```bash
dotnet test MantenimientoApi/MantenimientoApi.slnx
```

### Ejecución del Servidor API
```bash
dotnet run --project MantenimientoApi/API/API.csproj
```

Una vez en ejecución:
- **Página de Estado (Landing)**: `http://localhost:5000/` o `https://localhost:7001/`
- **Documentación Interactiva Scalar**: `http://localhost:5000/scalar/v1`
- **Especificación OpenAPI**: `http://localhost:5000/openapi/v1.json`
- **Diagnóstico de Salud**: `http://localhost:5000/api/salud`

---

## 8. Módulos y Endpoints Principales

### 📦 Equipos (`/api/equipos`)
- `GET /api/equipos`: Listado general con búsqueda, ordenamiento y encabezado de paginación `X-Pagination`.
- `GET /api/equipos/{codigoEquipo:codigoRuta}`: Detalle por código con soporte de caché `ETag` (HTTP 304).
- `POST /api/equipos`: Registro de nuevo equipo validando código alfanumérico.
- `PUT /api/equipos/{codigoEquipo:codigoRuta}`: Actualización de datos generales.
- `DELETE /api/equipos/{codigoEquipo:codigoRuta}`: Eliminación condicionada a no tener órdenes asociadas.

### ⚙️ Repuestos (`/api/repuestos`)
- `GET /api/repuestos`: Inventario completo con paginación `X-Pagination`.
- `GET /api/repuestos/bajo-minimo`: Consulta de repuestos en nivel crítico de inventario (`stock <= mínimo`).
- `GET /api/repuestos/{codigoRepuesto:codigoRuta}`: Detalle por código con validación `ETag`.
- `POST /api/repuestos`: Registro de nuevo repuesto con validación de código.
- `PUT /api/repuestos/{codigoRepuesto:codigoRuta}`: Actualización con validación de concurrencia optimista (`If-Match` / `RowVersion`).
- `DELETE /api/repuestos/{codigoRepuesto:codigoRuta}`: Eliminación de repuesto si no posee consumo histórico.

### 🛠️ Órdenes de Trabajo (`/api/ordenes`)
- `GET /api/ordenes`: Listado de órdenes con filtros por equipo, técnico, solicitante, estado, prioridad y paginación.
- `GET /api/ordenes/{numero}`: Consulta de orden por número con `ETag`.
- `POST /api/ordenes`: Creación de orden calculando automáticamente fecha límite según prioridad (RN-08) y registrando bitácora inicial.
- `PUT /api/ordenes/{numero}`: Actualización operativa (RN-09: inmutable si la orden está Cerrada).
- `PUT /api/ordenes/{numero}/estado`: Máquina de estados estricta y registro inmutable en bitácora.
- `POST /api/ordenes/{numero}/repuestos`: Asignación de repuestos deduciendo existencias de bodega (RN-05).
- `GET /api/ordenes/{numero}/historial`: Historial cronológico de cambios de estado y bitácora.

### 📊 Tablero de Control (Minimal APIs)
- `GET /api/tablero/equipos-por-estado`: Conteo consolidado de equipos agrupados por estado.
- `GET /api/tablero/ordenes-por-prioridad`: Distribución de órdenes por prioridad y conteo de órdenes activas.
- `GET /api/tablero/repuestos-bajo-minimo`: Resumen de repuestos bajo stock mínimo con cálculo de déficit.
- `GET /api/salud`: Verificación de operatividad del API y conectividad con la base de datos.

---

## 9. Lista de Verificación — Avance 1 (Semana 7)

- [x] **Arquitectura Limpia**: Separación estricta en 4 capas (`Abstracciones`, `DA`, `Flujo`, `API`).
- [x] **Modelo Relacional en 3FN**: 13 tablas mapeadas mediante Fluent API según el diagrama entidad-relación del proyecto.
- [x] **Concurrencia Optimista**: Tokens `ROW_VERSION` en `REPUESTO_TB` y `ORDEN_TB`.
- [x] **Stored Procedure**: `sp_ReporteConsumoRepuestosPorPeriodo` integrado en migración y DbContext.
- [x] **Semilla de Datos**: 32 órdenes, equipos, repuestos, catálogos y usuarios con contraseñas seguras PBKDF2.
- [x] **Reglas de Negocio (RN-01 a RN-09)**: Máquina de estados, trazabilidad en bitácora inmutable, validaciones de cierre y stock.
- [x] **CRUD Completo de 3 Entidades**: `Equipos`, `Repuestos` y `Órdenes de Trabajo`.
- [x] **Estándar de Errores RFC 7807**: Respuestas de error estructuradas en `application/problem+json` vía `IExceptionHandler`.
- [x] **Paginación y Caché HTTP**: Encabezados `X-Pagination`, `ETag` y respuestas `304 Not Modified`.
- [x] **Documentación Scalar**: Explorador interactivo moderno en `/scalar/v1`.
- [x] **Pruebas Unitarias**: 15 pruebas automatizadas pasando al 100% en verde con xUnit y Moq.
- [x] **CI Automatizado**: Workflow de GitHub Actions (`ci.yml`) compilando y ejecutando pruebas en verde.
- [x] **Gestión Segura de Secretos**: Sin credenciales en texto plano; documentación para User Secrets.
