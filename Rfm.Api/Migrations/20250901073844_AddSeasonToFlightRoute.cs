using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rfm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonToFlightRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeasonId",
                table: "Routes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_SeasonId",
                table: "Routes",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Seasons_SeasonId",
                table: "Routes",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Seasons_SeasonId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_SeasonId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "Routes");
        }
    }
}
