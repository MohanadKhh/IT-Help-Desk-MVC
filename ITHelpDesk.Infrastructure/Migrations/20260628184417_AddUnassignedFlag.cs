using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITHelpDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnassignedFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UnassignedReminderSent",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnassignedReminderSent",
                table: "Tickets");
        }
    }
}
