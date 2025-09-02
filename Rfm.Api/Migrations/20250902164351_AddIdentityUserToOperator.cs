using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rfm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityUserToOperator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourOperatorSeasons_Seasons_SeasonId",
                table: "TourOperatorSeasons");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "PriceEntries");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "TourOperators",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TourOperators",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourOperators_UserId",
                table: "TourOperators",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TourOperators_AspNetUsers_UserId",
                table: "TourOperators",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TourOperatorSeasons_Seasons_SeasonId",
                table: "TourOperatorSeasons",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourOperators_AspNetUsers_UserId",
                table: "TourOperators");

            migrationBuilder.DropForeignKey(
                name: "FK_TourOperatorSeasons_Seasons_SeasonId",
                table: "TourOperatorSeasons");

            migrationBuilder.DropIndex(
                name: "IX_TourOperators_UserId",
                table: "TourOperators");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "TourOperators");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TourOperators");

            migrationBuilder.AddColumn<string>(
                name: "DayOfWeek",
                table: "PriceEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_TourOperatorSeasons_Seasons_SeasonId",
                table: "TourOperatorSeasons",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
