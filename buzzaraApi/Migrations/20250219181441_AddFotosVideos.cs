using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFotosVideos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerfisAcompanhantes_Usuarios_UsuarioID",
                table: "PerfisAcompanhantes");

            migrationBuilder.AlterColumn<string>(
                name: "Localizacao",
                table: "PerfisAcompanhantes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "PerfisAcompanhantes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "FotosAcompanhantes",
                columns: table => new
                {
                    FotoAcompanhanteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerfilAcompanhanteID = table.Column<int>(type: "int", nullable: false),
                    UrlFoto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotosAcompanhantes", x => x.FotoAcompanhanteID);
                    table.ForeignKey(
                        name: "FK_FotosAcompanhantes_PerfisAcompanhantes_PerfilAcompanhanteID",
                        column: x => x.PerfilAcompanhanteID,
                        principalTable: "PerfisAcompanhantes",
                        principalColumn: "PerfilAcompanhanteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideosAcompanhantes",
                columns: table => new
                {
                    VideoAcompanhanteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerfilAcompanhanteID = table.Column<int>(type: "int", nullable: false),
                    UrlVideo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideosAcompanhantes", x => x.VideoAcompanhanteID);
                    table.ForeignKey(
                        name: "FK_VideosAcompanhantes_PerfisAcompanhantes_PerfilAcompanhanteID",
                        column: x => x.PerfilAcompanhanteID,
                        principalTable: "PerfisAcompanhantes",
                        principalColumn: "PerfilAcompanhanteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FotosAcompanhantes_PerfilAcompanhanteID",
                table: "FotosAcompanhantes",
                column: "PerfilAcompanhanteID");

            migrationBuilder.CreateIndex(
                name: "IX_VideosAcompanhantes_PerfilAcompanhanteID",
                table: "VideosAcompanhantes",
                column: "PerfilAcompanhanteID");

            migrationBuilder.AddForeignKey(
                name: "FK_PerfisAcompanhantes_Usuarios_UsuarioID",
                table: "PerfisAcompanhantes",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerfisAcompanhantes_Usuarios_UsuarioID",
                table: "PerfisAcompanhantes");

            migrationBuilder.DropTable(
                name: "FotosAcompanhantes");

            migrationBuilder.DropTable(
                name: "VideosAcompanhantes");

            migrationBuilder.AlterColumn<string>(
                name: "Localizacao",
                table: "PerfisAcompanhantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "PerfisAcompanhantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfisAcompanhantes_Usuarios_UsuarioID",
                table: "PerfisAcompanhantes",
                column: "UsuarioID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
