using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCards.Data.Migrations
{
    public partial class AddIsDeletedToCardSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CardSets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CardSets");
        }
    }
}
