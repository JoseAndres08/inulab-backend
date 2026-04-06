using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendLimpio.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "OrderItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_AddressId",
                table: "OrderItems",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Addresses_AddressId",
                table: "OrderItems",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Addresses_AddressId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_AddressId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "OrderItems");
        }
    }
}
