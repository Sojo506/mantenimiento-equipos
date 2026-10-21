using Abstracciones.Configuracion;
using Abstracciones.DTOs.Ordenes;
using Abstracciones.Entidades;
using Abstracciones.Excepciones;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Microsoft.Extensions.Options;

namespace Flujo.Servicios;

/// <summary>
/// Implementación de la lógica de negocio para la gestión integral de órdenes de trabajo,
/// aplicando las reglas RN-01 a RN-09, máquina de estados y trazabilidad inmutable en bitácora.
/// </summary>
public class ServicioOrdenDeTrabajo : IServicioOrdenDeTrabajo
{
    private readonly IRepositorioOrdenDeTrabajo _repositorioOrden;
    private readonly IRepositorioEquipo _repositorioEquipo;
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IRepositorioRepuesto _repositorioRepuesto;
    private readonly IRepositorioBitacoraOrden _repositorioBitacora;
    private readonly ReglasNegocioOptions _options;

    // Transiciones permitidas en la máquina de estados:
    // 7: Abierta, 8: Asignada, 9: En Proceso, 10: En Espera de Repuesto, 11: Cerrada, 12: Escalada
    private static readonly Dictionary<int, int[]> TransicionesValidas = new()
    {
        { 7, new[] { 8, 11, 12 } },       // Abierta -> Asignada, Cerrada, Escalada
        { 8, new[] { 7, 9, 12 } },          // Asignada -> Abierta, En Proceso, Escalada
        { 9, new[] { 10, 11, 12 } },       // En Proceso -> En Espera de Repuesto, Cerrada, Escalada
        { 10, new[] { 9, 12 } },           // En Espera de Repuesto -> En Proceso, Escalada
        { 12, new[] { 8, 9, 11 } },        // Escalada -> Asignada, En Proceso, Cerrada
        { 11, Array.Empty<int>() }         // Cerrada -> Inmutable (terminal, RN-09)
    };

    private static readonly Dictionary<int, string> NombresEstados = new()
    {
        { 7, "Abierta" },
        { 8, "Asignada" },
        { 9, "En Proceso" },
        { 10, "En Espera de Repuesto" },
        { 11, "Cerrada" },
        { 12, "Escalada" }
    };

    public ServicioOrdenDeTrabajo(
        IRepositorioOrdenDeTrabajo repositorioOrden,
        IRepositorioEquipo repositorioEquipo,
        IRepositorioUsuario repositorioUsuario,
        IRepositorioRepuesto repositorioRepuesto,
        IRepositorioBitacoraOrden repositorioBitacora,
        IOptions<ReglasNegocioOptions> options)
    {
        _repositorioOrden = repositorioOrden;
        _repositorioEquipo = repositorioEquipo;
        _repositorioUsuario = repositorioUsuario;
        _repositorioRepuesto = repositorioRepuesto;
        _repositorioBitacora = repositorioBitacora;
        _options = options.Value;
    }

