using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class CompleteERPSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaterialConsumedQuantity",
                table: "ShopFloorRecords",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerms",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCharges",
                table: "SalesOrders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "InquiryReference",
                table: "SalesInquiries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeadSource",
                table: "SalesInquiries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaterialConsumedQuantity",
                table: "ShopFloorRecords");

            migrationBuilder.DropColumn(
                name: "PaymentTerms",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ShippingCharges",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "InquiryReference",
                table: "SalesInquiries");

            migrationBuilder.DropColumn(
                name: "LeadSource",
                table: "SalesInquiries");
        }
    }
}
