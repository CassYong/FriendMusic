using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendMusic.Migrations
{
    /// <inheritdoc />
    public partial class removeMusicSharesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicShares");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MusicShares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MusicId = table.Column<int>(type: "int", nullable: false),
                    SharedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SharedWithId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShareDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusicShares_AspNetUsers_SharedById",
                        column: x => x.SharedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MusicShares_AspNetUsers_SharedWithId",
                        column: x => x.SharedWithId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MusicShares_Music_MusicId",
                        column: x => x.MusicId,
                        principalTable: "Music",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MusicShares_MusicId",
                table: "MusicShares",
                column: "MusicId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicShares_SharedById",
                table: "MusicShares",
                column: "SharedById");

            migrationBuilder.CreateIndex(
                name: "IX_MusicShares_SharedWithId",
                table: "MusicShares",
                column: "SharedWithId");
        }
    }
}
