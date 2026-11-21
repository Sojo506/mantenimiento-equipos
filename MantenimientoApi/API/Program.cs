
using API.Middleware;
using API.Restricciones;
using DA;
using Flujo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONTROLADORES Y JSON
// ==========================================

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;

        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
    });

// ==========================================
// CONFIGURACIÓN DE RUTAS
// ==========================================

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add(
        "codigoRuta",
        typeof(CodigoRutaConstraint));
});

// ==========================================
// MANEJO GLOBAL DE ERRORES
// ==========================================

builder.Services.AddExceptionHandler<ManejadorExcepcionesGlobal>();
builder.Services.AddProblemDetails();

// ==========================================
// CORS
// ==========================================

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

// ==========================================
// CONFIGURACIÓN JWT
// ==========================================

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException(
        "No se encontró JwtSettings:SecretKey en la configuración.");
}

if (Encoding.UTF8.GetByteCount(secretKey) < 32)
{
    throw new InvalidOperationException(
        "La clave JWT debe tener al menos 32 bytes.");
}

if (string.IsNullOrWhiteSpace(issuer))
{
    throw new InvalidOperationException(
        "No se encontró JwtSettings:Issuer en la configuración.");
}

if (string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException(
        "No se encontró JwtSettings:Audience en la configuración.");
}

var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(secretKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,

        ValidateAudience = true,
        ValidAudience = audience,

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,

        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Name,

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ==========================================
// CONFIGURACIÓN OPENAPI
// ==========================================

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Ingresa el token JWT."
        };

        document.Components.SecuritySchemes["Bearer"] = bearerScheme;

        return Task.CompletedTask;
    });
});

// ==========================================
// REGISTRO DE CAPAS
// ==========================================

builder.Services.AgregarCapaDatos(builder.Configuration);
builder.Services.AgregarCapaFlujo(builder.Configuration);

// ==========================================
// CONSTRUCCIÓN DE LA APLICACIÓN
// ==========================================

var app = builder.Build();

// ==========================================
// MIDDLEWARE
// ==========================================

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("PermitirTodo");

// ==========================================
// OPENAPI Y SCALAR
// ==========================================

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.WithTitle("Mantenimiento API — Sistema de Gestión")
           .WithTheme(ScalarTheme.DeepSpace);
});


// ==========================================
// AUTENTICACIÓN Y AUTORIZACIÓN
// ==========================================

app.UseAuthentication();
app.UseAuthorization();

// ==========================================
// CONTROLADORES
// ==========================================

app.MapControllers();

// ==========================================
// INICIO
// ==========================================

app.Run();
