using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayReservation.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatAllocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessCoachCount",
                table: "Trains",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BusinessPercentage",
                table: "Trains",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumCoaches",
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

            migrationBuilder.AddColumn<string>(
                name: "CoachNo",
                table: "Passengers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeatNo",
                table: "Passengers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassType",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "JourneyDate",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Quota",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessCoachCount",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "BusinessPercentage",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "NumCoaches",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "QuotaPercentage",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "CoachNo",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "SeatNo",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "ClassType",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "JourneyDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Quota",
                table: "Bookings");
        }
    }
}
