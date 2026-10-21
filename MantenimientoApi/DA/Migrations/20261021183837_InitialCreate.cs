using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DA.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ESTADO_TB",
                columns: table => new
                {
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESTADO_TB", x => x.ID_ESTADO);
                });

            migrationBuilder.CreateTable(
                name: "ESPECIALIDAD_TB",
                columns: table => new
                {
                    ID_ESPECIALIDAD = table.Column<int>(type: "int", nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESPECIALIDAD_TB", x => x.ID_ESPECIALIDAD);
                    table.ForeignKey(
                        name: "FK_ESPECIALIDAD_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MARCA_TB",
                columns: table => new
                {
                    ID_MARCA = table.Column<int>(type: "int", nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MARCA_TB", x => x.ID_MARCA);
                    table.ForeignKey(
                        name: "FK_MARCA_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MODELO_TB",
                columns: table => new
                {
                    ID_MODELO = table.Column<int>(type: "int", nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MODELO_TB", x => x.ID_MODELO);
                    table.ForeignKey(
                        name: "FK_MODELO_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "REPUESTO_TB",
                columns: table => new
                {
                    CODIGO_REPUESTO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    STOCK = table.Column<int>(type: "int", nullable: false),
                    STOCK_MINIMO = table.Column<int>(type: "int", nullable: false),
                    COSTO_UNITARIO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false),
                    ROW_VERSION = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REPUESTO_TB", x => x.CODIGO_REPUESTO);
                    table.ForeignKey(
                        name: "FK_REPUESTO_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TIPO_EQUIPO_TB",
                columns: table => new
                {
                    ID_TIPO_EQUIPO = table.Column<int>(type: "int", nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIPO_EQUIPO_TB", x => x.ID_TIPO_EQUIPO);
                    table.ForeignKey(
                        name: "FK_TIPO_EQUIPO_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TIPO_GRAVEDAD_TB",
                columns: table => new
                {
                    ID_TIPO_GRAVEDAD = table.Column<int>(type: "int", nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIPO_GRAVEDAD_TB", x => x.ID_TIPO_GRAVEDAD);
                    table.ForeignKey(
                        name: "FK_TIPO_GRAVEDAD_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TIPO_ORDEN_TB",
                columns: table => new
                {
                    ID_TIPO_ORDEN = table.Column<int>(type: "int", nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIPO_ORDEN_TB", x => x.ID_TIPO_ORDEN);
                    table.ForeignKey(
                        name: "FK_TIPO_ORDEN_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TIPO_USUARIO_TB",
                columns: table => new
                {
                    ID_TIPO_USUARIO = table.Column<int>(type: "int", nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIPO_USUARIO_TB", x => x.ID_TIPO_USUARIO);
                    table.ForeignKey(
                        name: "FK_TIPO_USUARIO_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UBICACION_TB",
                columns: table => new
                {
                    ID_UBICACION = table.Column<int>(type: "int", nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UBICACION_TB", x => x.ID_UBICACION);
                    table.ForeignKey(
                        name: "FK_UBICACION_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO_TB",
                columns: table => new
                {
                    IDENTIFICACION = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PRIMER_APELLIDO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SEGUNDO_APELLIDO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CORREO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ID_TIPO_USUARIO = table.Column<int>(type: "int", nullable: false),
                    ID_ESPECIALIDAD = table.Column<int>(type: "int", nullable: true),
                    DISPONIBILIDAD = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO_TB", x => x.IDENTIFICACION);
                    table.ForeignKey(
                        name: "FK_USUARIO_TB_ESPECIALIDAD_TB_ID_ESPECIALIDAD",
                        column: x => x.ID_ESPECIALIDAD,
                        principalTable: "ESPECIALIDAD_TB",
                        principalColumn: "ID_ESPECIALIDAD",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_USUARIO_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_USUARIO_TB_TIPO_USUARIO_TB_ID_TIPO_USUARIO",
                        column: x => x.ID_TIPO_USUARIO,
                        principalTable: "TIPO_USUARIO_TB",
                        principalColumn: "ID_TIPO_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EQUIPO_TB",
                columns: table => new
                {
                    CODIGO_EQUIPO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NOMBRE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ID_TIPO_EQUIPO = table.Column<int>(type: "int", nullable: false),
                    ID_MARCA = table.Column<int>(type: "int", nullable: false),
                    ID_MODELO = table.Column<int>(type: "int", nullable: false),
                    ID_UBICACION = table.Column<int>(type: "int", nullable: false),
                    FECHA_ADQUISICION = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQUIPO_TB", x => x.CODIGO_EQUIPO);
                    table.ForeignKey(
                        name: "FK_EQUIPO_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EQUIPO_TB_MARCA_TB_ID_MARCA",
                        column: x => x.ID_MARCA,
                        principalTable: "MARCA_TB",
                        principalColumn: "ID_MARCA",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EQUIPO_TB_MODELO_TB_ID_MODELO",
                        column: x => x.ID_MODELO,
                        principalTable: "MODELO_TB",
                        principalColumn: "ID_MODELO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EQUIPO_TB_TIPO_EQUIPO_TB_ID_TIPO_EQUIPO",
                        column: x => x.ID_TIPO_EQUIPO,
                        principalTable: "TIPO_EQUIPO_TB",
                        principalColumn: "ID_TIPO_EQUIPO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EQUIPO_TB_UBICACION_TB_ID_UBICACION",
                        column: x => x.ID_UBICACION,
                        principalTable: "UBICACION_TB",
                        principalColumn: "ID_UBICACION",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ORDEN_TB",
                columns: table => new
                {
                    NUMERO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_EQUIPO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IDENTIFICACION_SOLICITANTE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IDENTIFICACION_TECNICO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ID_TIPO_ORDEN = table.Column<int>(type: "int", nullable: false),
                    PRIORIDAD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FECHA_CREACION = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FECHA_LIMITE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TIEMPO_INVERTIDO = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SIN_REPUESTOS = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    JUSTIFICACION_SIN_REPUESTOS = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false),
                    ROW_VERSION = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDEN_TB", x => x.NUMERO);
                    table.ForeignKey(
                        name: "FK_ORDEN_TB_EQUIPO_TB_ID_EQUIPO",
                        column: x => x.ID_EQUIPO,
                        principalTable: "EQUIPO_TB",
                        principalColumn: "CODIGO_EQUIPO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ORDEN_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ORDEN_TB_TIPO_ORDEN_TB_ID_TIPO_ORDEN",
                        column: x => x.ID_TIPO_ORDEN,
                        principalTable: "TIPO_ORDEN_TB",
                        principalColumn: "ID_TIPO_ORDEN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ORDEN_TB_USUARIO_TB_IDENTIFICACION_SOLICITANTE",
                        column: x => x.IDENTIFICACION_SOLICITANTE,
                        principalTable: "USUARIO_TB",
                        principalColumn: "IDENTIFICACION",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ORDEN_TB_USUARIO_TB_IDENTIFICACION_TECNICO",
                        column: x => x.IDENTIFICACION_TECNICO,
                        principalTable: "USUARIO_TB",
                        principalColumn: "IDENTIFICACION",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PLAN_MANTENIMIENTO_TB",
                columns: table => new
                {
                    ID_PLAN_MANTENIMIENTO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CODIGO_EQUIPO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FRECUENCIA = table.Column<int>(type: "int", nullable: false),
                    PROXIMA_FECHA = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ID_ESTADO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLAN_MANTENIMIENTO_TB", x => x.ID_PLAN_MANTENIMIENTO);
                    table.ForeignKey(
                        name: "FK_PLAN_MANTENIMIENTO_TB_EQUIPO_TB_CODIGO_EQUIPO",
                        column: x => x.CODIGO_EQUIPO,
                        principalTable: "EQUIPO_TB",
                        principalColumn: "CODIGO_EQUIPO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PLAN_MANTENIMIENTO_TB_ESTADO_TB_ID_ESTADO",
                        column: x => x.ID_ESTADO,
                        principalTable: "ESTADO_TB",
                        principalColumn: "ID_ESTADO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BITACORA_ORDEN_TB",
                columns: table => new
                {
                    ID_BITACORA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ORDEN = table.Column<int>(type: "int", nullable: false),
                    IDENTIFICACION = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FECHA = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ESTADO_ANTERIOR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ESTADO_NUEVO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    COMENTARIO = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BITACORA_ORDEN_TB", x => x.ID_BITACORA);
                    table.ForeignKey(
                        name: "FK_BITACORA_ORDEN_TB_ORDEN_TB_ORDEN",
                        column: x => x.ORDEN,
                        principalTable: "ORDEN_TB",
                        principalColumn: "NUMERO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BITACORA_ORDEN_TB_USUARIO_TB_IDENTIFICACION",
                        column: x => x.IDENTIFICACION,
                        principalTable: "USUARIO_TB",
                        principalColumn: "IDENTIFICACION",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FALLA_REGISTRADA_TB",
                columns: table => new
                {
                    ID_FALLA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CODIGO_EQUIPO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FECHA = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ID_TIPO_GRAVEDAD = table.Column<int>(type: "int", nullable: false),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ORDEN_ASOCIADA = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FALLA_REGISTRADA_TB", x => x.ID_FALLA);
                    table.ForeignKey(
                        name: "FK_FALLA_REGISTRADA_TB_EQUIPO_TB_CODIGO_EQUIPO",
                        column: x => x.CODIGO_EQUIPO,
                        principalTable: "EQUIPO_TB",
                        principalColumn: "CODIGO_EQUIPO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FALLA_REGISTRADA_TB_ORDEN_TB_ORDEN_ASOCIADA",
                        column: x => x.ORDEN_ASOCIADA,
                        principalTable: "ORDEN_TB",
                        principalColumn: "NUMERO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FALLA_REGISTRADA_TB_TIPO_GRAVEDAD_TB_ID_TIPO_GRAVEDAD",
                        column: x => x.ID_TIPO_GRAVEDAD,
                        principalTable: "TIPO_GRAVEDAD_TB",
                        principalColumn: "ID_TIPO_GRAVEDAD",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ORDEN_REPUESTO_TB",
                columns: table => new
                {
                    NUMERO = table.Column<int>(type: "int", nullable: false),
                    CODIGO_REPUESTO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CANTIDAD_USADA = table.Column<int>(type: "int", nullable: false),
                    COSTO_APLICADO = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDEN_REPUESTO_TB", x => new { x.NUMERO, x.CODIGO_REPUESTO });
                    table.ForeignKey(
                        name: "FK_ORDEN_REPUESTO_TB_ORDEN_TB_NUMERO",
                        column: x => x.NUMERO,
                        principalTable: "ORDEN_TB",
                        principalColumn: "NUMERO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ORDEN_REPUESTO_TB_REPUESTO_TB_CODIGO_REPUESTO",
                        column: x => x.CODIGO_REPUESTO,
                        principalTable: "REPUESTO_TB",
                        principalColumn: "CODIGO_REPUESTO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ESTADO_TB",
                columns: new[] { "ID_ESTADO", "NOMBRE" },
                values: new object[,]
                {
                    { 1, "Activo" },
                    { 2, "Inactivo" },
                    { 3, "Operativo" },
                    { 4, "En Mantenimiento" },
                    { 5, "En Riesgo" },
                    { 6, "Dado de Baja" },
                    { 7, "Abierta" },
                    { 8, "Asignada" },
                    { 9, "En Proceso" },
                    { 10, "En Espera de Repuesto" },
                    { 11, "Cerrada" },
                    { 12, "Escalada" }
                });

            migrationBuilder.InsertData(
                table: "ESPECIALIDAD_TB",
                columns: new[] { "ID_ESPECIALIDAD", "ID_ESTADO", "NOMBRE" },
                values: new object[,]
                {
                    { 1, 1, "Mecánica Industrial y Neumática" },
                    { 2, 1, "Electrónica y Automatización" },
                    { 3, 1, "Equipos Biomédicos" },
                    { 4, 1, "Infraestructura de Servidores" },
                    { 5, 1, "Refrigeración y Climatización" }
                });

            migrationBuilder.InsertData(
                table: "MARCA_TB",
                columns: new[] { "ID_MARCA", "ID_ESTADO", "NOMBRE" },
                values: new object[,]
                {
                    { 1, 1, "Dell Technologies" },
                    { 2, 1, "KUKA Robotics" },
                    { 3, 1, "General Electric Healthcare" },
                    { 4, 1, "Atlas Copco" },
                    { 5, 1, "Caterpillar Energy" }
                });

            migrationBuilder.InsertData(
                table: "MODELO_TB",
                columns: new[] { "ID_MODELO", "ID_ESTADO", "NOMBRE" },
                values: new object[,]
                {
                    { 1, 1, "PowerEdge R750 Enterprise" },
                    { 2, 1, "KR CYBERTECH Nano" },
                    { 3, 1, "Optima CT660 Medical" },
                    { 4, 1, "GA 37 VSD+ Screw Compressor" },
                    { 5, 1, "Cat C15 ACERT Generator" }
                });

            migrationBuilder.InsertData(
                table: "REPUESTO_TB",
                columns: new[] { "CODIGO_REPUESTO", "COSTO_UNITARIO", "ID_ESTADO", "NOMBRE", "STOCK", "STOCK_MINIMO" },
                values: new object[,]
                {
                    { "REP-001", 45.00m, 1, "Filtro de Aire Cartucho Micrónico", 24, 10 },
                    { "REP-002", 32.50m, 1, "Aceite Sintético ISO VG 68 (Galón)", 50, 15 },
                    { "REP-003", 275.00m, 1, "Disco SSD Enterprise NVMe 1.92TB", 8, 5 },
                    { "REP-004", 68.00m, 1, "Correa Dentada de Transmisión HTD", 3, 8 },
                    { "REP-005", 58.00m, 1, "Sensor Inductivo M12 24VDC", 14, 6 },
                    { "REP-006", 85.00m, 1, "Rodamiento de Rodillos Cónicos SKF", 2, 10 },
                    { "REP-007", 110.00m, 1, "Válvula Reguladora de Presión Neumática", 16, 5 },
                    { "REP-008", 210.00m, 1, "Batería Plomo-Ácido AGM 12V 100Ah", 1, 4 }
                });

            migrationBuilder.InsertData(
                table: "TIPO_EQUIPO_TB",
                columns: new[] { "ID_TIPO_EQUIPO", "ID_ESTADO", "NOMBRE" },
                values: new object[,]
                {
                    { 1, 1, "Servidor Rack de Cómputo" },
                    { 2, 1, "Brazo Robótico Articulado" },
                    { 3, 1, "Tomógrafo Clínico" },
                    { 4, 1, "Compresor de Aire Industrial" },
                    { 5, 1, "Generador Eléctrico Diesel" }
                });

            migrationBuilder.InsertData(
                table: "TIPO_GRAVEDAD_TB",
                columns: new[] { "ID_TIPO_GRAVEDAD", "ID_ESTADO", "NOMBRE" },
                values: new object[,]
                {
                    { 1, 1, "Baja" },
                    { 2, 1, "Media" },
                    { 3, 1, "Alta" },
                    { 4, 1, "Critica" }
                });

            migrationBuilder.InsertData(
                table: "TIPO_ORDEN_TB",
                columns: new[] { "ID_TIPO_ORDEN", "ID_ESTADO", "NOMBRE" },
                values: new object[,]
                {
                    { 1, 1, "Preventiva" },
                    { 2, 1, "Correctiva" }
                });

            migrationBuilder.InsertData(
                table: "TIPO_USUARIO_TB",
                columns: new[] { "ID_TIPO_USUARIO", "ID_ESTADO", "NOMBRE" },
                values: new object[,]
                {
                    { 1, 1, "Administrador" },
                    { 2, 1, "Supervisor" },
                    { 3, 1, "Tecnico" },
                    { 4, 1, "Solicitante" }
                });

            migrationBuilder.InsertData(
                table: "UBICACION_TB",
                columns: new[] { "ID_UBICACION", "ID_ESTADO", "NOMBRE" },
                values: new object[,]
                {
                    { 1, 1, "Centro de Datos Principal - Piso 2" },
                    { 2, 1, "Planta de Ensamble - Bahía A3" },
                    { 3, 1, "Unidad de Diagnóstico Clínico" },
                    { 4, 1, "Taller Central de Neumática" },
                    { 5, 1, "Patio de Fuerza y Generación" }
                });

            migrationBuilder.InsertData(
                table: "EQUIPO_TB",
                columns: new[] { "CODIGO_EQUIPO", "FECHA_ADQUISICION", "ID_ESTADO", "ID_MARCA", "ID_MODELO", "ID_TIPO_EQUIPO", "ID_UBICACION", "NOMBRE" },
                values: new object[,]
                {
                    { "EQ-001", new DateTime(2023, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, 1, 1, 1, "Servidor Base de Datos ERP" },
                    { "EQ-002", new DateTime(2022, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, 2, 2, 2, "Robot de Soldadura Automatizada" },
                    { "EQ-003", new DateTime(2021, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 3, 3, 3, 3, "Tomógrafo Computarizado Principal" },
                    { "EQ-004", new DateTime(2020, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 4, 4, 4, 4, "Compresor de Aire Línea 2" },
                    { "EQ-005", new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 5, 5, 5, 5, "Generador Diesel Emergencia 500kVA" },
                    { "EQ-006", new DateTime(2023, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, 1, 1, 1, "Servidor Web Cluster Nodo 1" },
                    { "EQ-007", new DateTime(2022, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, 2, 2, 2, "Robot de Pintura y Acabado" },
                    { "EQ-008", new DateTime(2015, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 4, 4, 4, 4, "Compresor Reciprocante Antiguo" }
                });

            migrationBuilder.InsertData(
                table: "USUARIO_TB",
                columns: new[] { "IDENTIFICACION", "CORREO", "DISPONIBILIDAD", "ID_ESPECIALIDAD", "ID_ESTADO", "ID_TIPO_USUARIO", "NOMBRE", "PASSWORD_HASH", "PRIMER_APELLIDO", "SEGUNDO_APELLIDO" },
                values: new object[,]
                {
                    { "USR-001", "admin@mantenimiento.com", true, null, 1, 1, "Carlos", "AQsWISw3Qk1YY2VveYONl6GUDdl4trcHnw7gZp00WlUYFUswQiNOaAEUypVTaqUxTg==", "Mora", "Soto" },
                    { "USR-002", "supervisor@mantenimiento.com", true, null, 1, 2, "Elena", "AQsWISw3Qk1YY2VveYONl6GZSFxdRHC00SwWBpbDilZxtuzcQfid043cCouSgKVjiw==", "Vargas", "Castro" },
                    { "USR-003", "tecnico1@mantenimiento.com", true, 1, 1, 3, "Javier", "AQsWISw3Qk1YY2VveYONl6G0QCT6olmdAI5lH5ZeVXahC/jzgcrM+Rqm3MEmzEHn+w==", "Rojas", "Méndez" },
                    { "USR-004", "tecnico2@mantenimiento.com", true, 2, 1, 3, "Andrés", "AQsWISw3Qk1YY2VveYONl6G0QCT6olmdAI5lH5ZeVXahC/jzgcrM+Rqm3MEmzEHn+w==", "Navarro", "Pérez" },
                    { "USR-005", "solicitante@mantenimiento.com", true, null, 1, 4, "María", "AQsWISw3Qk1YY2VveYONl6F2cu6rnSK2MDnqBwENW5Dc8XT41MUitdbvn3VGpOudQQ==", "Solano", "Chaves" },
                    { "USR-006", "solicitante2@mantenimiento.com", true, null, 1, 4, "Roberto", "AQsWISw3Qk1YY2VveYONl6F2cu6rnSK2MDnqBwENW5Dc8XT41MUitdbvn3VGpOudQQ==", "Gómez", "Alvarado" }
                });

            migrationBuilder.InsertData(
                table: "ORDEN_TB",
                columns: new[] { "NUMERO", "ID_EQUIPO", "DESCRIPCION", "FECHA_CREACION", "FECHA_LIMITE", "ID_ESTADO", "ID_TIPO_ORDEN", "IDENTIFICACION_SOLICITANTE", "IDENTIFICACION_TECNICO", "JUSTIFICACION_SIN_REPUESTOS", "PRIORIDAD", "TIEMPO_INVERTIDO" },
                values: new object[,]
                {
                    { 1, "EQ-002", "Orden de trabajo #1: Mantenimiento preventivo programado sobre EQ-002.", new DateTime(2026, 8, 2, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 12, 20, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-006", null, null, "Baja", 2.25m },
                    { 2, "EQ-003", "Orden de trabajo #2: Mantenimiento preventivo programado sobre EQ-003.", new DateTime(2026, 8, 4, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 9, 8, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-005", null, null, "Media", 3.00m },
                    { 3, "EQ-004", "Orden de trabajo #3: Mantenimiento correctivo por incidencia sobre EQ-004.", new DateTime(2026, 8, 5, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 7, 20, 0, 0, 0, DateTimeKind.Utc), 11, 2, "USR-006", null, null, "Alta", 3.75m }
                });

            migrationBuilder.InsertData(
                table: "ORDEN_TB",
                columns: new[] { "NUMERO", "ID_EQUIPO", "DESCRIPCION", "FECHA_CREACION", "FECHA_LIMITE", "ID_ESTADO", "ID_TIPO_ORDEN", "IDENTIFICACION_SOLICITANTE", "IDENTIFICACION_TECNICO", "JUSTIFICACION_SIN_REPUESTOS", "PRIORIDAD", "SIN_REPUESTOS", "TIEMPO_INVERTIDO" },
                values: new object[] { 4, "EQ-005", "Orden de trabajo #4: Mantenimiento preventivo programado sobre EQ-005.", new DateTime(2026, 8, 7, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 8, 8, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-005", null, "Mantenimiento preventivo de inspección visual, calibración y limpieza sin sustitución de partes.", "Critica", true, 4.50m });

            migrationBuilder.InsertData(
                table: "ORDEN_TB",
                columns: new[] { "NUMERO", "ID_EQUIPO", "DESCRIPCION", "FECHA_CREACION", "FECHA_LIMITE", "ID_ESTADO", "ID_TIPO_ORDEN", "IDENTIFICACION_SOLICITANTE", "IDENTIFICACION_TECNICO", "JUSTIFICACION_SIN_REPUESTOS", "PRIORIDAD", "TIEMPO_INVERTIDO" },
                values: new object[,]
                {
                    { 5, "EQ-006", "Orden de trabajo #5: Mantenimiento preventivo programado sobre EQ-006.", new DateTime(2026, 8, 8, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 18, 20, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-006", null, null, "Baja", 5.25m },
                    { 6, "EQ-007", "Orden de trabajo #6: Mantenimiento correctivo por incidencia sobre EQ-007.", new DateTime(2026, 8, 10, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 12, 8, 0, 0, 0, DateTimeKind.Utc), 11, 2, "USR-005", "USR-003", null, "Alta", 6.00m },
                    { 7, "EQ-001", "Orden de trabajo #7: Mantenimiento preventivo programado sobre EQ-001.", new DateTime(2026, 8, 11, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 21, 20, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-006", "USR-004", null, "Baja", 6.75m },
                    { 8, "EQ-002", "Orden de trabajo #8: Mantenimiento preventivo programado sobre EQ-002.", new DateTime(2026, 8, 13, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 14, 8, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-005", "USR-003", null, "Critica", 7.50m }
                });

            migrationBuilder.InsertData(
                table: "ORDEN_TB",
                columns: new[] { "NUMERO", "ID_EQUIPO", "DESCRIPCION", "FECHA_CREACION", "FECHA_LIMITE", "ID_ESTADO", "ID_TIPO_ORDEN", "IDENTIFICACION_SOLICITANTE", "IDENTIFICACION_TECNICO", "JUSTIFICACION_SIN_REPUESTOS", "PRIORIDAD", "SIN_REPUESTOS", "TIEMPO_INVERTIDO" },
                values: new object[] { 9, "EQ-003", "Orden de trabajo #9: Mantenimiento correctivo por incidencia sobre EQ-003.", new DateTime(2026, 8, 14, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 16, 20, 0, 0, 0, DateTimeKind.Utc), 11, 2, "USR-006", "USR-004", "Mantenimiento preventivo de inspección visual, calibración y limpieza sin sustitución de partes.", "Alta", true, 8.25m });

            migrationBuilder.InsertData(
                table: "ORDEN_TB",
                columns: new[] { "NUMERO", "ID_EQUIPO", "DESCRIPCION", "FECHA_CREACION", "FECHA_LIMITE", "ID_ESTADO", "ID_TIPO_ORDEN", "IDENTIFICACION_SOLICITANTE", "IDENTIFICACION_TECNICO", "JUSTIFICACION_SIN_REPUESTOS", "PRIORIDAD", "TIEMPO_INVERTIDO" },
                values: new object[,]
                {
                    { 10, "EQ-004", "Orden de trabajo #10: Mantenimiento preventivo programado sobre EQ-004.", new DateTime(2026, 8, 16, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 21, 8, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-005", "USR-003", null, "Media", 9.00m },
                    { 11, "EQ-005", "Orden de trabajo #11: Mantenimiento preventivo programado sobre EQ-005.", new DateTime(2026, 8, 17, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 27, 20, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-006", "USR-004", null, "Baja", 9.75m },
                    { 12, "EQ-006", "Orden de trabajo #12: Mantenimiento correctivo por incidencia sobre EQ-006.", new DateTime(2026, 8, 19, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 20, 8, 0, 0, 0, DateTimeKind.Utc), 11, 2, "USR-005", "USR-003", null, "Critica", 10.50m },
                    { 13, "EQ-007", "Orden de trabajo #13: Mantenimiento preventivo programado sobre EQ-007.", new DateTime(2026, 8, 20, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 30, 20, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-006", "USR-004", null, "Baja", 11.25m }
                });

            migrationBuilder.InsertData(
                table: "ORDEN_TB",
                columns: new[] { "NUMERO", "ID_EQUIPO", "DESCRIPCION", "FECHA_CREACION", "FECHA_LIMITE", "ID_ESTADO", "ID_TIPO_ORDEN", "IDENTIFICACION_SOLICITANTE", "IDENTIFICACION_TECNICO", "JUSTIFICACION_SIN_REPUESTOS", "PRIORIDAD", "SIN_REPUESTOS", "TIEMPO_INVERTIDO" },
                values: new object[] { 14, "EQ-001", "Orden de trabajo #14: Mantenimiento preventivo programado sobre EQ-001.", new DateTime(2026, 8, 22, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 27, 8, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-005", "USR-003", "Mantenimiento preventivo de inspección visual, calibración y limpieza sin sustitución de partes.", "Media", true, 12.00m });

            migrationBuilder.InsertData(
                table: "ORDEN_TB",
                columns: new[] { "NUMERO", "ID_EQUIPO", "DESCRIPCION", "FECHA_CREACION", "FECHA_LIMITE", "ID_ESTADO", "ID_TIPO_ORDEN", "IDENTIFICACION_SOLICITANTE", "IDENTIFICACION_TECNICO", "JUSTIFICACION_SIN_REPUESTOS", "PRIORIDAD", "TIEMPO_INVERTIDO" },
                values: new object[,]
                {
                    { 15, "EQ-002", "Orden de trabajo #15: Mantenimiento correctivo por incidencia sobre EQ-002.", new DateTime(2026, 8, 23, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 25, 20, 0, 0, 0, DateTimeKind.Utc), 11, 2, "USR-006", "USR-004", null, "Alta", 12.75m },
                    { 16, "EQ-003", "Orden de trabajo #16: Mantenimiento preventivo programado sobre EQ-003.", new DateTime(2026, 8, 25, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 26, 8, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-005", "USR-003", null, "Critica", 13.50m },
                    { 17, "EQ-004", "Orden de trabajo #17: Mantenimiento preventivo programado sobre EQ-004.", new DateTime(2026, 8, 26, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 20, 0, 0, 0, DateTimeKind.Utc), 11, 1, "USR-006", "USR-004", null, "Baja", 14.25m },
                    { 18, "EQ-005", "Orden de trabajo #18: Mantenimiento correctivo por incidencia sobre EQ-005.", new DateTime(2026, 8, 28, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 30, 8, 0, 0, 0, DateTimeKind.Utc), 11, 2, "USR-005", "USR-003", null, "Alta", 15.00m },
                    { 19, "EQ-006", "Orden de trabajo #19: Mantenimiento preventivo programado sobre EQ-006.", new DateTime(2026, 8, 29, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 8, 20, 0, 0, 0, DateTimeKind.Utc), 9, 1, "USR-006", "USR-004", null, "Baja", 2.0m },
                    { 20, "EQ-007", "Orden de trabajo #20: Mantenimiento preventivo programado sobre EQ-007.", new DateTime(2026, 8, 31, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Utc), 9, 1, "USR-005", "USR-003", null, "Critica", 2.0m },
                    { 21, "EQ-001", "Orden de trabajo #21: Mantenimiento correctivo por incidencia sobre EQ-001.", new DateTime(2026, 9, 1, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 3, 20, 0, 0, 0, DateTimeKind.Utc), 9, 2, "USR-006", "USR-004", null, "Alta", 2.0m },
                    { 22, "EQ-002", "Orden de trabajo #22: Mantenimiento preventivo programado sobre EQ-002.", new DateTime(2026, 9, 3, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 8, 8, 0, 0, 0, DateTimeKind.Utc), 9, 1, "USR-005", "USR-003", null, "Media", 2.0m },
                    { 23, "EQ-003", "Orden de trabajo #23: Mantenimiento preventivo programado sobre EQ-003.", new DateTime(2026, 9, 4, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 14, 20, 0, 0, 0, DateTimeKind.Utc), 9, 1, "USR-006", "USR-004", null, "Baja", 2.0m }
                });

            migrationBuilder.InsertData(
                table: "ORDEN_TB",
                columns: new[] { "NUMERO", "ID_EQUIPO", "DESCRIPCION", "FECHA_CREACION", "FECHA_LIMITE", "ID_ESTADO", "ID_TIPO_ORDEN", "IDENTIFICACION_SOLICITANTE", "IDENTIFICACION_TECNICO", "JUSTIFICACION_SIN_REPUESTOS", "PRIORIDAD" },
                values: new object[,]
                {
                    { 24, "EQ-004", "Orden de trabajo #24: Mantenimiento correctivo por incidencia sobre EQ-004.", new DateTime(2026, 9, 6, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 7, 8, 0, 0, 0, DateTimeKind.Utc), 8, 2, "USR-005", "USR-003", null, "Critica" },
                    { 25, "EQ-005", "Orden de trabajo #25: Mantenimiento preventivo programado sobre EQ-005.", new DateTime(2026, 9, 7, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 17, 20, 0, 0, 0, DateTimeKind.Utc), 8, 1, "USR-006", "USR-004", null, "Baja" },
                    { 26, "EQ-006", "Orden de trabajo #26: Mantenimiento preventivo programado sobre EQ-006.", new DateTime(2026, 9, 9, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 14, 8, 0, 0, 0, DateTimeKind.Utc), 8, 1, "USR-005", "USR-003", null, "Media" },
                    { 27, "EQ-007", "Orden de trabajo #27: Mantenimiento correctivo por incidencia sobre EQ-007.", new DateTime(2026, 9, 10, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 12, 20, 0, 0, 0, DateTimeKind.Utc), 8, 2, "USR-006", "USR-004", null, "Alta" },
                    { 28, "EQ-001", "Orden de trabajo #28: Mantenimiento preventivo programado sobre EQ-001.", new DateTime(2026, 9, 12, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 13, 8, 0, 0, 0, DateTimeKind.Utc), 7, 1, "USR-005", null, null, "Critica" },
                    { 29, "EQ-002", "Orden de trabajo #29: Mantenimiento preventivo programado sobre EQ-002.", new DateTime(2026, 9, 13, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 23, 20, 0, 0, 0, DateTimeKind.Utc), 7, 1, "USR-006", null, null, "Baja" },
                    { 30, "EQ-003", "Orden de trabajo #30: Mantenimiento correctivo por incidencia sobre EQ-003.", new DateTime(2026, 9, 15, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 17, 8, 0, 0, 0, DateTimeKind.Utc), 7, 2, "USR-005", null, null, "Alta" },
                    { 31, "EQ-004", "Orden de trabajo #31: Mantenimiento preventivo programado sobre EQ-004.", new DateTime(2026, 9, 16, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 26, 20, 0, 0, 0, DateTimeKind.Utc), 10, 1, "USR-006", "USR-004", null, "Baja" },
                    { 32, "EQ-005", "Orden de trabajo #32: Mantenimiento preventivo programado sobre EQ-005.", new DateTime(2026, 9, 18, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 19, 8, 0, 0, 0, DateTimeKind.Utc), 12, 1, "USR-005", "USR-003", null, "Critica" }
                });

            migrationBuilder.InsertData(
                table: "PLAN_MANTENIMIENTO_TB",
                columns: new[] { "ID_PLAN_MANTENIMIENTO", "CODIGO_EQUIPO", "FRECUENCIA", "ID_ESTADO", "PROXIMA_FECHA" },
                values: new object[,]
                {
                    { 1, "EQ-001", 30, 1, new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "EQ-002", 60, 1, new DateTime(2026, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "EQ-003", 90, 1, new DateTime(2026, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "EQ-004", 45, 1, new DateTime(2026, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "EQ-005", 180, 1, new DateTime(2026, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "BITACORA_ORDEN_TB",
                columns: new[] { "ID_BITACORA", "COMENTARIO", "ESTADO_ANTERIOR", "ESTADO_NUEVO", "FECHA", "IDENTIFICACION", "ORDEN" },
                values: new object[,]
                {
                    { 1, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 2, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 1 },
                    { 2, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 3, 0, 0, 0, 0, DateTimeKind.Utc), "USR-003", 1 },
                    { 3, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 3, 8, 0, 0, 0, DateTimeKind.Utc), "USR-003", 1 },
                    { 4, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 4, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 2 },
                    { 5, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 4, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 2 },
                    { 6, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 4, 20, 0, 0, 0, DateTimeKind.Utc), "USR-003", 2 },
                    { 7, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 5, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 3 },
                    { 8, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 6, 0, 0, 0, 0, DateTimeKind.Utc), "USR-003", 3 },
                    { 9, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 6, 8, 0, 0, 0, DateTimeKind.Utc), "USR-003", 3 },
                    { 10, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 7, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 4 },
                    { 11, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 7, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 4 },
                    { 12, "Cierre de orden sin repuestos: Mantenimiento preventivo de inspección visual, calibración y limpieza sin sustitución de partes.", "En Proceso", "Cerrada", new DateTime(2026, 8, 7, 20, 0, 0, 0, DateTimeKind.Utc), "USR-003", 4 },
                    { 13, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 8, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 5 },
                    { 14, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "USR-003", 5 },
                    { 15, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 9, 8, 0, 0, 0, DateTimeKind.Utc), "USR-003", 5 },
                    { 16, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 10, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 6 },
                    { 17, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 8, 10, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 6 },
                    { 18, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 10, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 6 },
                    { 19, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 10, 20, 0, 0, 0, DateTimeKind.Utc), "USR-003", 6 },
                    { 20, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 11, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 7 },
                    { 21, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 8, 11, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 7 },
                    { 22, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), "USR-004", 7 },
                    { 23, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 12, 8, 0, 0, 0, DateTimeKind.Utc), "USR-004", 7 },
                    { 24, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 13, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 8 },
                    { 25, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 8, 13, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 8 },
                    { 26, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 13, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 8 },
                    { 27, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 13, 20, 0, 0, 0, DateTimeKind.Utc), "USR-003", 8 },
                    { 28, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 14, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 9 },
                    { 29, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 8, 14, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 9 },
                    { 30, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 15, 0, 0, 0, 0, DateTimeKind.Utc), "USR-004", 9 },
                    { 31, "Cierre de orden sin repuestos: Mantenimiento preventivo de inspección visual, calibración y limpieza sin sustitución de partes.", "En Proceso", "Cerrada", new DateTime(2026, 8, 15, 8, 0, 0, 0, DateTimeKind.Utc), "USR-004", 9 },
                    { 32, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 16, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 10 },
                    { 33, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 8, 16, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 10 },
                    { 34, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 16, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 10 },
                    { 35, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 16, 20, 0, 0, 0, DateTimeKind.Utc), "USR-003", 10 },
                    { 36, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 17, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 11 },
                    { 37, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 8, 17, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 11 },
                    { 38, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Utc), "USR-004", 11 },
                    { 39, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 18, 8, 0, 0, 0, DateTimeKind.Utc), "USR-004", 11 },
                    { 40, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 19, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 12 },
                    { 41, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 8, 19, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 12 },
                    { 42, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 19, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 12 },
                    { 43, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 19, 20, 0, 0, 0, DateTimeKind.Utc), "USR-003", 12 },
                    { 44, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 20, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 13 },
                    { 45, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 8, 20, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 13 },
                    { 46, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "USR-004", 13 },
                    { 47, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 21, 8, 0, 0, 0, DateTimeKind.Utc), "USR-004", 13 },
                    { 48, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 22, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 14 },
                    { 49, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 8, 22, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 14 },
                    { 50, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 22, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 14 },
                    { 51, "Cierre de orden sin repuestos: Mantenimiento preventivo de inspección visual, calibración y limpieza sin sustitución de partes.", "En Proceso", "Cerrada", new DateTime(2026, 8, 22, 20, 0, 0, 0, DateTimeKind.Utc), "USR-003", 14 },
                    { 52, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 23, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 15 },
                    { 53, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 8, 23, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 15 },
                    { 54, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Utc), "USR-004", 15 },
                    { 55, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 24, 8, 0, 0, 0, DateTimeKind.Utc), "USR-004", 15 },
                    { 56, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 25, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 16 },
                    { 57, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 8, 25, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 16 },
                    { 58, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 25, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 16 },
                    { 59, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 25, 20, 0, 0, 0, DateTimeKind.Utc), "USR-003", 16 },
                    { 60, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 26, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 17 },
                    { 61, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 8, 26, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 17 },
                    { 62, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 27, 0, 0, 0, 0, DateTimeKind.Utc), "USR-004", 17 },
                    { 63, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 27, 8, 0, 0, 0, DateTimeKind.Utc), "USR-004", 17 },
                    { 64, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 28, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 18 },
                    { 65, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 8, 28, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 18 },
                    { 66, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 28, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 18 },
                    { 67, "Cierre exitoso de la orden tras verificación de funcionamiento.", "En Proceso", "Cerrada", new DateTime(2026, 8, 28, 20, 0, 0, 0, DateTimeKind.Utc), "USR-003", 18 },
                    { 68, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 29, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 19 },
                    { 69, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 8, 29, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 19 },
                    { 70, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "USR-004", 19 },
                    { 71, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 8, 31, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 20 },
                    { 72, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 8, 31, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 20 },
                    { 73, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 8, 31, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 20 },
                    { 74, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 1, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 21 },
                    { 75, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 9, 1, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 21 },
                    { 76, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 9, 2, 0, 0, 0, 0, DateTimeKind.Utc), "USR-004", 21 },
                    { 77, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 3, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 22 },
                    { 78, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 9, 3, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 22 },
                    { 79, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 9, 3, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 22 },
                    { 80, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 4, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 23 },
                    { 81, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 9, 4, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 23 },
                    { 82, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Utc), "USR-004", 23 },
                    { 83, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 6, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 24 },
                    { 84, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 9, 6, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 24 },
                    { 85, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 7, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 25 },
                    { 86, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 9, 7, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 25 },
                    { 87, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 9, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 26 },
                    { 88, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 9, 9, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 26 },
                    { 89, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 10, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 27 },
                    { 90, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 9, 10, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 27 },
                    { 91, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 12, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 28 },
                    { 92, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 13, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 29 },
                    { 93, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 15, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 30 },
                    { 94, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 16, 20, 0, 0, 0, DateTimeKind.Utc), "USR-006", 31 },
                    { 95, "Asignación formal de la orden al técnico USR-004.", "Abierta", "Asignada", new DateTime(2026, 9, 16, 22, 0, 0, 0, DateTimeKind.Utc), "USR-002", 31 },
                    { 96, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "USR-004", 31 },
                    { 97, "Creación y registro de la orden de trabajo.", "Ninguno", "Abierta", new DateTime(2026, 9, 18, 8, 0, 0, 0, DateTimeKind.Utc), "USR-005", 32 },
                    { 98, "Asignación formal de la orden al técnico USR-003.", "Abierta", "Asignada", new DateTime(2026, 9, 18, 10, 0, 0, 0, DateTimeKind.Utc), "USR-002", 32 },
                    { 99, "Inicio de labores de diagnóstico y ejecución de mantenimiento.", "Asignada", "En Proceso", new DateTime(2026, 9, 18, 12, 0, 0, 0, DateTimeKind.Utc), "USR-003", 32 }
                });

            migrationBuilder.InsertData(
                table: "FALLA_REGISTRADA_TB",
                columns: new[] { "ID_FALLA", "CODIGO_EQUIPO", "DESCRIPCION", "FECHA", "ID_TIPO_GRAVEDAD", "ORDEN_ASOCIADA" },
                values: new object[,]
                {
                    { 1, "EQ-004", "Falla reportada en equipo EQ-004: anomalía en operación continua que requirió orden #3.", new DateTime(2026, 8, 5, 20, 0, 0, 0, DateTimeKind.Utc), 3, 3 },
                    { 2, "EQ-007", "Falla reportada en equipo EQ-007: anomalía en operación continua que requirió orden #6.", new DateTime(2026, 8, 10, 8, 0, 0, 0, DateTimeKind.Utc), 3, 6 },
                    { 3, "EQ-003", "Falla reportada en equipo EQ-003: anomalía en operación continua que requirió orden #9.", new DateTime(2026, 8, 14, 20, 0, 0, 0, DateTimeKind.Utc), 3, 9 },
                    { 4, "EQ-006", "Falla reportada en equipo EQ-006: anomalía en operación continua que requirió orden #12.", new DateTime(2026, 8, 19, 8, 0, 0, 0, DateTimeKind.Utc), 4, 12 },
                    { 5, "EQ-002", "Falla reportada en equipo EQ-002: anomalía en operación continua que requirió orden #15.", new DateTime(2026, 8, 23, 20, 0, 0, 0, DateTimeKind.Utc), 3, 15 },
                    { 6, "EQ-005", "Falla reportada en equipo EQ-005: anomalía en operación continua que requirió orden #18.", new DateTime(2026, 8, 28, 8, 0, 0, 0, DateTimeKind.Utc), 3, 18 },
                    { 7, "EQ-001", "Falla reportada en equipo EQ-001: anomalía en operación continua que requirió orden #21.", new DateTime(2026, 9, 1, 20, 0, 0, 0, DateTimeKind.Utc), 3, 21 },
                    { 8, "EQ-004", "Falla reportada en equipo EQ-004: anomalía en operación continua que requirió orden #24.", new DateTime(2026, 9, 6, 8, 0, 0, 0, DateTimeKind.Utc), 4, 24 },
                    { 9, "EQ-007", "Falla reportada en equipo EQ-007: anomalía en operación continua que requirió orden #27.", new DateTime(2026, 9, 10, 20, 0, 0, 0, DateTimeKind.Utc), 3, 27 },
                    { 10, "EQ-003", "Falla reportada en equipo EQ-003: anomalía en operación continua que requirió orden #30.", new DateTime(2026, 9, 15, 8, 0, 0, 0, DateTimeKind.Utc), 3, 30 },
                    { 11, "EQ-004", "Fuga recurrente de presión en válvula de descarga.", new DateTime(2026, 9, 15, 10, 0, 0, 0, DateTimeKind.Utc), 3, 10 },
                    { 12, "EQ-004", "Sobrecalentamiento en cabezal de compresión.", new DateTime(2026, 8, 20, 14, 30, 0, 0, DateTimeKind.Utc), 3, 12 },
                    { 13, "EQ-004", "Paro imprevisto por vibración excesiva en rodamientos.", new DateTime(2026, 7, 25, 9, 15, 0, 0, DateTimeKind.Utc), 4, 15 }
                });

            migrationBuilder.InsertData(
                table: "ORDEN_REPUESTO_TB",
                columns: new[] { "CODIGO_REPUESTO", "NUMERO", "CANTIDAD_USADA", "COSTO_APLICADO" },
                values: new object[,]
                {
                    { "REP-005", 1, 2, 58.00m },
                    { "REP-001", 2, 3, 45.00m },
                    { "REP-002", 3, 1, 32.50m },
                    { "REP-005", 5, 3, 58.00m },
                    { "REP-001", 6, 1, 45.00m },
                    { "REP-005", 7, 2, 58.00m },
                    { "REP-001", 8, 3, 45.00m },
                    { "REP-001", 10, 2, 45.00m },
                    { "REP-005", 11, 3, 58.00m },
                    { "REP-001", 12, 1, 45.00m },
                    { "REP-005", 13, 2, 58.00m },
                    { "REP-002", 15, 1, 32.50m },
                    { "REP-001", 16, 2, 45.00m },
                    { "REP-005", 17, 3, 58.00m },
                    { "REP-001", 18, 1, 45.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BITACORA_ORDEN_TB_IDENTIFICACION",
                table: "BITACORA_ORDEN_TB",
                column: "IDENTIFICACION");

            migrationBuilder.CreateIndex(
                name: "IX_BITACORA_ORDEN_TB_ORDEN",
                table: "BITACORA_ORDEN_TB",
                column: "ORDEN");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPO_TB_ID_ESTADO",
                table: "EQUIPO_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPO_TB_ID_MARCA",
                table: "EQUIPO_TB",
                column: "ID_MARCA");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPO_TB_ID_MODELO",
                table: "EQUIPO_TB",
                column: "ID_MODELO");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPO_TB_ID_TIPO_EQUIPO",
                table: "EQUIPO_TB",
                column: "ID_TIPO_EQUIPO");

            migrationBuilder.CreateIndex(
                name: "IX_EQUIPO_TB_ID_UBICACION",
                table: "EQUIPO_TB",
                column: "ID_UBICACION");

            migrationBuilder.CreateIndex(
                name: "IX_ESPECIALIDAD_TB_ID_ESTADO",
                table: "ESPECIALIDAD_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_FALLA_REGISTRADA_TB_CODIGO_EQUIPO",
                table: "FALLA_REGISTRADA_TB",
                column: "CODIGO_EQUIPO");

            migrationBuilder.CreateIndex(
                name: "IX_FALLA_REGISTRADA_TB_ID_TIPO_GRAVEDAD",
                table: "FALLA_REGISTRADA_TB",
                column: "ID_TIPO_GRAVEDAD");

            migrationBuilder.CreateIndex(
                name: "IX_FALLA_REGISTRADA_TB_ORDEN_ASOCIADA",
                table: "FALLA_REGISTRADA_TB",
                column: "ORDEN_ASOCIADA");

            migrationBuilder.CreateIndex(
                name: "IX_MARCA_TB_ID_ESTADO",
                table: "MARCA_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_MODELO_TB_ID_ESTADO",
                table: "MODELO_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_ORDEN_REPUESTO_TB_CODIGO_REPUESTO",
                table: "ORDEN_REPUESTO_TB",
                column: "CODIGO_REPUESTO");

            migrationBuilder.CreateIndex(
                name: "IX_ORDEN_TB_ID_EQUIPO",
                table: "ORDEN_TB",
                column: "ID_EQUIPO");

            migrationBuilder.CreateIndex(
                name: "IX_ORDEN_TB_ID_ESTADO",
                table: "ORDEN_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_ORDEN_TB_ID_TIPO_ORDEN",
                table: "ORDEN_TB",
                column: "ID_TIPO_ORDEN");

            migrationBuilder.CreateIndex(
                name: "IX_ORDEN_TB_IDENTIFICACION_SOLICITANTE",
                table: "ORDEN_TB",
                column: "IDENTIFICACION_SOLICITANTE");

            migrationBuilder.CreateIndex(
                name: "IX_ORDEN_TB_IDENTIFICACION_TECNICO",
                table: "ORDEN_TB",
                column: "IDENTIFICACION_TECNICO");

            migrationBuilder.CreateIndex(
                name: "IX_PLAN_MANTENIMIENTO_TB_CODIGO_EQUIPO",
                table: "PLAN_MANTENIMIENTO_TB",
                column: "CODIGO_EQUIPO");

            migrationBuilder.CreateIndex(
                name: "IX_PLAN_MANTENIMIENTO_TB_ID_ESTADO",
                table: "PLAN_MANTENIMIENTO_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_REPUESTO_TB_ID_ESTADO",
                table: "REPUESTO_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_TIPO_EQUIPO_TB_ID_ESTADO",
                table: "TIPO_EQUIPO_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_TIPO_GRAVEDAD_TB_ID_ESTADO",
                table: "TIPO_GRAVEDAD_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_TIPO_ORDEN_TB_ID_ESTADO",
                table: "TIPO_ORDEN_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_TIPO_USUARIO_TB_ID_ESTADO",
                table: "TIPO_USUARIO_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_UBICACION_TB_ID_ESTADO",
                table: "UBICACION_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_TB_CORREO",
                table: "USUARIO_TB",
                column: "CORREO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_TB_ID_ESPECIALIDAD",
                table: "USUARIO_TB",
                column: "ID_ESPECIALIDAD");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_TB_ID_ESTADO",
                table: "USUARIO_TB",
                column: "ID_ESTADO");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_TB_ID_TIPO_USUARIO",
                table: "USUARIO_TB",
                column: "ID_TIPO_USUARIO");

            // Creación del procedimiento almacenado para el reporte de consumo de repuestos por período
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE sp_ReporteConsumoRepuestosPorPeriodo
                    @FechaInicio DATETIME2,
                    @FechaFin DATETIME2
                AS
                BEGIN
                    SET NOCOUNT ON;

                    SELECT 
                        r.CODIGO_REPUESTO AS CodigoRepuesto,
                        r.NOMBRE AS NombreRepuesto,
                        ISNULL(SUM(or_tb.CANTIDAD_USADA), 0) AS CantidadTotalConsumida,
                        ISNULL(SUM(or_tb.CANTIDAD_USADA * or_tb.COSTO_APLICADO), 0) AS CostoTotalAcumulado,
                        COUNT(DISTINCT o.NUMERO) AS NumeroDeOrdenes
                    FROM REPUESTO_TB r
                    INNER JOIN ORDEN_REPUESTO_TB or_tb ON r.CODIGO_REPUESTO = or_tb.CODIGO_REPUESTO
                    INNER JOIN ORDEN_TB o ON or_tb.NUMERO = o.NUMERO
                    WHERE o.FECHA_CREACION BETWEEN @FechaInicio AND @FechaFin
                    GROUP BY r.CODIGO_REPUESTO, r.NOMBRE
                    ORDER BY CantidadTotalConsumida DESC;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ReporteConsumoRepuestosPorPeriodo;");

            migrationBuilder.DropTable(
                name: "BITACORA_ORDEN_TB");

            migrationBuilder.DropTable(
                name: "FALLA_REGISTRADA_TB");

            migrationBuilder.DropTable(
                name: "ORDEN_REPUESTO_TB");

            migrationBuilder.DropTable(
                name: "PLAN_MANTENIMIENTO_TB");

            migrationBuilder.DropTable(
                name: "TIPO_GRAVEDAD_TB");

            migrationBuilder.DropTable(
                name: "ORDEN_TB");

            migrationBuilder.DropTable(
                name: "REPUESTO_TB");

            migrationBuilder.DropTable(
                name: "EQUIPO_TB");

            migrationBuilder.DropTable(
                name: "TIPO_ORDEN_TB");

            migrationBuilder.DropTable(
                name: "USUARIO_TB");

            migrationBuilder.DropTable(
                name: "MARCA_TB");

            migrationBuilder.DropTable(
                name: "MODELO_TB");

            migrationBuilder.DropTable(
                name: "TIPO_EQUIPO_TB");

            migrationBuilder.DropTable(
                name: "UBICACION_TB");

            migrationBuilder.DropTable(
                name: "ESPECIALIDAD_TB");

            migrationBuilder.DropTable(
                name: "TIPO_USUARIO_TB");

            migrationBuilder.DropTable(
                name: "ESTADO_TB");
        }
    }
}
