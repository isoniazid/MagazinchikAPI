using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagazinchikAPI.Migrations
{
    /// <inheritdoc />
    public partial class deletedDateTimeFromOrderProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OrderProducts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrderProducts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OrderProducts",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
