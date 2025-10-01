using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceMVC.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProductoImagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PRODUCTO_IMAGEN",
                columns: table => new
                {
                    IdImagen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    RutaImagen = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    NombreImagen = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    EsPrincipal = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTO_IMAGEN", x => x.IdImagen);
                    table.ForeignKey(
                        name: "FK_PRODUCTO_IMAGEN_PRODUCTO",
                        column: x => x.IdProducto,
                        principalTable: "PRODUCTO",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTO_IMAGEN_IdProducto",
                table: "PRODUCTO_IMAGEN",
                column: "IdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRODUCTO_IMAGEN");
        }
    }
}
