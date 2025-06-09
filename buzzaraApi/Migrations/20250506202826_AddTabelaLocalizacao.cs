using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTabelaLocalizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Localizacoes_ServicoID",
                table: "Localizacoes",
                column: "ServicoID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Localizacoes");
        }
    }
}
