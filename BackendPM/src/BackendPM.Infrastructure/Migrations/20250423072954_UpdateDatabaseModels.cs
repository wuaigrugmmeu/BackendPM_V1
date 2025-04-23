using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendPM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HttpMethod",
                table: "Permissions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "Permissions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResourcePath",
                table: "Permissions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "Permissions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Permissions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HttpMethod",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ResourcePath",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Permissions");
        }
    }
}
