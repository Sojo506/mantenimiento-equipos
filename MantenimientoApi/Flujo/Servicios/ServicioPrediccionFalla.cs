using Abstracciones.DTOs.Equipos;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Flujo.Servicios;

/// <summary>
/// Servicio de predicción de fallas a 30 días basado en ML.NET (Clasificación Binaria con Regresión Logística SDCA).
/// </summary>
public class ServicioPrediccionFalla : IServicioPrediccionFalla
{
    private static readonly Lazy<ITransformer> ModeloEntrenado = new(EntrenarModeloBase, isThreadSafe: true);
    private static readonly MLContext ContextoMl = new(seed: 42);

    private readonly IRepositorioEquipo _repositorioEquipo;
    private readonly IRepositorioFallaRegistrada _repositorioFalla;
    private readonly IRepositorioOrdenDeTrabajo _repositorioOrden;

    public ServicioPrediccionFalla(
        IRepositorioEquipo repositorioEquipo,
        IRepositorioFallaRegistrada repositorioFalla,
        IRepositorioOrdenDeTrabajo repositorioOrden)
    {
        _repositorioEquipo = repositorioEquipo;
        _repositorioFalla = repositorioFalla;
        _repositorioOrden = repositorioOrden;
    }

    public async Task<PrediccionFallaEquipoDto> PredecirFallaEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        string codigo = codigoEquipo.Trim().ToUpperInvariant();
        var equipo = await _repositorioEquipo.ObtenerPorCodigoAsync(codigo, cancellationToken);
        if (equipo == null)
        {
            throw new NoEncontradoException("Equipo", codigo);
        }

        int fallasUltimos6Meses = await _repositorioFalla.ContarFallasUltimosMesesAsync(codigo, 6, cancellationToken);
        var fallasEquipo = (await _repositorioFalla.ObtenerPorEquipoAsync(codigo, cancellationToken)).ToList();
        var ordenesEquipo = (await _repositorioOrden.ObtenerPorEquipoAsync(codigo, cancellationToken)).ToList();

        int ordenesCorrectivas = ordenesEquipo.Count(o => o.IdTipoOrden == 2);
        float horasAcumuladas = (float)ordenesEquipo.Sum(o => o.TiempoInvertido);
        float antiguedadMeses = (float)Math.Max(1, (DateTime.UtcNow - equipo.FechaAdquisicion).TotalDays / 30.0);
        float gravedadPromedio = fallasEquipo.Count > 0 ? (float)fallasEquipo.Average(f => f.IdTipoGravedad) : 1.0f;

        var entrada = new DatosEntradaFallaMl
        {
            AntiguedadMeses = antiguedadMeses,
            FallasUltimos6Meses = fallasUltimos6Meses,
            TotalOrdenesCorrectivas = ordenesCorrectivas,
            HorasMantenimiento = horasAcumuladas,
            GravedadPromedio = gravedadPromedio
        };

        float probabilidad;
        bool prediceFalla;

        using (var motor = ContextoMl.Model.CreatePredictionEngine<DatosEntradaFallaMl, ResultadoPrediccionMl>(ModeloEntrenado.Value))
        {
            var resultado = motor.Predict(entrada);
            probabilidad = Math.Clamp(resultado.Probability, 0.01f, 0.99f);
            prediceFalla = resultado.PredictedLabel || probabilidad >= 0.65f;
        }

        string nivelRiesgo = probabilidad switch
        {
            >= 0.70f => "Alto",
            >= 0.40f => "Medio",
            _ => "Bajo"
        };

        string recomendacion = nivelRiesgo switch
        {
            "Alto" => "Programar intervención preventiva inmediata y verificar stock de repuestos críticos.",
            "Medio" => "Monitorear parámetros operativos del equipo y adelantar inspección quincenal.",
            _ => "Mantener el calendario preventivo regular sin acciones extraordinarias."
        };

