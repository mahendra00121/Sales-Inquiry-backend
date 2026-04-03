using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProcurement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Procurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionPlanId = table.Column<int>(type: "int", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExpectedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QCStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QCNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Procurements_ProductionPlans_ProductionPlanId",
                        column: x => x.ProductionPlanId,
                        principalTable: "ProductionPlans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Procurements_ProductionPlanId",
                table: "Procurements",
                column: "ProductionPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Procurements");
        }
    }
}
