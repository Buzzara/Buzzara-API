using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsValidToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    ServicoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.ServicoID);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioID);
                });

            migrationBuilder.CreateTable(
                name: "PerfisAcompanhantes",
                columns: table => new
                {
                    PerfilAcompanhanteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Localizacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tarifa = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisAcompanhantes", x => x.PerfilAcompanhanteID);
                    table.ForeignKey(
                        name: "FK_PerfisAcompanhantes_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID");
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    AgendamentoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteID = table.Column<int>(type: "int", nullable: false),
                    PerfilAcompanhanteID = table.Column<int>(type: "int", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.AgendamentoID);
                    table.ForeignKey(
                        name: "FK_Agendamentos_PerfisAcompanhantes_PerfilAcompanhanteID",
                        column: x => x.PerfilAcompanhanteID,
                        principalTable: "PerfisAcompanhantes",
                        principalColumn: "PerfilAcompanhanteID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Usuarios_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID");
                });

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
                name: "IX_Agendamentos_ClienteID",
                table: "Agendamentos",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_PerfilAcompanhanteID",
                table: "Agendamentos",
                column: "PerfilAcompanhanteID");

            migrationBuilder.CreateIndex(
                name: "IX_FotosAcompanhantes_PerfilAcompanhanteID",
                table: "FotosAcompanhantes",
                column: "PerfilAcompanhanteID");

            migrationBuilder.CreateIndex(
                name: "IX_PerfisAcompanhantes_UsuarioID",
                table: "PerfisAcompanhantes",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_VideosAcompanhantes_PerfilAcompanhanteID",
                table: "VideosAcompanhantes",
                column: "PerfilAcompanhanteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "FotosAcompanhantes");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropTable(
                name: "VideosAcompanhantes");

            migrationBuilder.DropTable(
                name: "PerfisAcompanhantes");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
