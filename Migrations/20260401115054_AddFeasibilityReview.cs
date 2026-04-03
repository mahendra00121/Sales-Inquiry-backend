using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFeasibilityReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeasibilityReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InquiryId = table.Column<int>(type: "int", nullable: false),
                    IsFeasible = table.Column<bool>(type: "bit", nullable: false),
                    TechnicalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedProcessDays = table.Column<int>(type: "int", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeasibilityReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeasibilityReviews_SalesInquiries_InquiryId",
                        column: x => x.InquiryId,
                        principalTable: "SalesInquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeasibilityReviews_InquiryId",
                table: "FeasibilityReviews",
                column: "InquiryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeasibilityReviews");
        }
    }
}
