using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendLimpio.Migrations
{
    /// <inheritdoc />
    public partial class AddResultPdfUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResultPdfUrl",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultPdfUrl",
                table: "Orders");
        }
    }
}
