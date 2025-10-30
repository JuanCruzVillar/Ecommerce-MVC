using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceMVC.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSistemaOfertas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OFERTA",
                columns: table => new
                {
                    IdOferta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    PrecioOferta = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PorcentajeDescuento = table.Column<int>(type: "int", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ProductoIdProducto = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OFERTA", x => x.IdOferta);
                    table.ForeignKey(
                        name: "FK_OFERTA_PRODUCTO",
                        column: x => x.IdProducto,
                        principalTable: "PRODUCTO",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OFERTA_PRODUCTO_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "PRODUCTO",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OFERTA_IdProducto",
                table: "OFERTA",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_OFERTA_ProductoIdProducto",
                table: "OFERTA",
                column: "ProductoIdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OFERTA");
        }
    }
}
