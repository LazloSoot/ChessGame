using Microsoft.EntityFrameworkCore.Migrations;

namespace Chess.DataAccess.Migrations
{
    public partial class PlayerExtention : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uid",
                table: "Players",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Uid",
                table: "Players");
        }
    }
}
