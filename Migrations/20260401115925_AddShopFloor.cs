using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddShopFloor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopFloorRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionPlanId = table.Column<int>(type: "int", nullable: false),
                    ActualQuantityProduced = table.Column<int>(type: "int", nullable: false),
                    WasteQuantity = table.Column<int>(type: "int", nullable: false),
                    OperatorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopFloorRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopFloorRecords_ProductionPlans_ProductionPlanId",
                        column: x => x.ProductionPlanId,
                        principalTable: "ProductionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopFloorRecords_ProductionPlanId",
                table: "ShopFloorRecords",
                column: "ProductionPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopFloorRecords");
        }
    }
}
