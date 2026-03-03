using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hrms.Migrations
{
    /// <inheritdoc />
    public partial class systemsettingsfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "system_settings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "system_settings",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_system_settings_UpdatedById",
                table: "system_settings",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "fk_system_settings_updated_by_id",
                table: "system_settings",
                column: "UpdatedById",
                principalTable: "users",
                principalColumn: "pk_user_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_system_settings_updated_by_id",
                table: "system_settings");

            migrationBuilder.DropIndex(
                name: "IX_system_settings_UpdatedById",
                table: "system_settings");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "system_settings");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "system_settings");
        }
    }
}
