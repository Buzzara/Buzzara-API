using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPerfilAcompanhanteIdToServicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioID",
                table: "VideosAcompanhantes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerfilAcompanhanteID",
                table: "Servicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioID",
                table: "Servicos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideosAcompanhantes_UsuarioID",
                table: "VideosAcompanhantes",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_PerfilAcompanhanteID",
                table: "Servicos",
                column: "PerfilAcompanhanteID");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_UsuarioID",
                table: "Servicos",
                column: "UsuarioID");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_PerfisAcompanhantes_PerfilAcompanhanteID",
                table: "Servicos",
                column: "PerfilAcompanhanteID",
                principalTable: "PerfisAcompanhantes",
                principalColumn: "PerfilAcompanhanteID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Usuarios_UsuarioID",
                table: "Servicos",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioID");

            migrationBuilder.AddForeignKey(
                name: "FK_VideosAcompanhantes_Usuarios_UsuarioID",
                table: "VideosAcompanhantes",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_PerfisAcompanhantes_PerfilAcompanhanteID",
                table: "Servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Usuarios_UsuarioID",
                table: "Servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_VideosAcompanhantes_Usuarios_UsuarioID",
                table: "VideosAcompanhantes");

            migrationBuilder.DropIndex(
                name: "IX_VideosAcompanhantes_UsuarioID",
                table: "VideosAcompanhantes");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_PerfilAcompanhanteID",
                table: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_UsuarioID",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "UsuarioID",
                table: "VideosAcompanhantes");

            migrationBuilder.DropColumn(
                name: "PerfilAcompanhanteID",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "UsuarioID",
                table: "Servicos");
        }
    }
}
