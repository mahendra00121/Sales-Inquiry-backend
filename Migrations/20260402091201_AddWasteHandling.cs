using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWasteHandling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateTable(
                name: "WasteRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopFloorId = table.Column<int>(type: "int", nullable: false),
                    WasteType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionTaken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WasteRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WasteRecords_ShopFloorRecords_ShopFloorId",
                        column: x => x.ShopFloorId,
                        principalTable: "ShopFloorRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WasteRecords_ShopFloorId",
                table: "WasteRecords",
                column: "ShopFloorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WasteRecords");

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "SalesOrders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