    public async Task<IEnumerable<OrdenDeTrabajoDto>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        var ordenes = await _repositorioOrden.ObtenerTodasAsync(cancellationToken);
        return ordenes.Select(MapearADto);
    }

    public async Task<OrdenDeTrabajoDto> ObtenerPorNumeroAsync(int numero, CancellationToken cancellationToken = default)
    {
        var orden = await _repositorioOrden.ObtenerPorNumeroAsync(numero, cancellationToken);
        if (orden == null)
        {
            throw new NoEncontradoException("OrdenDeTrabajo", numero);
        }

        return MapearADto(orden);
    }

    public async Task<IEnumerable<OrdenDeTrabajoDto>> ObtenerPorEquipoAsync(string codigoEquipo, CancellationToken cancellationToken = default)
    {
        var ordenes = await _repositorioOrden.ObtenerPorEquipoAsync(codigoEquipo, cancellationToken);
        return ordenes.Select(MapearADto);
    }

    public async Task<IEnumerable<OrdenDeTrabajoDto>> ObtenerPorTecnicoAsync(string identificacionTecnico, CancellationToken cancellationToken = default)
    {
        var ordenes = await _repositorioOrden.ObtenerPorTecnicoAsync(identificacionTecnico, cancellationToken);
        return ordenes.Select(MapearADto);
    }

    public async Task<IEnumerable<OrdenDeTrabajoDto>> ObtenerPorSolicitanteAsync(string identificacionSolicitante, CancellationToken cancellationToken = default)
    {
        var ordenes = await _repositorioOrden.ObtenerPorSolicitanteAsync(identificacionSolicitante, cancellationToken);
        return ordenes.Select(MapearADto);
    }

    public async Task<OrdenDeTrabajoDto> CrearAsync(CrearOrdenDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Validar existencia del equipo y regla de equipo dado de baja (RN-04)
        var equipo = await _repositorioEquipo.ObtenerPorCodigoAsync(dto.CodigoEquipo, cancellationToken);
        if (equipo == null)
        {
            throw new NoEncontradoException("Equipo", dto.CodigoEquipo);
        }

        if (equipo.IdEstado == 6) // 6: DadoDeBaja
        {
            throw new ReglaNegocioException($"El equipo '{equipo.CodigoEquipo}' se encuentra 'Dado de Baja' y no admite órdenes de trabajo (RN-04).");
        }

        // 2. Validar que no exista otra orden correctiva abierta para el mismo equipo (RN-04)
        if (dto.IdTipoOrden == 2) // 2: Correctiva
        {
            bool tieneCorrectivaAbierta = await _repositorioOrden.TieneOrdenCorrectivaAbiertaAsync(dto.CodigoEquipo, cancellationToken);
            if (tieneCorrectivaAbierta)
            {
                throw new ReglaNegocioException($"El equipo '{dto.CodigoEquipo}' ya cuenta con una orden correctiva abierta en curso (RN-04).");
            }
        }

        // 3. Validar solicitante
        var solicitante = await _repositorioUsuario.ObtenerPorIdentificacionAsync(dto.IdentificacionSolicitante, cancellationToken);
        if (solicitante == null)
        {
            throw new NoEncontradoException("Usuario Solicitante", dto.IdentificacionSolicitante);
        }

        // 4. Si se asigna un técnico de entrada, validar su existencia y carga de trabajo (RN-07)
        if (!string.IsNullOrWhiteSpace(dto.IdentificacionTecnico))
        {
            var tecnico = await _repositorioUsuario.ObtenerPorIdentificacionAsync(dto.IdentificacionTecnico, cancellationToken);
            if (tecnico == null)
            {
                throw new NoEncontradoException("Usuario Técnico", dto.IdentificacionTecnico);
            }

            int ordenesEnProceso = await _repositorioOrden.ContarOrdenesEnProcesoPorTecnicoAsync(dto.IdentificacionTecnico, cancellationToken);
            if (ordenesEnProceso >= _options.MaxOrdenesEnProcesoPorTecnico)
            {
                throw new ReglaNegocioException($"El técnico '{dto.IdentificacionTecnico}' ya tiene {ordenesEnProceso} órdenes en proceso simultáneas, alcanzando el límite configurado de {_options.MaxOrdenesEnProcesoPorTecnico} (RN-07).");
            }
        }

        // 5. Calcular fecha límite automáticamente según prioridad (RN-08)
        DateTime ahora = DateTime.UtcNow;
        int horasPlazo = dto.Prioridad switch
        {
            "Critica" => _options.PlazoHorasCritica,
            "Alta" => _options.PlazoHorasAlta,
            "Media" => _options.PlazoHorasMedia,
            _ => _options.PlazoHorasBaja
        };

        DateTime fechaLimite = ahora.AddHours(horasPlazo);

        int estadoInicial = string.IsNullOrWhiteSpace(dto.IdentificacionTecnico) ? 7 : 8; // 7: Abierta, 8: Asignada

        var nuevaOrden = new OrdenDeTrabajo
        {
            CodigoEquipo = dto.CodigoEquipo,
            IdentificacionSolicitante = dto.IdentificacionSolicitante,
            IdentificacionTecnico = string.IsNullOrWhiteSpace(dto.IdentificacionTecnico) ? null : dto.IdentificacionTecnico,
            IdTipoOrden = dto.IdTipoOrden,
            Prioridad = dto.Prioridad,
            FechaCreacion = ahora,
            FechaLimite = fechaLimite,
            TiempoInvertido = 0,
            Descripcion = dto.Descripcion.Trim(),
            SinRepuestos = false,
            IdEstado = estadoInicial
        };

        await _repositorioOrden.AgregarAsync(nuevaOrden, cancellationToken);
        await _repositorioOrden.GuardarCambiosAsync(cancellationToken);

        // Registro inicial obligatorio en la bitácora
        await _repositorioBitacora.RegistrarAsync(new BitacoraOrden
        {
            Numero = nuevaOrden.Numero,
            Identificacion = dto.IdentificacionSolicitante,
            Fecha = ahora,
            EstadoAnterior = "Ninguno",
            EstadoNuevo = NombresEstados[estadoInicial],
            Comentario = "Creación y registro formal de la orden de trabajo en el sistema."
        }, cancellationToken);

        await _repositorioBitacora.GuardarCambiosAsync(cancellationToken);

        return await ObtenerPorNumeroAsync(nuevaOrden.Numero, cancellationToken);
    }

    public async Task<OrdenDeTrabajoDto> ActualizarAsync(int numero, ActualizarOrdenDto dto, CancellationToken cancellationToken = default)
    {
        var orden = await _repositorioOrden.ObtenerPorNumeroAsync(numero, cancellationToken);
        if (orden == null)
        {
            throw new NoEncontradoException("OrdenDeTrabajo", numero);
        }

        // RN-09: Una orden Cerrada es inmutable
        if (orden.IdEstado == 11) // 11: Cerrada
        {
            throw new ConflictoException("Una orden en estado 'Cerrada' es estrictamente inmutable: no puede modificarse (RN-09).");
        }

        ValidarTokenConcurrencia(orden.RowVersion, dto.RowVersion, numero);

        if (!string.IsNullOrWhiteSpace(dto.IdentificacionTecnico))
        {
            var tecnico = await _repositorioUsuario.ObtenerPorIdentificacionAsync(dto.IdentificacionTecnico, cancellationToken);
            if (tecnico == null)
            {
                throw new NoEncontradoException("Usuario Técnico", dto.IdentificacionTecnico);
            }
            orden.IdentificacionTecnico = dto.IdentificacionTecnico;
        }

        orden.Prioridad = dto.Prioridad;
        orden.Descripcion = dto.Descripcion.Trim();
        orden.TiempoInvertido = dto.TiempoInvertido;
        orden.SinRepuestos = dto.SinRepuestos;
        orden.JustificacionSinRepuestos = dto.JustificacionSinRepuestos;

        await _repositorioOrden.ActualizarAsync(orden, cancellationToken);
        await _repositorioOrden.GuardarCambiosAsync(cancellationToken);

        return await ObtenerPorNumeroAsync(numero, cancellationToken);
    }

    public async Task<OrdenDeTrabajoDto> CambiarEstadoAsync(int numero, CambiarEstadoOrdenDto dto, CancellationToken cancellationToken = default)
    {
        var orden = await _repositorioOrden.ObtenerPorNumeroAsync(numero, cancellationToken);
        if (orden == null)
        {
            throw new NoEncontradoException("OrdenDeTrabajo", numero);
        }

        // RN-09: Inmutabilidad de orden cerrada
        if (orden.IdEstado == 11)
        {
            throw new ConflictoException("Una orden en estado 'Cerrada' es estrictamente inmutable: no puede reabrirse ni modificarse (RN-09).");
        }

        ValidarTokenConcurrencia(orden.RowVersion, dto.RowVersion, numero);

        // Validación de máquina de estados
        if (!TransicionesValidas.TryGetValue(orden.IdEstado, out var destinosPermitidos) || !destinosPermitidos.Contains(dto.NuevoEstado))
        {
            string estadoActualStr = NombresEstados.GetValueOrDefault(orden.IdEstado, orden.IdEstado.ToString());
            string nuevoEstadoStr = NombresEstados.GetValueOrDefault(dto.NuevoEstado, dto.NuevoEstado.ToString());
            throw new ConflictoException($"Transición de estado inválida: no es posible pasar de '{estadoActualStr}' a '{nuevoEstadoStr}'.");
        }

        // RN-07: Si pasa a 'En Proceso' (9), validar límite de órdenes activas del técnico
        if (dto.NuevoEstado == 9 && orden.IdentificacionTecnico != null)
        {
            int enProceso = await _repositorioOrden.ContarOrdenesEnProcesoPorTecnicoAsync(orden.IdentificacionTecnico, cancellationToken);
            if (enProceso >= _options.MaxOrdenesEnProcesoPorTecnico)
            {
                throw new ReglaNegocioException($"El técnico asignado '{orden.IdentificacionTecnico}' ya cuenta con {enProceso} órdenes en proceso, alcanzando el límite permitido de {_options.MaxOrdenesEnProcesoPorTecnico} (RN-07).");
            }
        }

        // RN-02: Validación para el cierre de la orden (Estado 11: Cerrada)
        if (dto.NuevoEstado == 11)
        {
            decimal tiempoFinal = dto.TiempoInvertidoFinal ?? orden.TiempoInvertido;
            if (tiempoFinal <= 0)
            {
                throw new ReglaNegocioException("No se puede cerrar una orden sin registrar un tiempo invertido mayor a 0 horas (RN-02).");
            }

            bool sinRepuestos = dto.SinRepuestos ?? orden.SinRepuestos;
            string? justificacion = dto.JustificacionSinRepuestos ?? orden.JustificacionSinRepuestos;
            bool tieneRepuestosUsados = orden.RepuestosUsados != null && orden.RepuestosUsados.Any();

            if (!tieneRepuestosUsados && (!sinRepuestos || string.IsNullOrWhiteSpace(justificacion)))
            {
                throw new ReglaNegocioException("No se puede cerrar una orden sin al menos un repuesto usado o una declaración justificada de 'sin repuestos' (RN-02).");
            }

            orden.TiempoInvertido = tiempoFinal;
            orden.SinRepuestos = sinRepuestos;
            orden.JustificacionSinRepuestos = justificacion;
        }

        string estadoAnteriorNombre = NombresEstados.GetValueOrDefault(orden.IdEstado, orden.IdEstado.ToString());
        string estadoNuevoNombre = NombresEstados.GetValueOrDefault(dto.NuevoEstado, dto.NuevoEstado.ToString());

        orden.IdEstado = dto.NuevoEstado;

        await _repositorioOrden.ActualizarAsync(orden, cancellationToken);
        await _repositorioOrden.GuardarCambiosAsync(cancellationToken);

        // Registro estricto e inmutable en BitacoraOrden en CADA cambio de estado
        var bitacora = new BitacoraOrden
        {
            Numero = orden.Numero,
            Identificacion = dto.IdentificacionUsuario,
            Fecha = DateTime.UtcNow,
            EstadoAnterior = estadoAnteriorNombre,
            EstadoNuevo = estadoNuevoNombre,
            Comentario = dto.Comentario.Trim()
        };

        await _repositorioBitacora.RegistrarAsync(bitacora, cancellationToken);
        await _repositorioBitacora.GuardarCambiosAsync(cancellationToken);

        return await ObtenerPorNumeroAsync(numero, cancellationToken);
    }

    public async Task<OrdenDeTrabajoDto> AsignarRepuestoAsync(int numero, AsignarRepuestoOrdenDto dto, CancellationToken cancellationToken = default)
    {
        var orden = await _repositorioOrden.ObtenerPorNumeroAsync(numero, cancellationToken);
        if (orden == null)
        {
            throw new NoEncontradoException("OrdenDeTrabajo", numero);
        }

        if (orden.IdEstado == 11)
        {
            throw new ConflictoException("No se pueden agregar repuestos a una orden cerrada (RN-09).");
        }

        var repuesto = await _repositorioRepuesto.ObtenerPorCodigoAsync(dto.CodigoRepuesto, cancellationToken);
        if (repuesto == null)
        {
            throw new NoEncontradoException("Repuesto", dto.CodigoRepuesto);
        }

        // RN-05: Al usar repuestos se descuenta el stock; no puede quedar negativo
        if (repuesto.Stock < dto.Cantidad)
        {
            throw new ReglaNegocioException($"Stock insuficiente para el repuesto '{repuesto.CodigoRepuesto}'. Stock disponible: {repuesto.Stock}, solicitado: {dto.Cantidad} (RN-05).");
        }

        repuesto.Stock -= dto.Cantidad;
        await _repositorioRepuesto.ActualizarAsync(repuesto, cancellationToken);

        var repuestoExistente = orden.RepuestosUsados.FirstOrDefault(r => r.CodigoRepuesto == dto.CodigoRepuesto);
        if (repuestoExistente != null)
        {
            repuestoExistente.CantidadUsada += dto.Cantidad;
        }
        else
        {
            orden.RepuestosUsados.Add(new OrdenRepuesto
            {
                Numero = orden.Numero,
                CodigoRepuesto = repuesto.CodigoRepuesto,
                CantidadUsada = dto.Cantidad,
                CostoAplicado = repuesto.CostoUnitario
            });
        }

        await _repositorioOrden.ActualizarAsync(orden, cancellationToken);
        await _repositorioOrden.GuardarCambiosAsync(cancellationToken);

        return await ObtenerPorNumeroAsync(numero, cancellationToken);
    }

    public async Task<IEnumerable<BitacoraOrden>> ObtenerHistorialBitacoraAsync(int numero, CancellationToken cancellationToken = default)
    {
        var orden = await _repositorioOrden.ObtenerPorNumeroAsync(numero, cancellationToken);
        if (orden == null)
        {
            throw new NoEncontradoException("OrdenDeTrabajo", numero);
        }

        return await _repositorioBitacora.ObtenerHistorialPorOrdenAsync(numero, cancellationToken);
    }

    private static void ValidarTokenConcurrencia(byte[] actualToken, string clientTokenStr, int numeroOrden)
    {
        if (string.IsNullOrWhiteSpace(clientTokenStr)) return;

        try
        {
            byte[] clientToken = Convert.FromBase64String(clientTokenStr);
            if (actualToken != null && actualToken.Length > 0 && !clientToken.SequenceEqual(actualToken))
            {
                throw new ConflictoException($"La orden #{numeroOrden} fue modificada por otro usuario concurrentemente. Recargue la orden para obtener los últimos datos.");
            }
        }
        catch (FormatException)
        {
            throw new ReglaNegocioException("El token de concurrencia de la orden no tiene un formato Base64 válido.");
        }
    }

    private static OrdenDeTrabajoDto MapearADto(OrdenDeTrabajo o)
    {
        return new OrdenDeTrabajoDto
        {
            Numero = o.Numero,
            CodigoEquipo = o.CodigoEquipo,
            EquipoNombre = o.Equipo?.Nombre ?? string.Empty,
            IdentificacionSolicitante = o.IdentificacionSolicitante,
            SolicitanteNombre = o.Solicitante?.Nombre ?? string.Empty,
            IdentificacionTecnico = o.IdentificacionTecnico,
            TecnicoNombre = o.Tecnico?.Nombre,
            IdTipoOrden = o.IdTipoOrden,
            TipoOrdenNombre = o.TipoOrden?.Nombre ?? string.Empty,
            Prioridad = o.Prioridad,
            FechaCreacion = o.FechaCreacion,
            FechaLimite = o.FechaLimite,
            TiempoInvertido = o.TiempoInvertido,
            Descripcion = o.Descripcion,
            SinRepuestos = o.SinRepuestos,
            JustificacionSinRepuestos = o.JustificacionSinRepuestos,
            IdEstado = o.IdEstado,
            EstadoNombre = o.Estado?.Nombre ?? string.Empty,
            RowVersion = o.RowVersion != null && o.RowVersion.Length > 0 ? Convert.ToBase64String(o.RowVersion) : string.Empty,
            Repuestos = o.RepuestosUsados?.Select(r => new OrdenRepuestoDto
            {
                CodigoRepuesto = r.CodigoRepuesto,
                NombreRepuesto = r.Repuesto?.Nombre ?? string.Empty,
                CantidadUsada = r.CantidadUsada,
                CostoAplicado = r.CostoAplicado
            }).ToList() ?? new List<OrdenRepuestoDto>()
        };
    }
}
