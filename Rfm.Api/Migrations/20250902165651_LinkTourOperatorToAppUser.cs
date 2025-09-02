using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rfm.Api.Migrations
{
    /// <inheritdoc />
    public partial class LinkTourOperatorToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourOperators_AspNetUsers_UserId",
                table: "TourOperators");

            migrationBuilder.DropIndex(
                name: "IX_TourOperators_UserId",
                table: "TourOperators");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TourOperators");

            migrationBuilder.CreateIndex(
                name: "IX_TourOperators_IdentityUserId",
                table: "TourOperators",
                column: "IdentityUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TourOperators_AspNetUsers_IdentityUserId",
                table: "TourOperators",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourOperators_AspNetUsers_IdentityUserId",
                table: "TourOperators");

            migrationBuilder.DropIndex(
                name: "IX_TourOperators_IdentityUserId",
                table: "TourOperators");

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
        }
    }
}
