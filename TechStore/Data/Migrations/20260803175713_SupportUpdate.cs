using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class SupportUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportMessages_SupportTickets_SupportedTicketId",
                table: "SupportMessages");

            migrationBuilder.RenameColumn(
                name: "SupportedTicketId",
                table: "SupportMessages",
                newName: "SupportTicketId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportMessages_SupportedTicketId",
                table: "SupportMessages",
                newName: "IX_SupportMessages_SupportTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportMessages_SupportTickets_SupportTicketId",
                table: "SupportMessages",
                column: "SupportTicketId",
                principalTable: "SupportTickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportMessages_SupportTickets_SupportTicketId",
                table: "SupportMessages");

            migrationBuilder.RenameColumn(
                name: "SupportTicketId",
                table: "SupportMessages",
                newName: "SupportedTicketId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportMessages_SupportTicketId",
                table: "SupportMessages",
                newName: "IX_SupportMessages_SupportedTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportMessages_SupportTickets_SupportedTicketId",
                table: "SupportMessages",
                column: "SupportedTicketId",
                principalTable: "SupportTickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
