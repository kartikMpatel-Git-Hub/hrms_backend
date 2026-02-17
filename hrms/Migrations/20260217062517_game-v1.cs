using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hrms.Migrations
{
    /// <inheritdoc />
    public partial class gamev1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    pk_game_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    game_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    max_player_per_game = table.Column<int>(type: "int", nullable: false),
                    min_player_per_game = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.pk_game_id);
                });

            migrationBuilder.CreateTable(
                name: "user_game_interest",
                columns: table => new
                {
                    pk_user_game_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_game_interest", x => x.pk_user_game_id);
                    table.ForeignKey(
                        name: "fk_user_game_game_id",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "pk_game_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_game_user_id",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_game_slot",
                columns: table => new
                {
                    pk_game_slot_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_game_slot", x => x.pk_game_slot_id);
                    table.ForeignKey(
                        name: "fk_user_slot_game_id",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "pk_game_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_game_state",
                columns: table => new
                {
                    pk_user_game_state_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    game_played = table.Column<int>(type: "int", nullable: false),
                    last_played_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_game_state", x => x.pk_user_game_state_id);
                    table.ForeignKey(
                        name: "fk_game_state_user_id",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_state_game_id",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "pk_game_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_game_interest_GameId",
                table: "user_game_interest",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_user_game_interest_UserId",
                table: "user_game_interest",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_game_slot_GameId",
                table: "user_game_slot",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_user_game_state_GameId",
                table: "user_game_state",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_user_game_state_UserId",
                table: "user_game_state",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_game_interest");

            migrationBuilder.DropTable(
                name: "user_game_slot");

            migrationBuilder.DropTable(
                name: "user_game_state");

            migrationBuilder.DropTable(
                name: "games");
        }
    }
}
