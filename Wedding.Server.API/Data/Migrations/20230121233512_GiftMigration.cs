using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wedding.Server.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class GiftMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(255)", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Store = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiftGuest",
                columns: table => new
                {
                    GiftsId = table.Column<int>(type: "int", nullable: false),
                    GuestsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftGuest", x => new { x.GiftsId, x.GuestsId });
                    table.ForeignKey(
                        name: "FK_GiftGuest_Gifts_GiftsId",
                        column: x => x.GiftsId,
                        principalTable: "Gifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GiftGuest_Guests_GuestsId",
                        column: x => x.GuestsId,
                        principalTable: "Guests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GiftMetadata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GiftId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "varchar(100)", nullable: false),
                    Value = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftMetadata_Gifts_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Gifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GiftGuest_GuestsId",
                table: "GiftGuest",
                column: "GuestsId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftMetadata_GiftId",
                table: "GiftMetadata",
                column: "GiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiftGuest");

            migrationBuilder.DropTable(
                name: "GiftMetadata");

            migrationBuilder.DropTable(
                name: "Gifts");
        }
    }
}
