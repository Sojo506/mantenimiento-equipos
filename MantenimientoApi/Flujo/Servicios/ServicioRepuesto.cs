using Abstracciones.DTOs.Repuestos;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;

namespace Flujo.Servicios;

/// <summary>
/// Implementación de la lógica de negocio para la gestión de repuestos e inventario,
/// incluyendo detección de stock crítico y validación de concurrencia optimista (RN-05).
/// </summary>
public class ServicioRepuesto : IServicioRepuesto
{
    private readonly IRepositorioRepuesto _repositorioRepuesto;

    public ServicioRepuesto(IRepositorioRepuesto repositorioRepuesto)
    {
        _repositorioRepuesto = repositorioRepuesto;
    }

    public async Task<IEnumerable<RepuestoDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        var repuestos = await _repositorioRepuesto.ObtenerTodosAsync(cancellationToken);
        return repuestos.Select(MapearADto);
    }

    public async Task<IEnumerable<RepuestoDto>> ObtenerBajoStockMinimoAsync(CancellationToken cancellationToken = default)
    {
        var repuestos = await _repositorioRepuesto.ObtenerBajoStockMinimoAsync(cancellationToken);
        return repuestos.Select(MapearADto);
    }

    public async Task<RepuestoDto> ObtenerPorCodigoAsync(string codigoRepuesto, CancellationToken cancellationToken = default)
    {
        var repuesto = await _repositorioRepuesto.ObtenerPorCodigoAsync(codigoRepuesto, cancellationToken);
        if (repuesto == null)
        {
            throw new NoEncontradoException("Repuesto", codigoRepuesto);
        }

        return MapearADto(repuesto);
    }

    public async Task<RepuestoDto> CrearAsync(CrearRepuestoDto dto, CancellationToken cancellationToken = default)
    {
        if (await _repositorioRepuesto.ExisteCodigoAsync(dto.CodigoRepuesto, cancellationToken))
        {
            throw new ReglaNegocioException($"Ya existe un repuesto registrado con el código '{dto.CodigoRepuesto}'.");
        }

        if (dto.Stock < 0)
        {
            throw new ReglaNegocioException("El stock de un repuesto no puede ser negativo (RN-05).");
        }

        var repuesto = new Repuesto
        {
            CodigoRepuesto = dto.CodigoRepuesto.Trim().ToUpper(),
            Nombre = dto.Nombre.Trim(),
            Stock = dto.Stock,
            StockMinimo = dto.StockMinimo,
            CostoUnitario = dto.CostoUnitario,
            IdEstado = dto.IdEstado
        };

        await _repositorioRepuesto.AgregarAsync(repuesto, cancellationToken);
        await _repositorioRepuesto.GuardarCambiosAsync(cancellationToken);

        return await ObtenerPorCodigoAsync(repuesto.CodigoRepuesto, cancellationToken);
    }

    public async Task<RepuestoDto> ActualizarAsync(string codigoRepuesto, ActualizarRepuestoDto dto, CancellationToken cancellationToken = default)
    {
        var repuesto = await _repositorioRepuesto.ObtenerPorCodigoAsync(codigoRepuesto, cancellationToken);
        if (repuesto == null)
        {
            throw new NoEncontradoException("Repuesto", codigoRepuesto);
        }

        if (dto.Stock < 0)
        {
            throw new ReglaNegocioException("El stock de un repuesto no puede ser negativo (RN-05).");
        }

        // Validación de concurrencia optimista mediante el token RowVersion
        if (!string.IsNullOrWhiteSpace(dto.RowVersion))
        {
            try
            {
                byte[] clientToken = Convert.FromBase64String(dto.RowVersion);
                if (repuesto.RowVersion != null && repuesto.RowVersion.Length > 0 && !clientToken.SequenceEqual(repuesto.RowVersion))
                {
                    throw new ConflictoException($"El repuesto '{codigoRepuesto}' fue modificado concurrentemente por otro usuario. Por favor recargue los datos actuales.");
                }
            }
            catch (FormatException)
            {
                throw new ReglaNegocioException("El formato del token de concurrencia (RowVersion) no es válido.");
            }
        }

        repuesto.Nombre = dto.Nombre.Trim();
        repuesto.Stock = dto.Stock;
        repuesto.StockMinimo = dto.StockMinimo;
        repuesto.CostoUnitario = dto.CostoUnitario;
        repuesto.IdEstado = dto.IdEstado;

        await _repositorioRepuesto.ActualizarAsync(repuesto, cancellationToken);
        await _repositorioRepuesto.GuardarCambiosAsync(cancellationToken);

        return await ObtenerPorCodigoAsync(codigoRepuesto, cancellationToken);
    }

    public async Task EliminarAsync(string codigoRepuesto, CancellationToken cancellationToken = default)
    {
        var repuesto = await _repositorioRepuesto.ObtenerPorCodigoAsync(codigoRepuesto, cancellationToken);
        if (repuesto == null)
        {
            throw new NoEncontradoException("Repuesto", codigoRepuesto);
        }

        await _repositorioRepuesto.EliminarAsync(codigoRepuesto, cancellationToken);
        await _repositorioRepuesto.GuardarCambiosAsync(cancellationToken);
    }

    private static RepuestoDto MapearADto(Repuesto r)
    {
        return new RepuestoDto
        {
            CodigoRepuesto = r.CodigoRepuesto,
            Nombre = r.Nombre,
            Stock = r.Stock,
            StockMinimo = r.StockMinimo,
            CostoUnitario = r.CostoUnitario,
            IdEstado = r.IdEstado,
            EstadoNombre = r.Estado?.Nombre ?? string.Empty,
            RowVersion = r.RowVersion != null && r.RowVersion.Length > 0 ? Convert.ToBase64String(r.RowVersion) : string.Empty
        };
    }
}
