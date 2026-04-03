using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalQC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinalQCReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionRecordId = table.Column<int>(type: "int", nullable: false),
                    TestedQuantity = table.Column<int>(type: "int", nullable: false),
                    PassedQuantity = table.Column<int>(type: "int", nullable: false),
                    RejectedQuantity = table.Column<int>(type: "int", nullable: false),
                    QCIncharge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QCNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalQCReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinalQCReports_ShopFloorRecords_ProductionRecordId",
                        column: x => x.ProductionRecordId,
                        principalTable: "ShopFloorRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinalQCReports_ProductionRecordId",
                table: "FinalQCReports",
                column: "ProductionRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinalQCReports");
        }
    }
}
