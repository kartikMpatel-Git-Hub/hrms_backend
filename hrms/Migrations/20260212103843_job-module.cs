using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hrms.Migrations
{
    /// <inheritdoc />
    public partial class jobmodule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    pk_job_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    job_role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    place = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    requirements = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    jd_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ContactTo = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.pk_job_id);
                    table.ForeignKey(
                        name: "fk_job_contact_to_id",
                        column: x => x.ContactTo,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_job_creater_id",
                        column: x => x.CreatedBy,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "job_referrals",
                columns: table => new
                {
                    pk_job_referral_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    refered_person_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    refered_person_email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    cv_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReferedBy = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    refered_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_referrals", x => x.pk_job_referral_id);
                    table.ForeignKey(
                        name: "fk_job_referer_id",
                        column: x => x.ReferedBy,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_refere_job_id",
                        column: x => x.JobId,
                        principalTable: "jobs",
                        principalColumn: "pk_job_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "job_reviewers",
                columns: table => new
                {
                    pk_job_reviewer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_reviewers", x => x.pk_job_reviewer_id);
                    table.ForeignKey(
                        name: "fk_job_id",
                        column: x => x.JobId,
                        principalTable: "jobs",
                        principalColumn: "pk_job_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_job_reviewer_id",
                        column: x => x.ReviewerId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "job_shared",
                columns: table => new
                {
                    pk_job_shared_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shared_to = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    shared_at = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    SharedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_shared", x => x.pk_job_shared_id);
                    table.ForeignKey(
                        name: "fk_job_shared_id",
                        column: x => x.JobId,
                        principalTable: "jobs",
                        principalColumn: "pk_job_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_job_shared_user_id",
                        column: x => x.shared_at,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_job_referrals_JobId",
                table: "job_referrals",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_job_referrals_ReferedBy",
                table: "job_referrals",
                column: "ReferedBy");

            migrationBuilder.CreateIndex(
                name: "IX_job_reviewers_JobId",
                table: "job_reviewers",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_job_reviewers_ReviewerId",
                table: "job_reviewers",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_job_shared_JobId",
                table: "job_shared",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_job_shared_shared_at",
                table: "job_shared",
                column: "shared_at");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_ContactTo",
                table: "jobs",
                column: "ContactTo");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_CreatedBy",
                table: "jobs",
                column: "CreatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_referrals");

            migrationBuilder.DropTable(
                name: "job_reviewers");

            migrationBuilder.DropTable(
                name: "job_shared");

            migrationBuilder.DropTable(
                name: "jobs");
        }
    }
}
