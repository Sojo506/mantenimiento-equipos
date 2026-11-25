# Registro de Decisiones de Arquitectura y Seguridad (`DECISIONES.md`)

Proyecto: **Sistema de Gestión de Mantenimiento de Equipos**  
Curso: **SC-701 Programación Avanzada Web — Universidad Fidélitas**

---

## 1. Arquitectura Limpia en 4 Capas (`Abstracciones`, `DA`, `Flujo`, `API`)

### Decisión
Estructurar el backend (`MantenimientoApi`) en cuatro proyectos independientes con dependencias estrictamente unidireccionales:
- `API` → `Flujo` y `DA` (únicamente para registro en el contenedor de Inyección de Dependencias en `Program.cs`).
- `Flujo` → `Abstracciones`.
- `DA` → `Abstracciones`.
- `Abstracciones` → Sin dependencias hacia ninguna otra capa.

### Justificación
1. **Inversión de Dependencias (DIP):** La capa `Flujo` programa contra interfaces (`IRepositorioEquipo`, `IRepositorioOrdenDeTrabajo`, `IRepositorioFallaRegistrada`, `IRepositorioReportes`) definidas en `Abstracciones`, lo que permite sustituir o simular (`Moq`) el acceso a datos en `Flujo.Tests` sin tocar SQL Server.
2. **Aislamiento del `DbContext`:** Ningún controlador de la capa `API` ni ninguna vista de `Web-Mantenimiento` referencia a `MantenimientoDbContext`. Toda consulta, incluyendo autenticación (`AuthController` → `IAuthServicio`), tablero (`TableroController` → `IServicioTablero`) y diagnóstico (`SaludController` → `IServicioSalud`), atraviesa la capa de negocio.

---

## 2. Desacoplamiento Total entre Frontend MVC y Backend REST

### Decisión
`Web-Mantenimiento` no tiene referencia de proyecto hacia `DA`, `Flujo` ni `MantenimientoDbContext`. Consume el backend exclusivamente por HTTP mediante `IHttpClientFactory` y una jerarquía de servicios cliente (`ApiClientBase`).

### Justificación
- Previene el agotamiento de sockets (`SocketExhaustion`) gracias al pool de conexiones administrado por `IHttpClientFactory`.
- Centraliza la propagación del token JWT mediante un `DelegatingHandler` personalizado (`JwtDelegatingHandler`) que intercepta cada petición saliente del cliente `"MantenimientoApi"` e inyecta el encabezado `Authorization: Bearer <token>`.
- Traduce de forma uniforme las respuestas de error RFC 7807 (`application/problem+json`) y los conflictos `409 Conflict` a mensajes amigables en la interfaz MVC.

---

## 3. Estrategia de Autenticación JWT + Sesión con Cookie `HttpOnly`

### Decisión
1. **En el API (`MantenimientoApi`):** Autenticación sin estado (*stateless*) basada en **JWT Bearer** firmados con `HMAC-SHA256` (clave simétrica de al menos 32 bytes), validando `Issuer`, `Audience`, `Lifetime` con `ClockSkew = TimeSpan.Zero` y claims de `ClaimTypes.Role` y `ClaimTypes.NameIdentifier`.
2. **En el Cliente MVC (`Web-Mantenimiento`):** Almacenamiento del JWT del lado del servidor en la sesión de ASP.NET Core respaldada por una cookie de sesión con banderas `HttpOnly = true`, `SecurePolicy = Always`, `SameSite = Lax` e `IsEssential = true`.

### Justificación
- **Mitigación de XSS:** Al no guardar el JWT en `localStorage` ni `sessionStorage` del navegador y mantener la cookie de sesión como `HttpOnly`, ningún script malicioso en el cliente puede robar el token Bearer.
- **Autorización en Defensa en Profundidad:** Se aplica doble control de acceso:
  - En el MVC mediante el filtro `[AutorizacionSesion("Rol1", "Rol2")]` y renderizado condicional del menú en `_Layout.cshtml`.
  - En el API mediante `[Authorize(Roles = "...")]` en cada controlador y acción sensible (`POST`, `PUT`, `DELETE`).

---

## 4. Hashing Criptográfico de Contraseñas (PBKDF2-HMAC-SHA256)

### Decisión
Implementar el hashing de contraseñas con `Rfc2898DeriveBytes.Pbkdf2` usando **HMAC-SHA256**, **100,000 iteraciones**, salt aleatorio de **128 bits (16 bytes)** por usuario y comparación en tiempo constante (`CryptographicOperations.FixedTimeEquals`), encapsulado en `IServicioPasswordHasher` (registrado como `Singleton`).

### Justificación
- Resiste ataques de fuerza bruta, tablas arcoíris (*rainbow tables*) y ataques de temporización (*timing attacks*).
- No almacena jamás contraseñas en texto plano ni en la base de datos ni en las semillas (`MantenimientoSeedData`).

---

## 5. Gestión de Secretos (Zero Secrets en Repositorio)

### Decisión
No incluir cadenas de conexión con credenciales reales ni llaves privadas de producción en `appsettings.json` bajo control de versiones. Se utiliza **.NET User Secrets** (`dotnet user-secrets`) para el entorno de desarrollo local y variables de entorno para CI/CD.

---

## 6. Concurrencia Optimista (`RowVersion` + `ETag` / `If-Match`)

### Decisión
Configurar una columna nativa `ROW_VERSION` (`rowversion` / `byte[]`) en las entidades críticas de alta contención (`ORDEN_TB` y `REPUESTO_TB`) y exponer su valor codificado en Base64 como encabezado HTTP `ETag`. Las actualizaciones (`PUT`) validan el token recibido en el cuerpo o en el encabezado `If-Match`.

### Justificación
- Evita el problema de "actualizaciones perdidas" (*Lost Updates*) cuando dos técnicos o supervisores intentan modificar simultáneamente el stock de un repuesto o el estado de una misma orden, devolviendo `409 Conflict` en formato RFC 7807.

---

## 7. Predicción de Fallas a 30 Días con ML.NET

### Decisión
Integrar **ML.NET (`Microsoft.ML`)** en la capa `Flujo` (`ServicioPrediccionFalla`) utilizando un pipeline de clasificación binaria con normalización `NormalizeMinMax` y el entrenador `SdcaLogisticRegression`, manteniendo el modelo entrenado en memoria mediante `Lazy<ITransformer>` (*thread-safe*) y creando el `PredictionEngine` bajo demanda por solicitud.

### Justificación
- `ITransformer` en ML.NET es seguro para concurrencia entre múltiples hilos, evitando reentrenar el modelo en cada petición HTTP, mientras que el `PredictionEngine` se instancia de forma aislada por llamada para garantizar seguridad de hilos.
