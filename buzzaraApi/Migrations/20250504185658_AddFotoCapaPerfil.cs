using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoCapaPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoCapaUrl",
                table: "Usuarios",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoPerfilUrl",
                table: "Usuarios",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoCapaUrl",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FotoPerfilUrl",
                table: "Usuarios");
        }
    }
}
