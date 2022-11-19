using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCards.Data.Migrations
{
    public partial class AddFavoritesReportsNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardSetFavorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardSetId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardSetFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardSetFavorites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardSetFavorites_CardSets_CardSetId",
                        column: x => x.CardSetId,
                        principalTable: "CardSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardSetReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportCause = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CardSetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardSetReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardSetReports_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardSetReports_CardSets_CardSetId",
                        column: x => x.CardSetId,
                        principalTable: "CardSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    RecipientId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardSetFavorites_CardSetId",
                table: "CardSetFavorites",
                column: "CardSetId");

            migrationBuilder.CreateIndex(
                name: "IX_CardSetFavorites_UserId",
                table: "CardSetFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CardSetReports_CardSetId",
                table: "CardSetReports",
                column: "CardSetId");

            migrationBuilder.CreateIndex(
                name: "IX_CardSetReports_UserId",
                table: "CardSetReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_RecipientId",
                table: "UserNotifications",
                column: "RecipientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardSetFavorites");

            migrationBuilder.DropTable(
                name: "CardSetReports");

            migrationBuilder.DropTable(
                name: "UserNotifications");
        }
    }
}
