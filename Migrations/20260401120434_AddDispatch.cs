using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDispatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DispatchRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackingId = table.Column<int>(type: "int", nullable: false),
                    DispatchDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DestinationAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DispatchStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalDispatchWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatchRecords_PackingRecords_PackingId",
                        column: x => x.PackingId,
                        principalTable: "PackingRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DispatchRecords_PackingId",
                table: "DispatchRecords",
                column: "PackingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DispatchRecords");
        }
    }
}
