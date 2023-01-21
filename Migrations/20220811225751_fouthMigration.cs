using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnMyMind.Migrations
{
    public partial class fouthMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "ProfilePic",
                table: "Users",
                type: "longblob",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "ProfilePic",
                table: "Users",
                type: "tinyint unsigned",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "longblob",
                oldNullable: true);
        }
    }
}
