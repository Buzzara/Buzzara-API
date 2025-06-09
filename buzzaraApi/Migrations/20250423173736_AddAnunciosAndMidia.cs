using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAnunciosAndMidia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_Servicos_UsuarioID",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "UsuarioID",
                table: "VideosAcompanhantes");

            migrationBuilder.DropColumn(
                name: "UsuarioID",
                table: "Servicos");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Servicos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacao",
                table: "Servicos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Servicos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "FotosAnuncios",
                columns: table => new
                {
                    FotoAnuncioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServicoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotosAnuncios", x => x.FotoAnuncioID);
                    table.ForeignKey(
                        name: "FK_FotosAnuncios_Servicos_ServicoID",
                        column: x => x.ServicoID,
                        principalTable: "Servicos",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideosAnuncios",
                columns: table => new
                {
                    VideoAnuncioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServicoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideosAnuncios", x => x.VideoAnuncioID);
                    table.ForeignKey(
                        name: "FK_VideosAnuncios_Servicos_ServicoID",
                        column: x => x.ServicoID,
                        principalTable: "Servicos",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FotosAnuncios_ServicoID",
                table: "FotosAnuncios",
                column: "ServicoID");

            migrationBuilder.CreateIndex(
                name: "IX_VideosAnuncios_ServicoID",
                table: "VideosAnuncios",
                column: "ServicoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FotosAnuncios");

            migrationBuilder.DropTable(
                name: "VideosAnuncios");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "DataAtualizacao",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Servicos");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioID",
                table: "VideosAcompanhantes",
                type: "int",
                nullable: true);

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
                name: "IX_Servicos_UsuarioID",
                table: "Servicos",
                column: "UsuarioID");

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
    }
}
