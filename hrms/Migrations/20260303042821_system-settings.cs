using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hrms.Migrations
{
    /// <inheritdoc />
    public partial class systemsettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "system_settings",
                columns: table => new
                {
                    pk_system_settings_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    birthday_image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    anniversary_image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    default_profile_image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DefaultHrId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_settings", x => x.pk_system_settings_id);
                    table.ForeignKey(
                        name: "fk_system_settings_default_hr_id",
                        column: x => x.DefaultHrId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_system_settings_DefaultHrId",
                table: "system_settings",
                column: "DefaultHrId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "system_settings");
        }
    }
}
