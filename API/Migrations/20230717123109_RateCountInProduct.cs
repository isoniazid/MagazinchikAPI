using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagazinchikAPI.Migrations
{
    /// <inheritdoc />
    public partial class RateCountInProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RateCount",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RateCount",
                table: "Products");
        }
    }
}
