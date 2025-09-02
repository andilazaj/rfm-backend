using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rfm.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTourOperatorSeasonRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TourOperatorSeasons",
                columns: table => new
                {
                    TourOperatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    SeasonId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourOperatorSeasons", x => new { x.TourOperatorId, x.SeasonId });
                    table.ForeignKey(
                        name: "FK_TourOperatorSeasons_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourOperatorSeasons_TourOperators_TourOperatorId",
                        column: x => x.TourOperatorId,
                        principalTable: "TourOperators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourOperatorSeasons_SeasonId",
                table: "TourOperatorSeasons",
                column: "SeasonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourOperatorSeasons");
        }
    }
}
