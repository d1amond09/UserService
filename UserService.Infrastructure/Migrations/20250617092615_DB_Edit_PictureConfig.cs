using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DB_Edit_PictureConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pictures_Id",
                table: "Pictures",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_PublicId",
                table: "Pictures",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pictures_Id",
                table: "Pictures");

            migrationBuilder.DropIndex(
                name: "IX_Pictures_PublicId",
                table: "Pictures");
        }
    }
}
