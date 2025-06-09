using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class addIdadePesoAltura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Altura",
                table: "Servicos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Idade",
                table: "Servicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Peso",
                table: "Servicos",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Altura",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "Idade",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "Peso",
                table: "Servicos");
        }
    }
}
