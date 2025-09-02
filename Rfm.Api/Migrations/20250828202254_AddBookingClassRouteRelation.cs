using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rfm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingClassRouteRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingClassFlightRoute",
                columns: table => new
                {
                    BookingClassesId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoutesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingClassFlightRoute", x => new { x.BookingClassesId, x.RoutesId });
                    table.ForeignKey(
                        name: "FK_BookingClassFlightRoute_BookingClasses_BookingClassesId",
                        column: x => x.BookingClassesId,
                        principalTable: "BookingClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingClassFlightRoute_Routes_RoutesId",
                        column: x => x.RoutesId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingClassFlightRoute_RoutesId",
                table: "BookingClassFlightRoute",
                column: "RoutesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingClassFlightRoute");
        }
    }
}
