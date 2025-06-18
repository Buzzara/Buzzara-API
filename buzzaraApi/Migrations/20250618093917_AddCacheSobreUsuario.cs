using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCacheSobreUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServicoEspecial",
                table: "Servicos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServicoPrestado",
                table: "Servicos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ServicoCache",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicoId = table.Column<int>(type: "int", nullable: false),
                    FormaPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescricaoCache = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorCache = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicoCache", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicoCache_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SobreUsuario",
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
                    table.PrimaryKey("PK_SobreUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SobreUsuario_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicoCache_ServicoId",
                table: "ServicoCache",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_SobreUsuario_ServicoId",
                table: "SobreUsuario",
                column: "ServicoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServicoCache");

            migrationBuilder.DropTable(
                name: "SobreUsuario");

            migrationBuilder.DropColumn(
                name: "ServicoEspecial",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "ServicoPrestado",
                table: "Servicos");
        }
    }
}
