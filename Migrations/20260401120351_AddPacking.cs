using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPacking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PackingRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinalQCId = table.Column<int>(type: "int", nullable: false),
                    NumberOfBoxes = table.Column<int>(type: "int", nullable: false),
                    PackingType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PackedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PackingNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackingRecords_FinalQCReports_FinalQCId",
                        column: x => x.FinalQCId,
                        principalTable: "FinalQCReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackingRecords_FinalQCId",
                table: "PackingRecords",
                column: "FinalQCId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackingRecords");
        }
    }
}
