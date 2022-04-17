using Microsoft.EntityFrameworkCore.Migrations;

namespace chii.Migrations
{
    public partial class Updatetagtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NormUserCount",
                table: "Tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormUserCount",
                table: "Tags");
        }
    }
}
