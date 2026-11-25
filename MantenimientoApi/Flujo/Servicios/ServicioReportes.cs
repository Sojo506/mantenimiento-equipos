using System.Globalization;
using System.Text;
using Abstracciones.DTOs.Reportes;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;

namespace Flujo.Servicios;

/// <summary>
/// Implementación del servicio de reportes analíticos y generación de archivos CSV (RFC 4180 con UTF-8 BOM).
/// </summary>
public class ServicioReportes : IServicioReportes
{
    private readonly IRepositorioReportes _repositorioReportes;

    public ServicioReportes(IRepositorioReportes repositorioReportes)
    {
        _repositorioReportes = repositorioReportes;
    }

    public async Task<IEnumerable<ReporteConsumoRepuestoDto>> ObtenerConsumoRepuestosAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        CancellationToken cancellationToken = default)
    {
        if (fechaInicio > fechaFin)
        {
            throw new ReglaNegocioException("La fecha de inicio no puede ser posterior a la fecha final del reporte.");
        }

        return await _repositorioReportes.ObtenerConsumoRepuestosPorPeriodoAsync(fechaInicio, fechaFin, cancellationToken);
    }

    public async Task<byte[]> GenerarCsvConsumoRepuestosAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        CancellationToken cancellationToken = default)
    {
        var datos = await ObtenerConsumoRepuestosAsync(fechaInicio, fechaFin, cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("CodigoRepuesto,NombreRepuesto,CantidadTotalConsumida,CostoTotalAcumulado,NumeroDeOrdenes");

        foreach (var r in datos)
        {
            sb.AppendLine(string.Join(",",
                EscaparCsv(r.CodigoRepuesto),
                EscaparCsv(r.NombreRepuesto),
                r.CantidadTotalConsumida.ToString(CultureInfo.InvariantCulture),
                r.CostoTotalAcumulado.ToString("F2", CultureInfo.InvariantCulture),
                r.NumeroDeOrdenes.ToString(CultureInfo.InvariantCulture)));
        }

        return CodificarConBom(sb.ToString());
    }

    public async Task<IEnumerable<ReporteOrdenesEstadoDto>> ObtenerReporteOrdenesAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio > fechaFin)
        {
            throw new ReglaNegocioException("La fecha de inicio no puede ser posterior a la fecha final del reporte.");
        }

        return await _repositorioReportes.ObtenerReporteOrdenesAsync(fechaInicio, fechaFin, cancellationToken);
    }

    public async Task<byte[]> GenerarCsvOrdenesAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        var datos = await ObtenerReporteOrdenesAsync(fechaInicio, fechaFin, cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("NumeroOrden,CodigoEquipo,EquipoNombre,TipoOrden,Prioridad,Estado,TecnicoAsignado,FechaCreacion,FechaLimite,TiempoInvertidoHoras,CostoRepuestos");

        foreach (var o in datos)
        {
            sb.AppendLine(string.Join(",",
                o.NumeroOrden.ToString(CultureInfo.InvariantCulture),
                EscaparCsv(o.CodigoEquipo),
                EscaparCsv(o.EquipoNombre),
                EscaparCsv(o.TipoOrden),
                EscaparCsv(o.Prioridad),
                EscaparCsv(o.Estado),
                EscaparCsv(o.TecnicoAsignado),
                o.FechaCreacion.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                o.FechaLimite.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                o.TiempoInvertidoHoras.ToString("F2", CultureInfo.InvariantCulture),
                o.CostoRepuestos.ToString("F2", CultureInfo.InvariantCulture)));
        }

        return CodificarConBom(sb.ToString());
    }

    public Task<IEnumerable<ReporteEquipoCriticoDto>> ObtenerReporteEquiposCriticosAsync(
        CancellationToken cancellationToken = default) =>
        _repositorioReportes.ObtenerReporteEquiposCriticosAsync(cancellationToken);

    public async Task<byte[]> GenerarCsvEquiposCriticosAsync(CancellationToken cancellationToken = default)
    {
        var datos = await ObtenerReporteEquiposCriticosAsync(cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("CodigoEquipo,NombreEquipo,TipoEquipo,Ubicacion,EstadoActual,TotalFallasRegistradas,OrdenesAbiertas,OrdenesCerradas,HorasMantenimientoTotal");

        foreach (var e in datos)
        {
            sb.AppendLine(string.Join(",",
                EscaparCsv(e.CodigoEquipo),
                EscaparCsv(e.NombreEquipo),
                EscaparCsv(e.TipoEquipo),
                EscaparCsv(e.Ubicacion),
                EscaparCsv(e.EstadoActual),
                e.TotalFallasRegistradas.ToString(CultureInfo.InvariantCulture),
                e.OrdenesAbiertas.ToString(CultureInfo.InvariantCulture),
                e.OrdenesCerradas.ToString(CultureInfo.InvariantCulture),
                e.HorasMantenimientoTotal.ToString("F2", CultureInfo.InvariantCulture)));
        }

        return CodificarConBom(sb.ToString());
    }

    public Task<IEnumerable<ReporteProductividadTecnicoDto>> ObtenerReporteProductividadTecnicosAsync(
        CancellationToken cancellationToken = default) =>
        _repositorioReportes.ObtenerReporteProductividadTecnicosAsync(cancellationToken);

    public async Task<byte[]> GenerarCsvProductividadTecnicosAsync(CancellationToken cancellationToken = default)
    {
        var datos = await ObtenerReporteProductividadTecnicosAsync(cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("Identificacion,NombreTecnico,Especialidad,Disponible,OrdenesEnProceso,OrdenesAsignadas,OrdenesCerradas,HorasTotalesRegistradas");

        foreach (var t in datos)
        {
            sb.AppendLine(string.Join(",",
                EscaparCsv(t.Identificacion),
                EscaparCsv(t.NombreTecnico),
                EscaparCsv(t.Especialidad),
                t.Disponible ? "Si" : "No",
                t.OrdenesEnProceso.ToString(CultureInfo.InvariantCulture),
                t.OrdenesAsignadas.ToString(CultureInfo.InvariantCulture),
                t.OrdenesCerradas.ToString(CultureInfo.InvariantCulture),
                t.HorasTotalesRegistradas.ToString("F2", CultureInfo.InvariantCulture)));
        }

        return CodificarConBom(sb.ToString());
    }

    private static string EscaparCsv(string? valor)
    {
        if (string.IsNullOrEmpty(valor))
            return string.Empty;

        if (valor.Contains(',') || valor.Contains('"') || valor.Contains('\n') || valor.Contains('\r'))
        {
            return $"\"{valor.Replace("\"", "\"\"")}\"";
        }

        return valor;
    }

    private static byte[] CodificarConBom(string contenido)
    {
        var utf8Bom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        var preambulo = utf8Bom.GetPreamble();
        var cuerpo = utf8Bom.GetBytes(contenido);
        var resultado = new byte[preambulo.Length + cuerpo.Length];
        Buffer.BlockCopy(preambulo, 0, resultado, 0, preambulo.Length);
        Buffer.BlockCopy(cuerpo, 0, resultado, preambulo.Length, cuerpo.Length);
        return resultado;
    }
}
