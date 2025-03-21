using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendMusic.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEntitiesOrChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedMusic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharedMusic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendshipId = table.Column<int>(type: "int", nullable: false),
                    MusicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedMusic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedMusic_Friendship_FriendshipId",
                        column: x => x.FriendshipId,
                        principalTable: "Friendship",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedMusic_Music_MusicId",
                        column: x => x.MusicId,
                        principalTable: "Music",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedMusic_FriendshipId",
                table: "SharedMusic",
                column: "FriendshipId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedMusic_MusicId",
                table: "SharedMusic",
                column: "MusicId");
        }
    }
}
