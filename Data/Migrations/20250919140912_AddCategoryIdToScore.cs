using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordGuessGame.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIdToScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Scores",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Scores_CategoryId",
                table: "Scores",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Categories_CategoryId",
                table: "Scores",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Categories_CategoryId",
                table: "Scores");

            migrationBuilder.DropIndex(
                name: "IX_Scores_CategoryId",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Scores");
        }
    }
}
