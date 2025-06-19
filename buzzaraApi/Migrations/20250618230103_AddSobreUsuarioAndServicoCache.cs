using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSobreUsuarioAndServicoCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicoCache_Servicos_ServicoId",
                table: "ServicoCache");

            migrationBuilder.DropForeignKey(
                name: "FK_SobreUsuario_Servicos_ServicoId",
                table: "SobreUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SobreUsuario",
                table: "SobreUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServicoCache",
                table: "ServicoCache");

            migrationBuilder.RenameTable(
                name: "SobreUsuario",
                newName: "SobreUsuarios");

            migrationBuilder.RenameTable(
                name: "ServicoCache",
                newName: "ServicosCaches");

            migrationBuilder.RenameIndex(
                name: "IX_SobreUsuario_ServicoId",
                table: "SobreUsuarios",
                newName: "IX_SobreUsuarios_ServicoId");

            migrationBuilder.RenameIndex(
                name: "IX_ServicoCache_ServicoId",
                table: "ServicosCaches",
                newName: "IX_ServicosCaches_ServicoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SobreUsuarios",
                table: "SobreUsuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServicosCaches",
                table: "ServicosCaches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicosCaches_Servicos_ServicoId",
                table: "ServicosCaches",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "ServicoID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SobreUsuarios_Servicos_ServicoId",
                table: "SobreUsuarios",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "ServicoID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicosCaches_Servicos_ServicoId",
                table: "ServicosCaches");

            migrationBuilder.DropForeignKey(
                name: "FK_SobreUsuarios_Servicos_ServicoId",
                table: "SobreUsuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SobreUsuarios",
                table: "SobreUsuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServicosCaches",
                table: "ServicosCaches");

            migrationBuilder.RenameTable(
                name: "SobreUsuarios",
                newName: "SobreUsuario");

            migrationBuilder.RenameTable(
                name: "ServicosCaches",
                newName: "ServicoCache");

            migrationBuilder.RenameIndex(
                name: "IX_SobreUsuarios_ServicoId",
                table: "SobreUsuario",
                newName: "IX_SobreUsuario_ServicoId");

            migrationBuilder.RenameIndex(
                name: "IX_ServicosCaches_ServicoId",
                table: "ServicoCache",
                newName: "IX_ServicoCache_ServicoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SobreUsuario",
                table: "SobreUsuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServicoCache",
                table: "ServicoCache",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicoCache_Servicos_ServicoId",
                table: "ServicoCache",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "ServicoID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SobreUsuario_Servicos_ServicoId",
                table: "SobreUsuario",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "ServicoID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
