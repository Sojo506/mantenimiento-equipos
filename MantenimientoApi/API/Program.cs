using System.Text.Json.Serialization;
using API.Middleware;
using API.Restricciones;
using DA;
using Flujo;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de serialización JSON y controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Restricción de ruta personalizada para códigos de equipo y repuesto
builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("codigoRuta", typeof(CodigoRutaConstraint));
});

// Manejo global de excepciones RFC 7807 ProblemDetails
builder.Services.AddExceptionHandler<ManejadorExcepcionesGlobal>();
builder.Services.AddProblemDetails();

// Política CORS permisiva para desarrollo y consumo del frontend MVC
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("X-Pagination", "ETag");
    });
});

// OpenAPI y documentación interactiva
builder.Services.AddOpenApi();

// Registro de dependencias de la capa de acceso a datos (DA)
builder.Services.AgregarCapaDatos(builder.Configuration);

// Registro de dependencias de la capa de lógica de negocio (Flujo)
builder.Services.AgregarCapaFlujo(builder.Configuration);

var app = builder.Build();

// Pipeline de manejo de errores
app.UseExceptionHandler();
app.UseStatusCodePages();

// Archivos estáticos y página de inicio
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("PermitirTodo");

// Documentación de OpenAPI y Scalar
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("Mantenimiento API — Sistema de Gestión")
           .WithTheme(ScalarTheme.DeepSpace);
});

// -------------------------------------------------------------
// ENDPOINTS DE TABLERO Y MÉTRICAS (Minimal APIs)
// -------------------------------------------------------------

var grupoTablero = app.MapGroup("/api/tablero")
    .WithTags("Tablero de Control");

grupoTablero.MapGet("/equipos-por-estado", async (MantenimientoDbContext db, CancellationToken ct) =>
{
    var resultado = await db.Equipos
        .AsNoTracking()
        .GroupBy(e => new { e.IdEstado, e.Estado!.Nombre })
        .Select(g => new
        {
            g.Key.IdEstado,
            Estado = g.Key.Nombre,
            Cantidad = g.Count()
        })
        .ToListAsync(ct);

    return Results.Ok(resultado);
})
.WithName("ObtenerEquiposPorEstado")
.WithSummary("Obtiene el conteo consolidado de equipos agrupados por estado operativo.")
.Produces(StatusCodes.Status200OK);

grupoTablero.MapGet("/ordenes-por-prioridad", async (MantenimientoDbContext db, CancellationToken ct) =>
{
    var resultado = await db.OrdenesDeTrabajo
        .AsNoTracking()
        .GroupBy(o => o.Prioridad)
        .Select(g => new
        {
            Prioridad = g.Key,
            Total = g.Count(),
            Activas = g.Count(o => o.IdEstado != 11 && o.IdEstado != 10)
        })
        .ToListAsync(ct);

    return Results.Ok(resultado);
})
.WithName("ObtenerOrdenesPorPrioridad")
.WithSummary("Obtiene la distribución de órdenes de trabajo agrupadas por prioridad y estado activo.")
.Produces(StatusCodes.Status200OK);

grupoTablero.MapGet("/repuestos-bajo-minimo", async (MantenimientoDbContext db, CancellationToken ct) =>
{
    var criticos = await db.Repuestos
        .AsNoTracking()
        .Where(r => r.Stock <= r.StockMinimo)
        .Select(r => new
        {
            r.CodigoRepuesto,
            r.Nombre,
            r.Stock,
            r.StockMinimo,
            Deficit = r.StockMinimo - r.Stock,
            r.CostoUnitario
        })
        .ToListAsync(ct);

    return Results.Ok(new
    {
        TotalCriticos = criticos.Count,
        Items = criticos
    });
})
.WithName("ObtenerRepuestosBajoMinimo")
.WithSummary("Consulta los repuestos que se encuentran en estado crítico de existencias (stock <= mínimo).")
.Produces(StatusCodes.Status200OK);

// Endpoint de diagnóstico y salud del servicio
app.MapGet("/api/salud", async (MantenimientoDbContext db, CancellationToken ct) =>
{
    bool baseDatosConectada = await db.Database.CanConnectAsync(ct);

    return Results.Ok(new
    {
        Estado = "Operativo",
        Version = "1.0.0 (Avance 1)",
        BaseDatos = baseDatosConectada ? "Conectada" : "Desconectada",
        FechaUtc = DateTime.UtcNow,
        Entorno = app.Environment.EnvironmentName
    });
})
.WithTags("Diagnóstico")
.WithSummary("Verifica el estado de operatividad de la API y la conectividad con la base de datos.");

app.UseAuthorization();

app.MapControllers();

app.Run();
