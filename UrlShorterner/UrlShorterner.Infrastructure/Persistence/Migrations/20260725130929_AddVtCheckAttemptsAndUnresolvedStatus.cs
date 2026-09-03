using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVtCheckAttemptsAndUnresolvedStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_links_vt_status",
                table: "links");

            migrationBuilder.CreateIndex(
                name: "ix_links_vt_status",
                table: "links",
                column: "vt_status",
                filter: "vt_status IN ('Pending', 'Malicious', 'Unresolved')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_links_vt_status",
                table: "links");

            migrationBuilder.CreateIndex(
                name: "ix_links_vt_status",
                table: "links",
                column: "vt_status",
                filter: "vt_status IN ('Pending', 'Malicious')");
        }
    }
}
