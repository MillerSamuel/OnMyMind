using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnMyMind.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "ProfilePic",
                table: "Users",
                type: "tinyint unsigned",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProfilePic",
                table: "Users",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
