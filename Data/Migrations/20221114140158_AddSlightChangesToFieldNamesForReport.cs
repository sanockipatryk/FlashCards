using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCards.Data.Migrations
{
    public partial class AddSlightChangesToFieldNamesForReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReportEvaluated",
                table: "CardSetReports",
                newName: "IsEvaluated");

            migrationBuilder.RenameColumn(
                name: "Reasoning",
                table: "CardSetReports",
                newName: "MessageToReportedSetOwner");

            migrationBuilder.RenameColumn(
                name: "MessageToUser",
                table: "CardSetReports",
                newName: "EvaluationReasoning");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MessageToReportedSetOwner",
                table: "CardSetReports",
                newName: "Reasoning");

            migrationBuilder.RenameColumn(
                name: "IsEvaluated",
                table: "CardSetReports",
                newName: "ReportEvaluated");

            migrationBuilder.RenameColumn(
                name: "EvaluationReasoning",
                table: "CardSetReports",
                newName: "MessageToUser");
        }
    }
}
