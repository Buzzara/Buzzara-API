using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTabelaPagamentosAnuncios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosAnuncios_ServicoID",
                table: "PagamentosAnuncios",
                column: "ServicoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagamentosAnuncios");
        }
    }
}
