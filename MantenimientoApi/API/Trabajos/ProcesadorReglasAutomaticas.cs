using Abstracciones.Configuracion;
using Abstracciones.Interfaces.Flujo;
using Microsoft.Extensions.Options;

namespace API.Trabajos;

public class ProcesadorReglasAutomaticas : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<ReglasNegocioOptions> _options;
    private readonly ILogger<ProcesadorReglasAutomaticas> _logger;

    public ProcesadorReglasAutomaticas(
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<ReglasNegocioOptions> options,
        ILogger<ProcesadorReglasAutomaticas> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.CurrentValue.ProcesoAutomaticoHabilitado)
        {
            _logger.LogInformation("Procesador de reglas automáticas (RN-01/RN-06) deshabilitado por configuración.");
            return;
        }

        var intervalo = TimeSpan.FromMinutes(Math.Max(1, _options.CurrentValue.IntervaloMinutosProcesoAutomatico));
        _logger.LogInformation("Procesador de reglas automáticas iniciado. Intervalo: {Intervalo}.", intervalo);

        using var temporizador = new PeriodicTimer(intervalo);
        do
        {
            await EjecutarCicloAsync(stoppingToken);
        }
        while (await EsperarSiguienteTickAsync(temporizador, stoppingToken));
    }

    private async Task EjecutarCicloAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var proceso = scope.ServiceProvider.GetRequiredService<IServicioProcesoAutomatico>();
            var resultado = await proceso.EjecutarAsync(stoppingToken);

            if (resultado.OrdenesEscaladas > 0 || resultado.OrdenesPreventivasGeneradas > 0)
            {
                _logger.LogInformation(
                    "Reglas automáticas: {Escaladas} órdenes escaladas (RN-01) [{NumerosEscaladas}], {Generadas} órdenes preventivas generadas (RN-06) [{NumerosGeneradas}].",
                    resultado.OrdenesEscaladas, string.Join(", ", resultado.NumerosOrdenesEscaladas),
                    resultado.OrdenesPreventivasGeneradas, string.Join(", ", resultado.NumerosOrdenesGeneradas));
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar las reglas automáticas RN-01/RN-06.");
        }
    }

    private static async Task<bool> EsperarSiguienteTickAsync(PeriodicTimer temporizador, CancellationToken stoppingToken)
    {
        try
        {
            return await temporizador.WaitForNextTickAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }
}
