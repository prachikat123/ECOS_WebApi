using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECOS_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDefaultColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "MetaConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "MetaConfigs");
        }
    }
}
