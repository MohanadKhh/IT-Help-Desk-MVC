using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITHelpDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BreachEmailSent",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WarningEmailSent",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreachEmailSent",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "WarningEmailSent",
                table: "Tickets");
        }
    }
}
