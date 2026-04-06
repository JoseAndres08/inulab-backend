using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendLimpio.Migrations
{
    /// <inheritdoc />
    public partial class AddMotoLocationToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Usuarios_UserId",
                table: "Orders");

            migrationBuilder.AddColumn<double>(
                name: "MotoLat",
                table: "Orders",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MotoLng",
                table: "Orders",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Usuarios_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Usuarios_UserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MotoLat",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MotoLng",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Usuarios_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
