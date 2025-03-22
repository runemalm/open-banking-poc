using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sessions.Infrastructure.Persistence.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class AfterOpenDddUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Processed",
                table: "OutboxEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "OutboxEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