        return new PrediccionFallaEquipoDto
        {
            CodigoEquipo = equipo.CodigoEquipo,
            EquipoNombre = equipo.Nombre,
            EstadoActual = equipo.Estado?.Nombre ?? "Operativo",
            ProbabilidadFalla30Dias = probabilidad,
            PrediceFalla = prediceFalla,
            NivelRiesgo = nivelRiesgo,
            FallasUltimos6Meses = fallasUltimos6Meses,
            TotalOrdenesCorrectivas = ordenesCorrectivas,
            AntiguedadMeses = (float)Math.Round(antiguedadMeses, 1),
            HorasMantenimientoAcumuladas = (float)Math.Round(horasAcumuladas, 1),
            Recomendacion = recomendacion
        };
    }

    private static ITransformer EntrenarModeloBase()
    {
        var datosHistoricos = new List<DatosEntradaFallaMl>
        {
            // Casos de bajo riesgo (no fallaron en los 30 días siguientes)
            new() { AntiguedadMeses = 6,  FallasUltimos6Meses = 0, TotalOrdenesCorrectivas = 0, HorasMantenimiento = 2,  GravedadPromedio = 1.0f, FalloEn30Dias = false },
            new() { AntiguedadMeses = 12, FallasUltimos6Meses = 0, TotalOrdenesCorrectivas = 1, HorasMantenimiento = 4,  GravedadPromedio = 1.0f, FalloEn30Dias = false },
            new() { AntiguedadMeses = 18, FallasUltimos6Meses = 1, TotalOrdenesCorrectivas = 1, HorasMantenimiento = 6,  GravedadPromedio = 1.5f, FalloEn30Dias = false },
            new() { AntiguedadMeses = 24, FallasUltimos6Meses = 1, TotalOrdenesCorrectivas = 1, HorasMantenimiento = 8,  GravedadPromedio = 2.0f, FalloEn30Dias = false },
            new() { AntiguedadMeses = 30, FallasUltimos6Meses = 0, TotalOrdenesCorrectivas = 1, HorasMantenimiento = 5,  GravedadPromedio = 1.0f, FalloEn30Dias = false },
            new() { AntiguedadMeses = 36, FallasUltimos6Meses = 1, TotalOrdenesCorrectivas = 2, HorasMantenimiento = 10, GravedadPromedio = 2.0f, FalloEn30Dias = false },

            // Casos de riesgo medio/alto (sí presentaron falla en los 30 días siguientes)
            new() { AntiguedadMeses = 36, FallasUltimos6Meses = 2, TotalOrdenesCorrectivas = 3, HorasMantenimiento = 18, GravedadPromedio = 2.5f, FalloEn30Dias = true },
            new() { AntiguedadMeses = 48, FallasUltimos6Meses = 3, TotalOrdenesCorrectivas = 4, HorasMantenimiento = 25, GravedadPromedio = 3.0f, FalloEn30Dias = true },
            new() { AntiguedadMeses = 60, FallasUltimos6Meses = 3, TotalOrdenesCorrectivas = 5, HorasMantenimiento = 32, GravedadPromedio = 3.2f, FalloEn30Dias = true },
            new() { AntiguedadMeses = 72, FallasUltimos6Meses = 4, TotalOrdenesCorrectivas = 6, HorasMantenimiento = 40, GravedadPromedio = 3.5f, FalloEn30Dias = true },
            new() { AntiguedadMeses = 80, FallasUltimos6Meses = 5, TotalOrdenesCorrectivas = 7, HorasMantenimiento = 50, GravedadPromedio = 3.8f, FalloEn30Dias = true },
            new() { AntiguedadMeses = 96, FallasUltimos6Meses = 6, TotalOrdenesCorrectivas = 8, HorasMantenimiento = 65, GravedadPromedio = 4.0f, FalloEn30Dias = true }
        };

        IDataView dataView = ContextoMl.Data.LoadFromEnumerable(datosHistoricos);

        var pipeline = ContextoMl.Transforms.Concatenate(
                "Features",
                nameof(DatosEntradaFallaMl.AntiguedadMeses),
                nameof(DatosEntradaFallaMl.FallasUltimos6Meses),
                nameof(DatosEntradaFallaMl.TotalOrdenesCorrectivas),
                nameof(DatosEntradaFallaMl.HorasMantenimiento),
                nameof(DatosEntradaFallaMl.GravedadPromedio))
            .Append(ContextoMl.Transforms.NormalizeMinMax("Features"))
            .Append(ContextoMl.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: nameof(DatosEntradaFallaMl.FalloEn30Dias),
                featureColumnName: "Features"));

        return pipeline.Fit(dataView);
    }

    private sealed class DatosEntradaFallaMl
    {
        public float AntiguedadMeses { get; set; }
        public float FallasUltimos6Meses { get; set; }
        public float TotalOrdenesCorrectivas { get; set; }
        public float HorasMantenimiento { get; set; }
        public float GravedadPromedio { get; set; }
        public bool FalloEn30Dias { get; set; }
    }

    private sealed class ResultadoPrediccionMl
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
