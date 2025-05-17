using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOutfitTableMigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClothingTag_ClothingItems_ClothingItemId",
                table: "ClothingTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClothingTag",
                table: "ClothingTag");

            migrationBuilder.RenameTable(
                name: "ClothingTag",
                newName: "ClothingTags");

            migrationBuilder.RenameIndex(
                name: "IX_ClothingTag_ClothingItemId",
                table: "ClothingTags",
                newName: "IX_ClothingTags_ClothingItemId");

            migrationBuilder.AddColumn<Guid>(
                name: "OutfitId",
                table: "ClothingItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClothingTags",
                table: "ClothingTags",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Outfits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Season = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outfits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Outfits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClothingItems_OutfitId",
                table: "ClothingItems",
                column: "OutfitId");

            migrationBuilder.CreateIndex(
                name: "IX_Outfits_UserId",
                table: "Outfits",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClothingItems_Outfits_OutfitId",
                table: "ClothingItems",
                column: "OutfitId",
                principalTable: "Outfits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClothingTags_ClothingItems_ClothingItemId",
                table: "ClothingTags",
                column: "ClothingItemId",
                principalTable: "ClothingItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClothingItems_Outfits_OutfitId",
                table: "ClothingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ClothingTags_ClothingItems_ClothingItemId",
                table: "ClothingTags");

            migrationBuilder.DropTable(
                name: "Outfits");

            migrationBuilder.DropIndex(
                name: "IX_ClothingItems_OutfitId",
                table: "ClothingItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClothingTags",
                table: "ClothingTags");

            migrationBuilder.DropColumn(
                name: "OutfitId",
                table: "ClothingItems");

            migrationBuilder.RenameTable(
                name: "ClothingTags",
                newName: "ClothingTag");

            migrationBuilder.RenameIndex(
                name: "IX_ClothingTags_ClothingItemId",
                table: "ClothingTag",
                newName: "IX_ClothingTag_ClothingItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClothingTag",
                table: "ClothingTag",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClothingTag_ClothingItems_ClothingItemId",
                table: "ClothingTag",
                column: "ClothingItemId",
                principalTable: "ClothingItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
