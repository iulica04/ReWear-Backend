using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OutfitClothingItemsManyToManyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClothingItems_Outfits_OutfitId",
                table: "ClothingItems");

            migrationBuilder.DropIndex(
                name: "IX_ClothingItems_OutfitId",
                table: "ClothingItems");

            migrationBuilder.DropColumn(
                name: "OutfitId",
                table: "ClothingItems");

            migrationBuilder.CreateTable(
                name: "OutfitClothingItems",
                columns: table => new
                {
                    OutfitId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClothingItemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutfitClothingItems", x => new { x.OutfitId, x.ClothingItemId });
                    table.ForeignKey(
                        name: "FK_OutfitClothingItems_ClothingItems_ClothingItemId",
                        column: x => x.ClothingItemId,
                        principalTable: "ClothingItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutfitClothingItems_Outfits_OutfitId",
                        column: x => x.OutfitId,
                        principalTable: "Outfits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutfitClothingItems_ClothingItemId",
                table: "OutfitClothingItems",
                column: "ClothingItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutfitClothingItems");

            migrationBuilder.AddColumn<Guid>(
                name: "OutfitId",
                table: "ClothingItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClothingItems_OutfitId",
                table: "ClothingItems",
                column: "OutfitId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClothingItems_Outfits_OutfitId",
                table: "ClothingItems",
                column: "OutfitId",
                principalTable: "Outfits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
