using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCostingQuote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CostingQuotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InquiryId = table.Column<int>(type: "int", nullable: false),
                    RawMaterialCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProcessingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Overheads = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProfitMarginPercentage = table.Column<float>(type: "real", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValidityDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostingQuotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostingQuotes_SalesInquiries_InquiryId",
                        column: x => x.InquiryId,
                        principalTable: "SalesInquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostingQuotes_InquiryId",
                table: "CostingQuotes",
                column: "InquiryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostingQuotes");
        }
    }
}
