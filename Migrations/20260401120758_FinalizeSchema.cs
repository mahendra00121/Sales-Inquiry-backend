using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class FinalizeSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "ShopFloorRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "SalesInquiries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerExpectedDate",
                table: "SalesInquiries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "ProductionPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNumber",
                table: "Procurements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "FinalQCReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarrierName",
                table: "DispatchRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingId",
                table: "DispatchRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPriceWithTax",
                table: "CostingQuotes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "CostingQuotes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "ShopFloorRecords");

            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "SalesInquiries");

            migrationBuilder.DropColumn(
                name: "CustomerExpectedDate",
                table: "SalesInquiries");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "ProductionPlans");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                table: "Procurements");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "FinalQCReports");

            migrationBuilder.DropColumn(
                name: "CarrierName",
                table: "DispatchRecords");

            migrationBuilder.DropColumn(
                name: "TrackingId",
                table: "DispatchRecords");

            migrationBuilder.DropColumn(
                name: "FinalPriceWithTax",
                table: "CostingQuotes");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "CostingQuotes");
        }
    }
}
