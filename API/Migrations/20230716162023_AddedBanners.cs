using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MagazinchikAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedBanners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BannerId",
                table: "Photos",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_BannerId",
                table: "Photos",
                column: "BannerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Banners_BannerId",
                table: "Photos",
                column: "BannerId",
                principalTable: "Banners",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Banners_BannerId",
                table: "Photos");

            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropIndex(
                name: "IX_Photos_BannerId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "Photos");
        }
    }
}
