using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendLimpio.Migrations
{
    /// <inheritdoc />
    public partial class AddPetIdToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PetId",
                table: "OrderItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_PetId",
                table: "OrderItems",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Pets_PetId",
                table: "OrderItems",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Pets_PetId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_PetId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "OrderItems");
        }
    }
}
