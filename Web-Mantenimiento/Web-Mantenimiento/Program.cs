using Web_Mantenimiento.Http;
using Web_Mantenimiento.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

// Sesiones
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".WebMantenimiento.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Handler JWT
builder.Services.AddTransient<JwtDelegatingHandler>();

// Configuración de la API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException(
        "No se configuró ApiSettings:BaseUrl.");
}

builder.Services.AddHttpClient<ApiAuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient("MantenimientoApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<JwtDelegatingHandler>();
builder.Services.AddScoped<ApiUsuarioService>();
builder.Services.AddScoped<ApiCatalogoService>();
builder.Services.AddScoped<ApiEquipoService>();
builder.Services.AddScoped<ApiRepuestoService>();
builder.Services.AddScoped<ApiOrdenService>();
builder.Services.AddScoped<ApiTableroService>();
builder.Services.AddScoped<ApiFallaService>();
builder.Services.AddScoped<ApiReporteService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();