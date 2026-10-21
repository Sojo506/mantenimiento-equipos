using Abstracciones.Entidades;
using Abstracciones.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace DA.Semilla;

/// <summary>
/// Contiene los datos semilla iniciales del sistema con catálogos, usuarios,
/// equipos, repuestos y más de 30 órdenes de trabajo para demostración.
/// </summary>
public static class MantenimientoSeedData
{
    private static readonly byte[] SaltFijo = new byte[] { 11, 22, 33, 44, 55, 66, 77, 88, 99, 101, 111, 121, 131, 141, 151, 161 };
    private static readonly DateTime FechaBaseSemilla = new DateTime(2026, 8, 1, 8, 0, 0, DateTimeKind.Utc);

    public static void ConfigurarSemilla(ModelBuilder modelBuilder)
    {
        // 1. Estados generales, de equipos y de órdenes
        modelBuilder.Entity<Estado>().HasData(
            new Estado { IdEstado = 1, Nombre = "Activo" },
            new Estado { IdEstado = 2, Nombre = "Inactivo" },
            new Estado { IdEstado = 3, Nombre = "Operativo" },
            new Estado { IdEstado = 4, Nombre = "En Mantenimiento" },
            new Estado { IdEstado = 5, Nombre = "En Riesgo" },
            new Estado { IdEstado = 6, Nombre = "Dado de Baja" },
            new Estado { IdEstado = 7, Nombre = "Abierta" },
            new Estado { IdEstado = 8, Nombre = "Asignada" },
            new Estado { IdEstado = 9, Nombre = "En Proceso" },
            new Estado { IdEstado = 10, Nombre = "En Espera de Repuesto" },
            new Estado { IdEstado = 11, Nombre = "Cerrada" },
            new Estado { IdEstado = 12, Nombre = "Escalada" }
        );

        // 2. Tipos de Usuario (Roles)
        modelBuilder.Entity<TipoUsuario>().HasData(
            new TipoUsuario { IdTipoUsuario = 1, Nombre = "Administrador", IdEstado = 1 },
            new TipoUsuario { IdTipoUsuario = 2, Nombre = "Supervisor", IdEstado = 1 },
            new TipoUsuario { IdTipoUsuario = 3, Nombre = "Tecnico", IdEstado = 1 },
            new TipoUsuario { IdTipoUsuario = 4, Nombre = "Solicitante", IdEstado = 1 }
        );

        // 3. Especialidades
        modelBuilder.Entity<Especialidad>().HasData(
            new Especialidad { IdEspecialidad = 1, Nombre = "Mecánica Industrial y Neumática", IdEstado = 1 },
            new Especialidad { IdEspecialidad = 2, Nombre = "Electrónica y Automatización", IdEstado = 1 },
            new Especialidad { IdEspecialidad = 3, Nombre = "Equipos Biomédicos", IdEstado = 1 },
            new Especialidad { IdEspecialidad = 4, Nombre = "Infraestructura de Servidores", IdEstado = 1 },
            new Especialidad { IdEspecialidad = 5, Nombre = "Refrigeración y Climatización", IdEstado = 1 }
        );

        // Hashes de contraseñas de prueba (doc en README: Admin123!, Super123!, Tecnico123!, Solicitante123!)
        string hashAdmin = HashHelper.HashearConSalt("Admin123!", SaltFijo);
        string hashSuper = HashHelper.HashearConSalt("Super123!", SaltFijo);
        string hashTecnico = HashHelper.HashearConSalt("Tecnico123!", SaltFijo);
        string hashSolicitante = HashHelper.HashearConSalt("Solicitante123!", SaltFijo);

        // 4. Usuarios de prueba (al menos uno por cada rol)
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Identificacion = "USR-001",
                Nombre = "Carlos",
                PrimerApellido = "Mora",
                SegundoApellido = "Soto",
                Correo = "admin@mantenimiento.com",
                PasswordHash = hashAdmin,
                IdTipoUsuario = 1, // Administrador
                IdEspecialidad = null,
                Disponibilidad = true,
                IdEstado = 1
            },
            new Usuario
            {
                Identificacion = "USR-002",
                Nombre = "Elena",
                PrimerApellido = "Vargas",
                SegundoApellido = "Castro",
                Correo = "supervisor@mantenimiento.com",
                PasswordHash = hashSuper,
                IdTipoUsuario = 2, // Supervisor
                IdEspecialidad = null,
                Disponibilidad = true,
                IdEstado = 1
            },
            new Usuario
            {
                Identificacion = "USR-003",
                Nombre = "Javier",
                PrimerApellido = "Rojas",
                SegundoApellido = "Méndez",
                Correo = "tecnico1@mantenimiento.com",
                PasswordHash = hashTecnico,
                IdTipoUsuario = 3, // Tecnico
                IdEspecialidad = 1,
                Disponibilidad = true,
                IdEstado = 1
            },
            new Usuario
            {
                Identificacion = "USR-004",
                Nombre = "Andrés",
                PrimerApellido = "Navarro",
                SegundoApellido = "Pérez",
                Correo = "tecnico2@mantenimiento.com",
                PasswordHash = hashTecnico,
                IdTipoUsuario = 3, // Tecnico
                IdEspecialidad = 2,
                Disponibilidad = true,
                IdEstado = 1
            },
            new Usuario
            {
                Identificacion = "USR-005",
                Nombre = "María",
                PrimerApellido = "Solano",
                SegundoApellido = "Chaves",
                Correo = "solicitante@mantenimiento.com",
                PasswordHash = hashSolicitante,
                IdTipoUsuario = 4, // Solicitante
                IdEspecialidad = null,
                Disponibilidad = true,
                IdEstado = 1
            },
            new Usuario
            {
                Identificacion = "USR-006",
                Nombre = "Roberto",
                PrimerApellido = "Gómez",
                SegundoApellido = "Alvarado",
                Correo = "solicitante2@mantenimiento.com",
                PasswordHash = hashSolicitante,
                IdTipoUsuario = 4, // Solicitante
                IdEspecialidad = null,
                Disponibilidad = true,
                IdEstado = 1
            }
        );

        // 5. Tipos de Equipo
        modelBuilder.Entity<TipoEquipo>().HasData(
            new TipoEquipo { IdTipoEquipo = 1, Nombre = "Servidor Rack de Cómputo", IdEstado = 1 },
            new TipoEquipo { IdTipoEquipo = 2, Nombre = "Brazo Robótico Articulado", IdEstado = 1 },
            new TipoEquipo { IdTipoEquipo = 3, Nombre = "Tomógrafo Clínico", IdEstado = 1 },
            new TipoEquipo { IdTipoEquipo = 4, Nombre = "Compresor de Aire Industrial", IdEstado = 1 },
            new TipoEquipo { IdTipoEquipo = 5, Nombre = "Generador Eléctrico Diesel", IdEstado = 1 }
        );

        // 6. Marcas
        modelBuilder.Entity<Marca>().HasData(
            new Marca { IdMarca = 1, Nombre = "Dell Technologies", IdEstado = 1 },
            new Marca { IdMarca = 2, Nombre = "KUKA Robotics", IdEstado = 1 },
            new Marca { IdMarca = 3, Nombre = "General Electric Healthcare", IdEstado = 1 },
            new Marca { IdMarca = 4, Nombre = "Atlas Copco", IdEstado = 1 },
            new Marca { IdMarca = 5, Nombre = "Caterpillar Energy", IdEstado = 1 }
        );

        // 7. Modelos
        modelBuilder.Entity<Modelo>().HasData(
            new Modelo { IdModelo = 1, Nombre = "PowerEdge R750 Enterprise", IdEstado = 1 },
            new Modelo { IdModelo = 2, Nombre = "KR CYBERTECH Nano", IdEstado = 1 },
            new Modelo { IdModelo = 3, Nombre = "Optima CT660 Medical", IdEstado = 1 },
            new Modelo { IdModelo = 4, Nombre = "GA 37 VSD+ Screw Compressor", IdEstado = 1 },
            new Modelo { IdModelo = 5, Nombre = "Cat C15 ACERT Generator", IdEstado = 1 }
        );

        // 8. Ubicaciones
        modelBuilder.Entity<Ubicacion>().HasData(
            new Ubicacion { IdUbicacion = 1, Nombre = "Centro de Datos Principal - Piso 2", IdEstado = 1 },
            new Ubicacion { IdUbicacion = 2, Nombre = "Planta de Ensamble - Bahía A3", IdEstado = 1 },
            new Ubicacion { IdUbicacion = 3, Nombre = "Unidad de Diagnóstico Clínico", IdEstado = 1 },
            new Ubicacion { IdUbicacion = 4, Nombre = "Taller Central de Neumática", IdEstado = 1 },
            new Ubicacion { IdUbicacion = 5, Nombre = "Patio de Fuerza y Generación", IdEstado = 1 }
        );

        // 9. Equipos
        modelBuilder.Entity<Equipo>().HasData(
            new Equipo
            {
                CodigoEquipo = "EQ-001",
                Nombre = "Servidor Base de Datos ERP",
                IdTipoEquipo = 1,
                IdMarca = 1,
                IdModelo = 1,
                IdUbicacion = 1,
                FechaAdquisicion = new DateTime(2023, 2, 15),
                IdEstado = 3 // Operativo
            },
            new Equipo
            {
                CodigoEquipo = "EQ-002",
                Nombre = "Robot de Soldadura Automatizada",
                IdTipoEquipo = 2,
                IdMarca = 2,
                IdModelo = 2,
                IdUbicacion = 2,
                FechaAdquisicion = new DateTime(2022, 5, 20),
                IdEstado = 3 // Operativo
            },
            new Equipo
            {
                CodigoEquipo = "EQ-003",
                Nombre = "Tomógrafo Computarizado Principal",
                IdTipoEquipo = 3,
                IdMarca = 3,
                IdModelo = 3,
                IdUbicacion = 3,
                FechaAdquisicion = new DateTime(2021, 11, 10),
                IdEstado = 4 // EnMantenimiento
            },
            new Equipo
            {
                CodigoEquipo = "EQ-004",
                Nombre = "Compresor de Aire Línea 2",
                IdTipoEquipo = 4,
                IdMarca = 4,
                IdModelo = 4,
                IdUbicacion = 4,
                FechaAdquisicion = new DateTime(2020, 8, 5),
                IdEstado = 5 // EnRiesgo
            },
            new Equipo
            {
                CodigoEquipo = "EQ-005",
                Nombre = "Generador Diesel Emergencia 500kVA",
                IdTipoEquipo = 5,
                IdMarca = 5,
                IdModelo = 5,
                IdUbicacion = 5,
                FechaAdquisicion = new DateTime(2021, 3, 1),
                IdEstado = 3 // Operativo
            },
            new Equipo
            {
                CodigoEquipo = "EQ-006",
                Nombre = "Servidor Web Cluster Nodo 1",
                IdTipoEquipo = 1,
                IdMarca = 1,
                IdModelo = 1,
                IdUbicacion = 1,
                FechaAdquisicion = new DateTime(2023, 6, 12),
                IdEstado = 3 // Operativo
            },
            new Equipo
            {
                CodigoEquipo = "EQ-007",
                Nombre = "Robot de Pintura y Acabado",
                IdTipoEquipo = 2,
                IdMarca = 2,
                IdModelo = 2,
                IdUbicacion = 2,
                FechaAdquisicion = new DateTime(2022, 9, 18),
                IdEstado = 3 // Operativo
            },
            new Equipo
            {
                CodigoEquipo = "EQ-008",
                Nombre = "Compresor Reciprocante Antiguo",
                IdTipoEquipo = 4,
                IdMarca = 4,
                IdModelo = 4,
                IdUbicacion = 4,
                FechaAdquisicion = new DateTime(2015, 1, 10),
                IdEstado = 6 // DadoDeBaja
            }
        );

        // 10. Repuestos (incluye 3 bajo stock mínimo para probar alertas)
        modelBuilder.Entity<Repuesto>().HasData(
            new Repuesto
            {
                CodigoRepuesto = "REP-001",
                Nombre = "Filtro de Aire Cartucho Micrónico",
                Stock = 24,
                StockMinimo = 10,
                CostoUnitario = 45.00m,
                IdEstado = 1
            },
            new Repuesto
            {
                CodigoRepuesto = "REP-002",
                Nombre = "Aceite Sintético ISO VG 68 (Galón)",
                Stock = 50,
                StockMinimo = 15,
                CostoUnitario = 32.50m,
                IdEstado = 1
            },
            new Repuesto
            {
                CodigoRepuesto = "REP-003",
                Nombre = "Disco SSD Enterprise NVMe 1.92TB",
                Stock = 8,
                StockMinimo = 5,
                CostoUnitario = 275.00m,
                IdEstado = 1
            },
            new Repuesto
            {
                CodigoRepuesto = "REP-004",
                Nombre = "Correa Dentada de Transmisión HTD",
                Stock = 3, // BAJO EL MÍNIMO
                StockMinimo = 8,
                CostoUnitario = 68.00m,
                IdEstado = 1
            },
            new Repuesto
            {
                CodigoRepuesto = "REP-005",
                Nombre = "Sensor Inductivo M12 24VDC",
                Stock = 14,
                StockMinimo = 6,
                CostoUnitario = 58.00m,
                IdEstado = 1
            },
            new Repuesto
            {
                CodigoRepuesto = "REP-006",
                Nombre = "Rodamiento de Rodillos Cónicos SKF",
                Stock = 2, // BAJO EL MÍNIMO
                StockMinimo = 10,
                CostoUnitario = 85.00m,
                IdEstado = 1
            },
            new Repuesto
            {
                CodigoRepuesto = "REP-007",
                Nombre = "Válvula Reguladora de Presión Neumática",
                Stock = 16,
                StockMinimo = 5,
                CostoUnitario = 110.00m,
                IdEstado = 1
            },
            new Repuesto
            {
                CodigoRepuesto = "REP-008",
                Nombre = "Batería Plomo-Ácido AGM 12V 100Ah",
                Stock = 1, // BAJO EL MÍNIMO
                StockMinimo = 4,
                CostoUnitario = 210.00m,
                IdEstado = 1
            }
        );

        // 11. Tipos de Orden
        modelBuilder.Entity<TipoOrden>().HasData(
            new TipoOrden { IdTipoOrden = 1, Nombre = "Preventiva", IdEstado = 1 },
            new TipoOrden { IdTipoOrden = 2, Nombre = "Correctiva", IdEstado = 1 }
        );

        // 12. Tipos de Gravedad
        modelBuilder.Entity<TipoGravedad>().HasData(
            new TipoGravedad { IdTipoGravedad = 1, Nombre = "Baja", IdEstado = 1 },
            new TipoGravedad { IdTipoGravedad = 2, Nombre = "Media", IdEstado = 1 },
            new TipoGravedad { IdTipoGravedad = 3, Nombre = "Alta", IdEstado = 1 },
            new TipoGravedad { IdTipoGravedad = 4, Nombre = "Critica", IdEstado = 1 }
        );

        // 13. Planes de Mantenimiento Preventivo
        modelBuilder.Entity<PlanDeMantenimiento>().HasData(
            new PlanDeMantenimiento
            {
                IdPlanMantenimiento = 1,
                CodigoEquipo = "EQ-001",
                Frecuencia = 30,
                ProximaFecha = new DateTime(2026, 11, 1),
                IdEstado = 1
            },
            new PlanDeMantenimiento
            {
                IdPlanMantenimiento = 2,
                CodigoEquipo = "EQ-002",
                Frecuencia = 60,
                ProximaFecha = new DateTime(2026, 11, 15),
                IdEstado = 1
            },
            new PlanDeMantenimiento
            {
                IdPlanMantenimiento = 3,
                CodigoEquipo = "EQ-003",
                Frecuencia = 90,
                ProximaFecha = new DateTime(2026, 10, 20), // Fecha vencida para probar generación automática en segundo plano
                IdEstado = 1
            },
            new PlanDeMantenimiento
            {
                IdPlanMantenimiento = 4,
                CodigoEquipo = "EQ-004",
                Frecuencia = 45,
                ProximaFecha = new DateTime(2026, 11, 10),
                IdEstado = 1
            },
            new PlanDeMantenimiento
            {
                IdPlanMantenimiento = 5,
                CodigoEquipo = "EQ-005",
                Frecuencia = 180,
                ProximaFecha = new DateTime(2026, 12, 1),
                IdEstado = 1
            }
        );

        // 14. 32 Órdenes de Trabajo Semilla
        var ordenes = new List<OrdenDeTrabajo>();
        var bitacoras = new List<BitacoraOrden>();
        var ordenRepuestos = new List<OrdenRepuesto>();
        var fallas = new List<FallaRegistrada>();

        int idBitacora = 1;
        int idFalla = 1;

        // Generamos 32 órdenes con variada casuística
        for (int i = 1; i <= 32; i++)
        {
            string equipoId = $"EQ-00{(i % 7) + 1}";
            string solicitanteId = (i % 2 == 0) ? "USR-005" : "USR-006";
            string? tecnicoId = (i <= 5) ? null : ((i % 2 == 0) ? "USR-003" : "USR-004");
            int tipoOrden = (i % 3 == 0) ? 2 : 1; // 2=Correctiva, 1=Preventiva
            string prioridad = (i % 4 == 0) ? "Critica" : (i % 3 == 0) ? "Alta" : (i % 2 == 0) ? "Media" : "Baja";
            
            // Distribución de estados:
            // 1-18: Cerradas
            // 19-23: En Proceso
            // 24-27: Asignadas
            // 28-30: Abiertas
            // 31: En Espera de Repuesto
            // 32: Escalada
            int estado = (i <= 18) ? 11 : 
                         (i <= 23) ? 9 : 
                         (i <= 27) ? 8 : 
                         (i <= 30) ? 7 : 
                         (i == 31) ? 10 : 12;

            DateTime fechaCreacion = FechaBaseSemilla.AddDays(i * 1.5);
            DateTime fechaLimite = fechaCreacion.AddDays(prioridad == "Critica" ? 1 : prioridad == "Alta" ? 2 : prioridad == "Media" ? 5 : 10);
            decimal tiempoInvertido = (estado == 11) ? (i * 0.75m) + 1.5m : (estado == 9) ? 2.0m : 0m;
            bool sinRepuestos = (i == 4 || i == 9 || i == 14); // Algunas cerradas sin repuestos justificados
            string? justificacionSinRepuestos = sinRepuestos ? "Mantenimiento preventivo de inspección visual, calibración y limpieza sin sustitución de partes." : null;

            ordenes.Add(new OrdenDeTrabajo
            {
                Numero = i,
                CodigoEquipo = equipoId,
                IdentificacionSolicitante = solicitanteId,
                IdentificacionTecnico = (estado == 7) ? null : tecnicoId,
                IdTipoOrden = tipoOrden,
                Prioridad = prioridad,
                FechaCreacion = fechaCreacion,
                FechaLimite = fechaLimite,
                TiempoInvertido = tiempoInvertido,
                Descripcion = $"Orden de trabajo #{i}: Mantenimiento {(tipoOrden == 1 ? "preventivo programado" : "correctivo por incidencia")} sobre {equipoId}.",
                SinRepuestos = sinRepuestos,
                JustificacionSinRepuestos = justificacionSinRepuestos,
                IdEstado = estado
            });

            // Bitácora de apertura
            bitacoras.Add(new BitacoraOrden
            {
                IdBitacora = idBitacora++,
                Numero = i,
                Identificacion = solicitanteId,
                Fecha = fechaCreacion,
                EstadoAnterior = "Ninguno",
                EstadoNuevo = "Abierta",
                Comentario = "Creación y registro de la orden de trabajo."
            });

            // Si está asignada o posterior
            if (estado >= 8 && tecnicoId != null)
            {
                bitacoras.Add(new BitacoraOrden
                {
                    IdBitacora = idBitacora++,
                    Numero = i,
                    Identificacion = "USR-002", // Supervisor que asignó
                    Fecha = fechaCreacion.AddHours(2),
                    EstadoAnterior = "Abierta",
                    EstadoNuevo = "Asignada",
                    Comentario = $"Asignación formal de la orden al técnico {tecnicoId}."
                });
            }

            // Si pasó a En Proceso
            if (estado == 9 || estado == 10 || estado == 11 || estado == 12)
            {
                bitacoras.Add(new BitacoraOrden
                {
                    IdBitacora = idBitacora++,
                    Numero = i,
                    Identificacion = tecnicoId ?? "USR-003",
                    Fecha = fechaCreacion.AddHours(4),
                    EstadoAnterior = "Asignada",
                    EstadoNuevo = "En Proceso",
                    Comentario = "Inicio de labores de diagnóstico y ejecución de mantenimiento."
                });
            }

            // Si fue cerrada
            if (estado == 11)
            {
                bitacoras.Add(new BitacoraOrden
                {
                    IdBitacora = idBitacora++,
                    Numero = i,
                    Identificacion = tecnicoId ?? "USR-003",
                    Fecha = fechaCreacion.AddHours(12),
                    EstadoAnterior = "En Proceso",
                    EstadoNuevo = "Cerrada",
                    Comentario = sinRepuestos 
                        ? $"Cierre de orden sin repuestos: {justificacionSinRepuestos}" 
                        : "Cierre exitoso de la orden tras verificación de funcionamiento."
                });

                // Si no fue "sin repuestos", le agregamos repuesto a la tabla intermedia
                if (!sinRepuestos)
                {
                    string repuestoId = (i % 2 == 0) ? "REP-001" : (i % 3 == 0) ? "REP-002" : "REP-005";
                    int cantidad = (i % 3) + 1;
                    decimal costo = (repuestoId == "REP-001") ? 45.00m : (repuestoId == "REP-002") ? 32.50m : 58.00m;

                    ordenRepuestos.Add(new OrdenRepuesto
                    {
                        Numero = i,
                        CodigoRepuesto = repuestoId,
                        CantidadUsada = cantidad,
                        CostoAplicado = costo
                    });
                }
            }

            // Si es correctiva, registrar falla
            if (tipoOrden == 2)
            {
                fallas.Add(new FallaRegistrada
                {
                    Id = idFalla++,
                    CodigoEquipo = equipoId,
                    Fecha = fechaCreacion,
                    IdTipoGravedad = (prioridad == "Critica") ? 4 : (prioridad == "Alta") ? 3 : 2,
                    Descripcion = $"Falla reportada en equipo {equipoId}: anomalía en operación continua que requirió orden #{i}.",
                    NumeroOrden = i
                });
            }
        }

        // Agregar órdenes adicionales cerradas con repuestos para EQ-004 para cumplir RN-03 (más de N fallas en 6 meses -> EnRiesgo)
        fallas.Add(new FallaRegistrada
        {
            Id = idFalla++,
            CodigoEquipo = "EQ-004",
            Fecha = new DateTime(2026, 9, 15, 10, 0, 0, DateTimeKind.Utc),
            IdTipoGravedad = 3,
            Descripcion = "Fuga recurrente de presión en válvula de descarga.",
            NumeroOrden = 10
        });
        fallas.Add(new FallaRegistrada
        {
            Id = idFalla++,
            CodigoEquipo = "EQ-004",
            Fecha = new DateTime(2026, 8, 20, 14, 30, 0, DateTimeKind.Utc),
            IdTipoGravedad = 3,
            Descripcion = "Sobrecalentamiento en cabezal de compresión.",
            NumeroOrden = 12
        });
        fallas.Add(new FallaRegistrada
        {
            Id = idFalla++,
            CodigoEquipo = "EQ-004",
            Fecha = new DateTime(2026, 7, 25, 9, 15, 0, DateTimeKind.Utc),
            IdTipoGravedad = 4,
            Descripcion = "Paro imprevisto por vibración excesiva en rodamientos.",
            NumeroOrden = 15
        });

        modelBuilder.Entity<OrdenDeTrabajo>().HasData(ordenes);
        modelBuilder.Entity<BitacoraOrden>().HasData(bitacoras);
        modelBuilder.Entity<OrdenRepuesto>().HasData(ordenRepuestos);
        modelBuilder.Entity<FallaRegistrada>().HasData(fallas);
    }
}
