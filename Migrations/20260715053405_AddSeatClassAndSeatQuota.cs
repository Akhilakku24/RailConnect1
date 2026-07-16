using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayReservation.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatClassAndSeatQuota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessCoachCount",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "QuotaPercentage",
                table: "Trains");

            migrationBuilder.AddColumn<string>(
                name: "SeatClass",
                table: "Passengers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeatQuota",
                table: "Passengers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatClass",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "SeatQuota",
                table: "Passengers");

            migrationBuilder.AddColumn<int>(
                name: "BusinessCoachCount",
                table: "Trains",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "QuotaPercentage",
                table: "Trains",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
