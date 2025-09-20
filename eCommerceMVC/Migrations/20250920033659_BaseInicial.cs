using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceMVC.Migrations
{
    /// <inheritdoc />
    public partial class BaseInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORIA",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IdCategoriaPadre = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORIA", x => x.IdCategoria);
                    table.ForeignKey(
                        name: "FK_CATEGORIA_CATEGORIA_IdCategoriaPadre",
                        column: x => x.IdCategoriaPadre,
                        principalTable: "CATEGORIA",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CLIENTE",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Apellidos = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Correo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Contraseña = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Restablecer = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CLIENTE__D5946642A7D1C625", x => x.IdCliente);
                });

            migrationBuilder.CreateTable(
                name: "CUPON",
                columns: table => new
                {
                    IdCupon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    DescuentoFijo = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    DescuentoPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    MontoMinimo = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    UsosMaximos = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    UsosActuales = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FechaInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUPON", x => x.IdCupon);
                });

            migrationBuilder.CreateTable(
                name: "ESTADO_PEDIDO",
                columns: table => new
                {
                    IdEstadoPedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESTADO_PEDIDO", x => x.IdEstadoPedido);
                });

            migrationBuilder.CreateTable(
                name: "MARCA",
                columns: table => new
                {
                    IdMarca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MARCA__4076A887C7600A88", x => x.IdMarca);
                });

            migrationBuilder.CreateTable(
                name: "METODO_PAGO",
                columns: table => new
                {
                    IdMetodoPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    RequiereDatosAdicionales = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_METODO_PAGO", x => x.IdMetodoPago);
                });

            migrationBuilder.CreateTable(
                name: "DIRECCION_ENVIO",
                columns: table => new
                {
                    IdDireccionEnvio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    NombreCompleto = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    Referencias = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Ciudad = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Provincia = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    CodigoPostal = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Telefono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    EsDireccionPrincipal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DIRECCION_ENVIO", x => x.IdDireccionEnvio);
                    table.ForeignKey(
                        name: "FK_DIRECCION_ENVIO_CLIENTE",
                        column: x => x.IdCliente,
                        principalTable: "CLIENTE",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<int>(type: "int", nullable: true),
                    Nombres = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Correo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Contraseña = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    Restablecer = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__USUARIO__5B65BF97679AF4C5", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK__USUARIO__IdClien__5535A963",
                        column: x => x.IdCliente,
                        principalTable: "CLIENTE",
                        principalColumn: "IdCliente");
                });

            migrationBuilder.CreateTable(
                name: "PRODUCTO",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 400, nullable: true),
                    Descripcion = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 400, nullable: true),
                    IdMarca = table.Column<int>(type: "int", nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    Stock = table.Column<int>(type: "int", nullable: true),
                    RutaImagen = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    NombreImagen = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PRODUCTO__0988921006888CEC", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK__PRODUCTO__IdCate__403A8C7D",
                        column: x => x.IdCategoria,
                        principalTable: "CATEGORIA",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__PRODUCTO__IdMarc__3F466844",
                        column: x => x.IdMarca,
                        principalTable: "MARCA",
                        principalColumn: "IdMarca",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VENTA",
                columns: table => new
                {
                    IdVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<int>(type: "int", nullable: true),
                    TotalProductos = table.Column<int>(type: "int", nullable: true),
                    ImporteTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Contacto = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IdProvincia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Telefono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Direccion = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    IdTransaccion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    FechaVenta = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IdDireccionEnvio = table.Column<int>(type: "int", nullable: true),
                    IdMetodoPago = table.Column<int>(type: "int", nullable: true),
                    IdEstadoPedido = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    IdCupon = table.Column<int>(type: "int", nullable: true),
                    DescuentoAplicado = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    CostoEnvio = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    NotasEspeciales = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    FechaEstimadaEntrega = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VENTA__BC1240BD415D780E", x => x.IdVenta);
                    table.ForeignKey(
                        name: "FK_VENTA_CUPON",
                        column: x => x.IdCupon,
                        principalTable: "CUPON",
                        principalColumn: "IdCupon",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VENTA_DIRECCION_ENVIO",
                        column: x => x.IdDireccionEnvio,
                        principalTable: "DIRECCION_ENVIO",
                        principalColumn: "IdDireccionEnvio",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VENTA_ESTADO_PEDIDO",
                        column: x => x.IdEstadoPedido,
                        principalTable: "ESTADO_PEDIDO",
                        principalColumn: "IdEstadoPedido",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VENTA_METODO_PAGO",
                        column: x => x.IdMetodoPago,
                        principalTable: "METODO_PAGO",
                        principalColumn: "IdMetodoPago",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__VENTA__IdCliente__4D94879B",
                        column: x => x.IdCliente,
                        principalTable: "CLIENTE",
                        principalColumn: "IdCliente");
                });

            migrationBuilder.CreateTable(
                name: "CARRITO",
                columns: table => new
                {
                    IdCarrito = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: true),
                    IdProducto = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    ClienteIdCliente = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CARRITO", x => x.IdCarrito);
                    table.ForeignKey(
                        name: "FK_CARRITO_CLIENTE_ClienteIdCliente",
                        column: x => x.ClienteIdCliente,
                        principalTable: "CLIENTE",
                        principalColumn: "IdCliente");
                    table.ForeignKey(
                        name: "FK_CARRITO_PRODUCTO_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "PRODUCTO",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK_CARRITO_USUARIO",
                        column: x => x.IdUsuario,
                        principalTable: "USUARIO",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "DETALLE_VENTA",
                columns: table => new
                {
                    IdDetalleVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVenta = table.Column<int>(type: "int", nullable: true),
                    IdProducto = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DETALLE___AAA5CEC28EC83973", x => x.IdDetalleVenta);
                    table.ForeignKey(
                        name: "FK__DETALLE_V__IdPro__52593CB8",
                        column: x => x.IdProducto,
                        principalTable: "PRODUCTO",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK__DETALLE_V__IdVen__5165187F",
                        column: x => x.IdVenta,
                        principalTable: "VENTA",
                        principalColumn: "IdVenta");
                });

            migrationBuilder.CreateTable(
                name: "HISTORIAL_PEDIDO",
                columns: table => new
                {
                    IdHistorialPedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVenta = table.Column<int>(type: "int", nullable: false),
                    IdEstadoPedido = table.Column<int>(type: "int", nullable: false),
                    Comentarios = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    FechaCambio = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IdUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORIAL_PEDIDO", x => x.IdHistorialPedido);
                    table.ForeignKey(
                        name: "FK_HISTORIAL_PEDIDO_ESTADO",
                        column: x => x.IdEstadoPedido,
                        principalTable: "ESTADO_PEDIDO",
                        principalColumn: "IdEstadoPedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HISTORIAL_PEDIDO_USUARIO",
                        column: x => x.IdUsuario,
                        principalTable: "USUARIO",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HISTORIAL_PEDIDO_VENTA",
                        column: x => x.IdVenta,
                        principalTable: "VENTA",
                        principalColumn: "IdVenta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CARRITO_ClienteIdCliente",
                table: "CARRITO",
                column: "ClienteIdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_CARRITO_IdProducto",
                table: "CARRITO",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_CARRITO_IdUsuario",
                table: "CARRITO",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORIA_IdCategoriaPadre",
                table: "CATEGORIA",
                column: "IdCategoriaPadre");

            migrationBuilder.CreateIndex(
                name: "UQ_CUPON_CODIGO",
                table: "CUPON",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DETALLE_VENTA_IdProducto",
                table: "DETALLE_VENTA",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_DETALLE_VENTA_IdVenta",
                table: "DETALLE_VENTA",
                column: "IdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_DIRECCION_ENVIO_IdCliente",
                table: "DIRECCION_ENVIO",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORIAL_PEDIDO_IdEstadoPedido",
                table: "HISTORIAL_PEDIDO",
                column: "IdEstadoPedido");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORIAL_PEDIDO_IdUsuario",
                table: "HISTORIAL_PEDIDO",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORIAL_PEDIDO_IdVenta",
                table: "HISTORIAL_PEDIDO",
                column: "IdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTO_IdCategoria",
                table: "PRODUCTO",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTO_IdMarca",
                table: "PRODUCTO",
                column: "IdMarca");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_IdCliente",
                table: "USUARIO",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_VENTA_IdCliente",
                table: "VENTA",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_VENTA_IdCupon",
                table: "VENTA",
                column: "IdCupon");

            migrationBuilder.CreateIndex(
                name: "IX_VENTA_IdDireccionEnvio",
                table: "VENTA",
                column: "IdDireccionEnvio");

            migrationBuilder.CreateIndex(
                name: "IX_VENTA_IdEstadoPedido",
                table: "VENTA",
                column: "IdEstadoPedido");

            migrationBuilder.CreateIndex(
                name: "IX_VENTA_IdMetodoPago",
                table: "VENTA",
                column: "IdMetodoPago");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CARRITO");

            migrationBuilder.DropTable(
                name: "DETALLE_VENTA");

            migrationBuilder.DropTable(
                name: "HISTORIAL_PEDIDO");

            migrationBuilder.DropTable(
                name: "PRODUCTO");

            migrationBuilder.DropTable(
                name: "USUARIO");

            migrationBuilder.DropTable(
                name: "VENTA");

            migrationBuilder.DropTable(
                name: "CATEGORIA");

            migrationBuilder.DropTable(
                name: "MARCA");

            migrationBuilder.DropTable(
                name: "CUPON");

            migrationBuilder.DropTable(
                name: "DIRECCION_ENVIO");

            migrationBuilder.DropTable(
                name: "ESTADO_PEDIDO");

            migrationBuilder.DropTable(
                name: "METODO_PAGO");

            migrationBuilder.DropTable(
                name: "CLIENTE");
        }
    }
}
