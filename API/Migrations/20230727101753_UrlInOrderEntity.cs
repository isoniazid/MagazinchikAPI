using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagazinchikAPI.Migrations
{
    /// <inheritdoc />
    public partial class UrlInOrderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Orders");
        }
    }
}
