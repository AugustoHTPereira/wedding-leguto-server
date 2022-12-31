using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wedding.Server.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBucketInformationToGuestPictureMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BucketName",
                table: "GuestPictures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StorageObjectId",
                table: "GuestPictures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BucketName",
                table: "GuestPictures");

            migrationBuilder.DropColumn(
                name: "StorageObjectId",
                table: "GuestPictures");
        }
    }
}
