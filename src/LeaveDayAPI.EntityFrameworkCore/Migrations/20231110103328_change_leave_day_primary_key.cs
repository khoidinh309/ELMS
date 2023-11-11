using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveDayAPI.Migrations
{
    /// <inheritdoc />
    public partial class changeleavedayprimarykey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveDays",
                table: "LeaveDays");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LeaveDays");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveDays",
                table: "LeaveDays",
                columns: new[] { "UserId", "RemainingDayNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaveDays",
                table: "LeaveDays");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "LeaveDays",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaveDays",
                table: "LeaveDays",
                column: "Id");
        }
    }
}
