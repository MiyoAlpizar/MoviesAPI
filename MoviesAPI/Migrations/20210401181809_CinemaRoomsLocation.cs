using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Migrations
{
    public partial class CinemaRoomsLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "CinemaRooms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "CinemaRooms");
        }
    }
}
