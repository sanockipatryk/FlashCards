using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCards.Data.Migrations
{
    public partial class AddEvaluatedAndNullablesToReportAndNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardSetReports_CardSets_CardSetId",
                table: "CardSetReports");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRead",
                table: "UserNotifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSent",
                table: "UserNotifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "CardSetId",
                table: "CardSetReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEvaluated",
                table: "CardSetReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateReported",
                table: "CardSetReports",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EvaluatingAdminId",
                table: "CardSetReports",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageToUser",
                table: "CardSetReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reasoning",
                table: "CardSetReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReportEvaluated",
                table: "CardSetReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReportResponse",
                table: "CardSetReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardSetReports_EvaluatingAdminId",
                table: "CardSetReports",
                column: "EvaluatingAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardSetReports_AspNetUsers_EvaluatingAdminId",
                table: "CardSetReports",
                column: "EvaluatingAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CardSetReports_CardSets_CardSetId",
                table: "CardSetReports",
                column: "CardSetId",
                principalTable: "CardSets",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardSetReports_AspNetUsers_EvaluatingAdminId",
                table: "CardSetReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CardSetReports_CardSets_CardSetId",
                table: "CardSetReports");

            migrationBuilder.DropIndex(
                name: "IX_CardSetReports_EvaluatingAdminId",
                table: "CardSetReports");

            migrationBuilder.DropColumn(
                name: "DateRead",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "DateSent",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "DateEvaluated",
                table: "CardSetReports");

            migrationBuilder.DropColumn(
                name: "DateReported",
                table: "CardSetReports");

            migrationBuilder.DropColumn(
                name: "EvaluatingAdminId",
                table: "CardSetReports");

            migrationBuilder.DropColumn(
                name: "MessageToUser",
                table: "CardSetReports");

            migrationBuilder.DropColumn(
                name: "Reasoning",
                table: "CardSetReports");

            migrationBuilder.DropColumn(
                name: "ReportEvaluated",
                table: "CardSetReports");

            migrationBuilder.DropColumn(
                name: "ReportResponse",
                table: "CardSetReports");

            migrationBuilder.AlterColumn<int>(
                name: "CardSetId",
                table: "CardSetReports",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CardSetReports_CardSets_CardSetId",
                table: "CardSetReports",
                column: "CardSetId",
                principalTable: "CardSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
