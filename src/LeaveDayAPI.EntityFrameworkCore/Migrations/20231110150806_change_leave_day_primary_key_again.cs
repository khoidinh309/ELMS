using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveDayAPI.Migrations
{
    /// <inheritdoc />
    public partial class changeleavedayprimarykeyagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveDays",
                table: "LeaveDays");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveDays",
                table: "LeaveDays",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveDays",
                table: "LeaveDays");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveDays",
                table: "LeaveDays",
                columns: new[] { "UserId", "RemainingDayNumber" });
        }
    }
}
