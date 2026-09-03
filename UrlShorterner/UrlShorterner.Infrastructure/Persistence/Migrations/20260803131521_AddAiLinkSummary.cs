using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAiLinkSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ai_estimated_reading_minutes",
                table: "links",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ai_key_topics",
                table: "links",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ai_keywords",
                table: "links",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ai_summary",
                table: "links",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ai_summary_attempted",
                table: "links",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ai_estimated_reading_minutes",
                table: "links");

            migrationBuilder.DropColumn(
                name: "ai_key_topics",
                table: "links");

            migrationBuilder.DropColumn(
                name: "ai_keywords",
                table: "links");

            migrationBuilder.DropColumn(
                name: "ai_summary",
                table: "links");

            migrationBuilder.DropColumn(
                name: "ai_summary_attempted",
                table: "links");
        }
    }
}
