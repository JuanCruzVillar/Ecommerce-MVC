using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceMVC.Migrations
{
    /// <inheritdoc />
    public partial class AgregarJerarquiaCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaPadreIdCategoria",
                table: "CATEGORIA",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdCategoriaPadre",
                table: "CATEGORIA",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORIA_CategoriaPadreIdCategoria",
                table: "CATEGORIA",
                column: "CategoriaPadreIdCategoria");

            migrationBuilder.AddForeignKey(
                name: "FK_CATEGORIA_CATEGORIA_CategoriaPadreIdCategoria",
                table: "CATEGORIA",
                column: "CategoriaPadreIdCategoria",
                principalTable: "CATEGORIA",
                principalColumn: "IdCategoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CATEGORIA_CATEGORIA_CategoriaPadreIdCategoria",
                table: "CATEGORIA");

            migrationBuilder.DropIndex(
                name: "IX_CATEGORIA_CategoriaPadreIdCategoria",
                table: "CATEGORIA");

            migrationBuilder.DropColumn(
                name: "CategoriaPadreIdCategoria",
                table: "CATEGORIA");

            migrationBuilder.DropColumn(
                name: "IdCategoriaPadre",
                table: "CATEGORIA");
        }
    }
}
