using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using API.Tests.Infraestructura;
using Microsoft.EntityFrameworkCore;

namespace API.Tests.ReglasNegocio;

public class ReglasNegocioIntegracionTests : IClassFixture<FabricaApiPruebas>
{
    private readonly FabricaApiPruebas _fabrica;

    public ReglasNegocioIntegracionTests(FabricaApiPruebas fabrica)
    {
        _fabrica = fabrica;
    }

    [Fact]
    public async Task RN01_OrdenCriticaSinAtenderMasDelLimite_SeEscalaYQuedaRegistradaEnBitacora()
    {
        var admin = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoAdmin);
        var supervisor = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoSupervisor);

        var orden = await CrearOrdenAsync(admin, "EQ-006", "Critica");
        int numero = orden.GetProperty("numero").GetInt32();

        await RetrocederOrdenAsync(numero, horas: 10);

        var proceso = await supervisor.PostAsync("api/procesos/reglas-automaticas", null);
        Assert.Equal(HttpStatusCode.OK, proceso.StatusCode);
        var resultado = await ClienteApi.LeerJsonAsync(proceso);
        Assert.Contains(numero, resultado.GetProperty("numerosOrdenesEscaladas").EnumerateArray().Select(e => e.GetInt32()));

        var actualizada = await ClienteApi.LeerJsonAsync(await admin.GetAsync($"api/ordenes/{numero}"));
        Assert.Equal(12, actualizada.GetProperty("idEstado").GetInt32());

        var historial = (await ClienteApi.LeerJsonAsync(await admin.GetAsync($"api/ordenes/{numero}/historial")))
            .EnumerateArray().ToList();
        var ultimo = historial[^1];
        Assert.Equal("Abierta", ultimo.GetProperty("estadoAnterior").GetString());
        Assert.Equal("Escalada", ultimo.GetProperty("estadoNuevo").GetString());
        Assert.Contains("RN-01", ultimo.GetProperty("comentario").GetString());

        var segunda = await ClienteApi.LeerJsonAsync(await supervisor.PostAsync("api/procesos/reglas-automaticas", null));
        Assert.DoesNotContain(numero, segunda.GetProperty("numerosOrdenesEscaladas").EnumerateArray().Select(e => e.GetInt32()));
    }

    [Fact]
    public async Task RN01_OrdenEscaladaYReasignada_NoSeVuelveAEscalarHastaQueVenzaElNuevoPlazo()
    {
        var admin = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoAdmin);
        var supervisor = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoSupervisor);

        var orden = await CrearOrdenAsync(admin, "EQ-005", "Critica");
        int numero = orden.GetProperty("numero").GetInt32();
        await RetrocederOrdenAsync(numero, horas: 10);

        await supervisor.PostAsync("api/procesos/reglas-automaticas", null);
        var escalada = await ClienteApi.LeerJsonAsync(await admin.GetAsync($"api/ordenes/{numero}"));
        Assert.Equal(12, escalada.GetProperty("idEstado").GetInt32());

        var asignacion = await supervisor.PutAsJsonAsync($"api/ordenes/{numero}/asignacion", new
        {
            IdentificacionTecnico = ClienteApi.IdTecnico2,
            IdentificacionUsuario = ClienteApi.IdSupervisor,
            RowVersion = escalada.GetProperty("rowVersion").GetString()
        });
        Assert.Equal(HttpStatusCode.OK, asignacion.StatusCode);

        var proceso = await ClienteApi.LeerJsonAsync(await supervisor.PostAsync("api/procesos/reglas-automaticas", null));
        Assert.DoesNotContain(numero, proceso.GetProperty("numerosOrdenesEscaladas").EnumerateArray().Select(e => e.GetInt32()));

        var actual = await ClienteApi.LeerJsonAsync(await admin.GetAsync($"api/ordenes/{numero}"));
        Assert.Equal(8, actual.GetProperty("idEstado").GetInt32());
    }

    [Fact]
    public async Task RN01_OrdenCriticaRecienCreada_NoSeEscala()
    {
        var admin = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoAdmin);
        var supervisor = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoSupervisor);

        var orden = await CrearOrdenAsync(admin, "EQ-007", "Critica");
        int numero = orden.GetProperty("numero").GetInt32();

        await supervisor.PostAsync("api/procesos/reglas-automaticas", null);

        var actual = await ClienteApi.LeerJsonAsync(await admin.GetAsync($"api/ordenes/{numero}"));
        Assert.Equal(7, actual.GetProperty("idEstado").GetInt32());
    }

    [Fact]
    public async Task RN06_PlanVencido_GeneraOrdenPreventivaYReprogramaElPlan()
    {
        var supervisor = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoSupervisor);

        DateTime vencimiento = DateTime.UtcNow.Date.AddDays(-1);
        var crear = await supervisor.PostAsJsonAsync("api/planes", new { CodigoEquipo = "EQ-007", Frecuencia = 15, ProximaFecha = vencimiento });
        Assert.Equal(HttpStatusCode.Created, crear.StatusCode);
        var plan = await ClienteApi.LeerJsonAsync(crear);
        int idPlan = plan.GetProperty("idPlanMantenimiento").GetInt32();
        Assert.True(plan.GetProperty("vencido").GetBoolean());

        var proceso = await ClienteApi.LeerJsonAsync(await supervisor.PostAsync("api/procesos/reglas-automaticas", null));
        var generadas = proceso.GetProperty("numerosOrdenesGeneradas").EnumerateArray().Select(e => e.GetInt32()).ToList();

        var ordenesEquipo = await ClienteApi.LeerJsonAsync(await supervisor.GetAsync("api/ordenes?codigoEquipo=EQ-007&tamanoPagina=50"));
        var preventiva = ordenesEquipo.EnumerateArray().Single(o =>
            generadas.Contains(o.GetProperty("numero").GetInt32()) &&
            o.GetProperty("descripcion").GetString()!.Contains($"plan #{idPlan}"));
        Assert.Equal(1, preventiva.GetProperty("idTipoOrden").GetInt32());
        Assert.Equal(7, preventiva.GetProperty("idEstado").GetInt32());

        var planActual = await ClienteApi.LeerJsonAsync(await supervisor.GetAsync($"api/planes/{idPlan}"));
        Assert.Equal(vencimiento.AddDays(15), planActual.GetProperty("proximaFecha").GetDateTime());
        Assert.False(planActual.GetProperty("vencido").GetBoolean());
    }

    [Fact]
    public async Task Planes_EquipoConPlanExistente_Devuelve409()
    {
        var supervisor = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoSupervisor);

        var respuesta = await supervisor.PostAsJsonAsync("api/planes", new { CodigoEquipo = "EQ-001", Frecuencia = 10, ProximaFecha = DateTime.UtcNow.AddDays(3) });

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        Assert.Equal("application/problem+json", respuesta.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Planes_SinTokenDevuelve401_YTecnicoNoPuedeCrearDevuelve403()
    {
        var anonimo = _fabrica.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync("api/planes")).StatusCode);

        var tecnico = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoTecnico);
        Assert.Equal(HttpStatusCode.OK, (await tecnico.GetAsync("api/planes")).StatusCode);

        var crear = await tecnico.PostAsJsonAsync("api/planes", new { CodigoEquipo = "EQ-006", Frecuencia = 10, ProximaFecha = DateTime.UtcNow.AddDays(3) });
        Assert.Equal(HttpStatusCode.Forbidden, crear.StatusCode);
    }

    [Fact]
    public async Task Concurrencia_AsignarRepuestoConVersionDesactualizada_Devuelve409YNoDescuentaStock()
    {
        var admin = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoAdmin);
        var supervisor = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoSupervisor);

        var orden = await CrearOrdenAsync(admin, "EQ-001", "Media");
        int numero = orden.GetProperty("numero").GetInt32();
        string versionTecnico = orden.GetProperty("rowVersion").GetString()!;

        var cambio = await supervisor.PutAsJsonAsync($"api/ordenes/{numero}/estado", new
        {
            NuevoEstado = 12,
            IdentificacionUsuario = ClienteApi.IdSupervisor,
            Comentario = "Escalada manualmente por el supervisor",
            RowVersion = versionTecnico
        });
        Assert.Equal(HttpStatusCode.OK, cambio.StatusCode);
        string versionNueva = (await ClienteApi.LeerJsonAsync(cambio)).GetProperty("rowVersion").GetString()!;
        Assert.NotEqual(versionTecnico, versionNueva);
        Assert.Equal($"\"{versionNueva}\"", cambio.Headers.ETag?.Tag);

        int stockAntes = await ObtenerStockAsync(admin, "REP-002");

        using var solicitud = new HttpRequestMessage(HttpMethod.Post, $"api/ordenes/{numero}/repuestos")
        {
            Content = JsonContent.Create(new { CodigoRepuesto = "REP-002", Cantidad = 2 })
        };
        solicitud.Headers.TryAddWithoutValidation("If-Match", $"\"{versionTecnico}\"");
        var conflicto = await admin.SendAsync(solicitud);

        Assert.Equal(HttpStatusCode.Conflict, conflicto.StatusCode);
        var problema = await ClienteApi.LeerJsonAsync(conflicto);
        Assert.Equal(409, problema.GetProperty("status").GetInt32());
        Assert.Contains("modificada por otro usuario", problema.GetProperty("detail").GetString());
        Assert.Equal(stockAntes, await ObtenerStockAsync(admin, "REP-002"));

        var reintento = await admin.PostAsJsonAsync($"api/ordenes/{numero}/repuestos",
            new { CodigoRepuesto = "REP-002", Cantidad = 2, RowVersion = versionNueva });
        Assert.Equal(HttpStatusCode.OK, reintento.StatusCode);
        Assert.Equal(stockAntes - 2, await ObtenerStockAsync(admin, "REP-002"));
    }

    [Fact]
    public async Task Concurrencia_DosUsuariosCambianEstadoConLaMismaVersion_ElSegundoRecibe409()
    {
        var admin = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoAdmin);
        var supervisor = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoSupervisor);

        var orden = await CrearOrdenAsync(admin, "EQ-002", "Alta");
        int numero = orden.GetProperty("numero").GetInt32();
        string version = orden.GetProperty("rowVersion").GetString()!;

        var primero = await supervisor.PutAsJsonAsync($"api/ordenes/{numero}/estado", new
        {
            NuevoEstado = 12,
            IdentificacionUsuario = ClienteApi.IdSupervisor,
            Comentario = "Primer cambio",
            RowVersion = version
        });
        var segundo = await admin.PutAsJsonAsync($"api/ordenes/{numero}/estado", new
        {
            NuevoEstado = 12,
            IdentificacionUsuario = ClienteApi.IdSupervisor,
            Comentario = "Segundo cambio con versión vieja",
            RowVersion = version
        });

        Assert.Equal(HttpStatusCode.OK, primero.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segundo.StatusCode);

        var historial = (await ClienteApi.LeerJsonAsync(await admin.GetAsync($"api/ordenes/{numero}/historial"))).EnumerateArray().ToList();
        Assert.Equal(2, historial.Count);
    }

    [Fact]
    public async Task RN02_CerrarSinTiempoNiRepuestos_Devuelve400ConProblemDetails()
    {
        var admin = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoAdmin);

        var orden = await CrearOrdenAsync(admin, "EQ-003", "Baja");
        int numero = orden.GetProperty("numero").GetInt32();

        var respuesta = await admin.PutAsJsonAsync($"api/ordenes/{numero}/estado", new
        {
            NuevoEstado = 11,
            IdentificacionUsuario = ClienteApi.IdSupervisor,
            Comentario = "Intento de cierre incompleto",
            TiempoInvertidoFinal = 0,
            RowVersion = orden.GetProperty("rowVersion").GetString()
        });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        var problema = await ClienteApi.LeerJsonAsync(respuesta);
        Assert.Contains("RN-02", problema.GetProperty("detail").GetString());

        var actual = await ClienteApi.LeerJsonAsync(await admin.GetAsync($"api/ordenes/{numero}"));
        Assert.Equal(7, actual.GetProperty("idEstado").GetInt32());
    }

    [Fact]
    public async Task Asignacion_SupervisorAsignaTecnicoAOrdenAbierta_QuedaAsignadaYEnBitacora()
    {
        var admin = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoAdmin);
        var supervisor = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoSupervisor);

        var orden = await CrearOrdenAsync(admin, "EQ-004", "Alta");
        int numero = orden.GetProperty("numero").GetInt32();

        var respuesta = await supervisor.PutAsJsonAsync($"api/ordenes/{numero}/asignacion", new
        {
            IdentificacionTecnico = ClienteApi.IdTecnico2,
            IdentificacionUsuario = ClienteApi.IdSupervisor,
            RowVersion = orden.GetProperty("rowVersion").GetString()
        });

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var asignada = await ClienteApi.LeerJsonAsync(respuesta);
        Assert.Equal(8, asignada.GetProperty("idEstado").GetInt32());
        Assert.Equal(ClienteApi.IdTecnico2, asignada.GetProperty("identificacionTecnico").GetString());

        var invalida = await supervisor.PutAsJsonAsync($"api/ordenes/{numero}/asignacion", new
        {
            IdentificacionTecnico = ClienteApi.IdSolicitante,
            IdentificacionUsuario = ClienteApi.IdSupervisor,
            RowVersion = asignada.GetProperty("rowVersion").GetString()
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalida.StatusCode);
    }

    [Theory]
    [InlineData("api/ordenes?tamanoPagina=0", "tamanoPagina")]
    [InlineData("api/ordenes?pagina=0", "pagina")]
    [InlineData("api/ordenes?prioridad=Urgente", "prioridad")]
    [InlineData("api/ordenes?idEstado=3", "idEstado")]
    [InlineData("api/ordenes?ordenarPor=color", "ordenarPor")]
    public async Task Ordenes_ListadoConParametrosInvalidos_Devuelve400ConErrorDeCampo(string url, string campo)
    {
        var admin = await ClienteApi.CrearClienteAutenticadoAsync(_fabrica, ClienteApi.CorreoAdmin);

        var respuesta = await admin.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        var problema = await ClienteApi.LeerJsonAsync(respuesta);
        Assert.True(problema.GetProperty("errors").TryGetProperty(campo, out _));
    }

    private static async Task<JsonElement> CrearOrdenAsync(HttpClient cliente, string codigoEquipo, string prioridad)
    {
        var respuesta = await cliente.PostAsJsonAsync("api/ordenes", new
        {
            CodigoEquipo = codigoEquipo,
            IdentificacionSolicitante = ClienteApi.IdSolicitante,
            IdTipoOrden = 1,
            Prioridad = prioridad,
            Descripcion = $"Orden de prueba de integración ({prioridad}) para {codigoEquipo}"
        });
        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        return await ClienteApi.LeerJsonAsync(respuesta);
    }

    private Task RetrocederOrdenAsync(int numero, int horas) =>
        _fabrica.EjecutarEnBaseAsync(async db =>
        {
            var entidad = await db.OrdenesDeTrabajo.SingleAsync(o => o.Numero == numero);
            entidad.FechaCreacion = entidad.FechaCreacion.AddHours(-horas);
            foreach (var evento in db.BitacorasOrden.Where(b => b.Numero == numero))
                evento.Fecha = evento.Fecha.AddHours(-horas);
            await db.SaveChangesAsync();
        });

    private static async Task<int> ObtenerStockAsync(HttpClient cliente, string codigoRepuesto)
    {
        var repuesto = await ClienteApi.LeerJsonAsync(await cliente.GetAsync($"api/repuestos/{codigoRepuesto}"));
        return repuesto.GetProperty("stock").GetInt32();
    }
}
