using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rfm.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePriceEntryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Seats",
                table: "PriceEntries",
                newName: "SeatCount");

            migrationBuilder.AlterColumn<string>(
                name: "TourOperatorId",
                table: "PriceEntries",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "DayOfWeek",
                table: "PriceEntries",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_PriceEntries_BookingClassId",
                table: "PriceEntries",
                column: "BookingClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceEntries_RouteId",
                table: "PriceEntries",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceEntries_SeasonId",
                table: "PriceEntries",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceEntries_TourOperatorId",
                table: "PriceEntries",
                column: "TourOperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceEntries_AspNetUsers_TourOperatorId",
                table: "PriceEntries",
                column: "TourOperatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceEntries_BookingClasses_BookingClassId",
                table: "PriceEntries",
                column: "BookingClassId",
                principalTable: "BookingClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceEntries_Routes_RouteId",
                table: "PriceEntries",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceEntries_Seasons_SeasonId",
                table: "PriceEntries",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceEntries_AspNetUsers_TourOperatorId",
                table: "PriceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceEntries_BookingClasses_BookingClassId",
                table: "PriceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceEntries_Routes_RouteId",
                table: "PriceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceEntries_Seasons_SeasonId",
                table: "PriceEntries");

            migrationBuilder.DropIndex(
                name: "IX_PriceEntries_BookingClassId",
                table: "PriceEntries");

            migrationBuilder.DropIndex(
                name: "IX_PriceEntries_RouteId",
                table: "PriceEntries");

            migrationBuilder.DropIndex(
                name: "IX_PriceEntries_SeasonId",
                table: "PriceEntries");

            migrationBuilder.DropIndex(
                name: "IX_PriceEntries_TourOperatorId",
                table: "PriceEntries");

            migrationBuilder.RenameColumn(
                name: "SeatCount",
                table: "PriceEntries",
                newName: "Seats");

            migrationBuilder.AlterColumn<int>(
                name: "TourOperatorId",
                table: "PriceEntries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "DayOfWeek",
                table: "PriceEntries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
