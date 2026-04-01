using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidationTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotoPerfilUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FotoCapaUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EstaOnline = table.Column<bool>(type: "bit", nullable: false),
                    UltimoAcesso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimoIP = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Tarifa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                name: "Servicos",
                columns: table => new
                {
                    ServicoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Saidas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LugarEncontro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicoPrestado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicoEspecial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disponibilidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Idade = table.Column<int>(type: "int", nullable: true),
                    Peso = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Altura = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    PerfilAcompanhanteID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.ServicoID);
                    table.ForeignKey(
                        name: "FK_Servicos_PerfisAcompanhantes_PerfilAcompanhanteID",
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
                name: "HorariosAtendimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicoId = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HorarioInicio = table.Column<TimeSpan>(type: "time", nullable: true),
                    HorarioFim = table.Column<TimeSpan>(type: "time", nullable: true),
                    Atende = table.Column<bool>(type: "bit", nullable: false),
                    VinteQuatroHoras = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosAtendimentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorariosAtendimentos_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Localizacoes",
                columns: table => new
                {
                    LocalizacaoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    ServicoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizacoes", x => x.LocalizacaoID);
                    table.ForeignKey(
                        name: "FK_Localizacoes_Servicos_ServicoID",
                        column: x => x.ServicoID,
                        principalTable: "Servicos",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagamentosAnuncios",
                columns: table => new
                {
                    PagamentoAnuncioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicoID = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Pago = table.Column<bool>(type: "bit", nullable: false),
                    QrCodeUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosAnuncios", x => x.PagamentoAnuncioID);
                    table.ForeignKey(
                        name: "FK_PagamentosAnuncios_Servicos_ServicoID",
                        column: x => x.ServicoID,
                        principalTable: "Servicos",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServicosCaches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicoId = table.Column<int>(type: "int", nullable: false),
                    FormaPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescricaoCache = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorCache = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicosCaches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicosCaches_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SobreUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Atendimento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Etnia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Relacionamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cabelo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estatura = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Corpo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pubis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServicoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SobreUsuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SobreUsuarios_Servicos_ServicoId",
                        column: x => x.ServicoId,
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
                name: "IX_FotosAnuncios_ServicoID",
                table: "FotosAnuncios",
                column: "ServicoID");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosAtendimentos_ServicoId",
                table: "HorariosAtendimentos",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Localizacoes_ServicoID",
                table: "Localizacoes",
                column: "ServicoID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosAnuncios_ServicoID",
                table: "PagamentosAnuncios",
                column: "ServicoID");

            migrationBuilder.CreateIndex(
                name: "IX_PerfisAcompanhantes_UsuarioID",
                table: "PerfisAcompanhantes",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_PerfilAcompanhanteID",
                table: "Servicos",
                column: "PerfilAcompanhanteID");

            migrationBuilder.CreateIndex(
                name: "IX_ServicosCaches_ServicoId",
                table: "ServicosCaches",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_SobreUsuarios_ServicoId",
                table: "SobreUsuarios",
                column: "ServicoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideosAcompanhantes_PerfilAcompanhanteID",
                table: "VideosAcompanhantes",
                column: "PerfilAcompanhanteID");

            migrationBuilder.CreateIndex(
                name: "IX_VideosAnuncios_ServicoID",
                table: "VideosAnuncios",
                column: "ServicoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "FotosAcompanhantes");

            migrationBuilder.DropTable(
                name: "FotosAnuncios");

            migrationBuilder.DropTable(
                name: "HorariosAtendimentos");

            migrationBuilder.DropTable(
                name: "Localizacoes");

            migrationBuilder.DropTable(
                name: "PagamentosAnuncios");

            migrationBuilder.DropTable(
                name: "ServicosCaches");

            migrationBuilder.DropTable(
                name: "SobreUsuarios");

            migrationBuilder.DropTable(
                name: "VideosAcompanhantes");

            migrationBuilder.DropTable(
                name: "VideosAnuncios");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropTable(
                name: "PerfisAcompanhantes");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
