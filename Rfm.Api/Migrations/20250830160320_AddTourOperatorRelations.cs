using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rfm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTourOperatorRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TourOperators",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TourOperatorId",
                table: "BookingClasses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingClasses_TourOperatorId",
                table: "BookingClasses",
                column: "TourOperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingClasses_TourOperators_TourOperatorId",
                table: "BookingClasses",
                column: "TourOperatorId",
                principalTable: "TourOperators",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingClasses_TourOperators_TourOperatorId",
                table: "BookingClasses");

            migrationBuilder.DropIndex(
                name: "IX_BookingClasses_TourOperatorId",
                table: "BookingClasses");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "TourOperators");

            migrationBuilder.DropColumn(
                name: "TourOperatorId",
                table: "BookingClasses");
        }
    }
}
