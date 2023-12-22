using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VisitDate",
                table: "Visits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            
            migrationBuilder.Sql("update DentalCoreDb.dbo.Visits set [VisitDate] = CreatedOn where VisitDate = '0001-01-01 00:00:00.0000000';");
            migrationBuilder.Sql("update DentalCoreDb.dbo.Payments set [PaymentDate] = CreatedOn where PaymentDate = '0001-01-01 00:00:00.0000000';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitDate",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Payments");
        }
    }
}
