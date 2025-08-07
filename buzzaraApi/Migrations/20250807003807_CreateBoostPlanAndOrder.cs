using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buzzaraApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateBoostPlanAndOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoostPlans",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DaysDuration = table.Column<int>(type: "int", nullable: false),
                    UpsPerDay = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Recommended = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoostPlans", x => x.PlanId);
                });

            migrationBuilder.CreateTable(
                name: "BoostOrders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicoId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    LastTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PreferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InitPoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SandboxInitPoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Paid = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoostOrders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_BoostOrders_BoostPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "BoostPlans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoostOrders_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "ServicoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoostOrders_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoostOrders_PlanId",
                table: "BoostOrders",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_BoostOrders_ServicoId",
                table: "BoostOrders",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_BoostOrders_UserId",
                table: "BoostOrders",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoostOrders");

            migrationBuilder.DropTable(
                name: "BoostPlans");
        }
    }
}
