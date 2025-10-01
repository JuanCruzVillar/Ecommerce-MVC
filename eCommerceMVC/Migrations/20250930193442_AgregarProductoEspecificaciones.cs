using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceMVC.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProductoEspecificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PRODUCTO_ESPECIFICACION",
                columns: table => new
                {
                    IdEspecificacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Clave = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTO_ESPECIFICACION", x => x.IdEspecificacion);
                    table.ForeignKey(
                        name: "FK_PRODUCTO_ESPECIFICACION_PRODUCTO",
                        column: x => x.IdProducto,
                        principalTable: "PRODUCTO",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTO_ESPECIFICACION_IdProducto",
                table: "PRODUCTO_ESPECIFICACION",
                column: "IdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRODUCTO_ESPECIFICACION");
        }
    }
}
