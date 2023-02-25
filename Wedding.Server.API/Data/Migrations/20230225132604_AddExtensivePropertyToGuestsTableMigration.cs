using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wedding.Server.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExtensivePropertyToGuestsTableMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Extensive",
                table: "Guests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extensive",
                table: "Guests");
        }
    }
}
