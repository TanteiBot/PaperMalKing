using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaperMalKing.Database.Migrations
{
    /// <inheritdoc />
    public partial class FavouriteIdIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShikiFavourites_Id",
                table: "ShikiFavourites",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AniListFavourites_Id",
                table: "AniListFavourites",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShikiFavourites_Id",
                table: "ShikiFavourites");

            migrationBuilder.DropIndex(
                name: "IX_AniListFavourites_Id",
                table: "AniListFavourites");
        }
    }
}
