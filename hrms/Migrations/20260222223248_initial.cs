using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hrms.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    pk_department_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.pk_department_id);
                });

            migrationBuilder.CreateTable(
                name: "expense_category",
                columns: table => new
                {
                    pk_expense_category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_category", x => x.pk_expense_category_id);
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    pk_game_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    game_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    max_player_per_game = table.Column<int>(type: "int", nullable: false),
                    duration_in_minutes = table.Column<int>(type: "int", nullable: false),
                    min_player_per_game = table.Column<int>(type: "int", nullable: false),
                    slot_assigned_before_x_minutes = table.Column<int>(type: "int", nullable: false),
                    slot_create_for_next_x_days = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.pk_game_id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    pk_tag_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tag_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.pk_tag_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    pk_user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    user_role = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date_of_joining = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportTo = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    designation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.pk_user_id);
                    table.ForeignKey(
                        name: "fk_department_id",
                        column: x => x.DepartmentId,
                        principalTable: "departments",
                        principalColumn: "pk_department_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reported_user_id",
                        column: x => x.ReportTo,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "game_operation_window",
                columns: table => new
                {
                    pk_game_operation_window_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    operational_start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    operational_end_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_operation_window", x => x.pk_game_operation_window_id);
                    table.ForeignKey(
                        name: "fk_game_operation_window_game_id",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "pk_game_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "booking_slot",
                columns: table => new
                {
                    pk_booking_slot_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    BookedBy = table.Column<int>(type: "int", nullable: true),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequestBy = table.Column<int>(type: "int", nullable: true),
                    RequesterId = table.Column<int>(type: "int", nullable: true),
                    current_priority_order = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_slot", x => x.pk_booking_slot_id);
                    table.ForeignKey(
                        name: "FK_booking_slot_users_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "users",
                        principalColumn: "pk_user_id");
                    table.ForeignKey(
                        name: "fk_bookin_slot_game_id",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "pk_game_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_booking_slot_booked_by_id",
                        column: x => x.BookedBy,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "daily_celebrations",
                columns: table => new
                {
                    pk_daily_celebration_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    celebration_type = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    celebration_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_celebrations", x => x.pk_daily_celebration_id);
                    table.ForeignKey(
                        name: "fk_daily_celebration_user_id",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "game_slots",
                columns: table => new
                {
                    pk_game_slot_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookedById = table.Column<int>(type: "int", nullable: true),
                    booked_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_slots", x => x.pk_game_slot_id);
                    table.ForeignKey(
                        name: "FK_game_slots_users_BookedById",
                        column: x => x.BookedById,
                        principalTable: "users",
                        principalColumn: "pk_user_id");
                    table.ForeignKey(
                        name: "fk_game_slot_game_id",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "pk_game_id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
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
                name: "notifications",
                columns: table => new
                {
                    pk_notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotifiedTo = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    is_viewed = table.Column<bool>(type: "bit", nullable: false),
                    notification_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.pk_notification_id);
                    table.ForeignKey(
                        name: "fk_notified_user_id",
                        column: x => x.NotifiedTo,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    pk_post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PostById = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    is_public = table.Column<bool>(type: "bit", nullable: false),
                    is_inappropriate = table.Column<bool>(type: "bit", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.pk_post_id);
                    table.ForeignKey(
                        name: "fk_post_by_user_id",
                        column: x => x.PostById,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "travels",
                columns: table => new
                {
                    pk_travel_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    travel_start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    trael_end_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    expense_max_amount_limit = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_travels", x => x.pk_travel_id);
                    table.ForeignKey(
                        name: "fk_created_user_by",
                        column: x => x.CreatedBy,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_game_interest",
                columns: table => new
                {
                    pk_user_game_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "booking_players",
                columns: table => new
                {
                    pk_booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_players", x => x.pk_booking_id);
                    table.ForeignKey(
                        name: "fk_game_slot_player_id",
                        column: x => x.PlayerId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_slot_slot_id",
                        column: x => x.SlotId,
                        principalTable: "booking_slot",
                        principalColumn: "pk_booking_slot_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "requested_players",
                columns: table => new
                {
                    pk_requested_player_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requested_players", x => x.pk_requested_player_id);
                    table.ForeignKey(
                        name: "fk_requested_player_player_id",
                        column: x => x.PlayerId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_requested_player_slot_id",
                        column: x => x.SlotId,
                        principalTable: "booking_slot",
                        principalColumn: "pk_booking_slot_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "slot_offers",
                columns: table => new
                {
                    pk_slot_offer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingSlotId = table.Column<int>(type: "int", nullable: false),
                    OffereTo = table.Column<int>(type: "int", nullable: false),
                    priority_order = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expired_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slot_offers", x => x.pk_slot_offer_id);
                    table.ForeignKey(
                        name: "fk_slot_offer_booking_slot_id",
                        column: x => x.BookingSlotId,
                        principalTable: "booking_slot",
                        principalColumn: "pk_booking_slot_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_slot_offer_offered_to_id",
                        column: x => x.OffereTo,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "game_slot_players",
                columns: table => new
                {
                    pk_game_slot_player_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_slot_players", x => x.pk_game_slot_player_id);
                    table.ForeignKey(
                        name: "fk_game_slot_player_player_id",
                        column: x => x.PlayerId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_slot_player_slot_id",
                        column: x => x.SlotId,
                        principalTable: "game_slots",
                        principalColumn: "pk_game_slot_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "game_slot_waiting",
                columns: table => new
                {
                    pk_game_slot_waiting_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameSlotId = table.Column<int>(type: "int", nullable: false),
                    RequestedById = table.Column<int>(type: "int", nullable: false),
                    requested_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    is_cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_slot_waiting", x => x.pk_game_slot_waiting_id);
                    table.ForeignKey(
                        name: "fk_game_slot_waiting_requested_by_id",
                        column: x => x.RequestedById,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_slot_waiting_slot_id",
                        column: x => x.GameSlotId,
                        principalTable: "game_slots",
                        principalColumn: "pk_game_slot_id",
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
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "post_comments",
                columns: table => new
                {
                    pk_post_comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    CommentById = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_comments", x => x.pk_post_comment_id);
                    table.ForeignKey(
                        name: "fk_post_comment_by_user_id",
                        column: x => x.CommentById,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_post_comment_post_id",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "pk_post_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "post_likes",
                columns: table => new
                {
                    pk_post_like_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    LikedById = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_likes", x => x.pk_post_like_id);
                    table.ForeignKey(
                        name: "fk_post_like_by_user_id",
                        column: x => x.LikedById,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_post_like_post_id",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "pk_post_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "post_tags",
                columns: table => new
                {
                    pk_post_tag_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_tags", x => x.pk_post_tag_id);
                    table.ForeignKey(
                        name: "fk_post_tag_post_id",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "pk_post_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_post_tag_tag_id",
                        column: x => x.TagId,
                        principalTable: "tags",
                        principalColumn: "pk_tag_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    pk_expense_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelId = table.Column<int>(type: "int", nullable: false),
                    TravelerId = table.Column<int>(type: "int", nullable: false),
                    expense_amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    details = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    expense_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.pk_expense_id);
                    table.ForeignKey(
                        name: "fk_category_expense_id",
                        column: x => x.CategoryId,
                        principalTable: "expense_category",
                        principalColumn: "pk_expense_category_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_travel_expense_id",
                        column: x => x.TravelId,
                        principalTable: "travels",
                        principalColumn: "pk_travel_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_traveler_expense_id",
                        column: x => x.TravelerId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "travel_documents",
                columns: table => new
                {
                    pk_travel_document_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelId = table.Column<int>(type: "int", nullable: false),
                    TravelerId = table.Column<int>(type: "int", nullable: false),
                    document_url = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    document_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    document_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UploadedBy = table.Column<int>(type: "int", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_travel_documents", x => x.pk_travel_document_id);
                    table.ForeignKey(
                        name: "fk_travel_document_id",
                        column: x => x.TravelId,
                        principalTable: "travels",
                        principalColumn: "pk_travel_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_traveler_document_id",
                        column: x => x.TravelerId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_uploaded_document_by",
                        column: x => x.UploadedBy,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "travelers",
                columns: table => new
                {
                    pk_traveler_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelId = table.Column<int>(type: "int", nullable: false),
                    TravelerId = table.Column<int>(type: "int", nullable: false),
                    is_deletd = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_travelers", x => x.pk_traveler_id);
                    table.ForeignKey(
                        name: "fk_traveler_treavel_id",
                        column: x => x.TravelId,
                        principalTable: "travels",
                        principalColumn: "pk_travel_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_traveler_user_id",
                        column: x => x.TravelerId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "game_slot_waiting_players",
                columns: table => new
                {
                    pk_game_slot_waiting_player_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameSlotWaitingId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_slot_waiting_players", x => x.pk_game_slot_waiting_player_id);
                    table.ForeignKey(
                        name: "fk_game_slot_waiting_player_player_id",
                        column: x => x.PlayerId,
                        principalTable: "users",
                        principalColumn: "pk_user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_slot_waiting_player_waiting_id",
                        column: x => x.GameSlotWaitingId,
                        principalTable: "game_slot_waiting",
                        principalColumn: "pk_game_slot_waiting_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "expense_proof",
                columns: table => new
                {
                    pk_expense_proof_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseId = table.Column<int>(type: "int", nullable: false),
                    proof_document_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    expense_document_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_proof", x => x.pk_expense_proof_id);
                    table.ForeignKey(
                        name: "fk_expense_proof_id",
                        column: x => x.ExpenseId,
                        principalTable: "expenses",
                        principalColumn: "pk_expense_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_booking_players_PlayerId",
                table: "booking_players",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_players_SlotId",
                table: "booking_players",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_slot_BookedBy",
                table: "booking_slot",
                column: "BookedBy");

            migrationBuilder.CreateIndex(
                name: "IX_booking_slot_GameId",
                table: "booking_slot",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_slot_RequesterId",
                table: "booking_slot",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_daily_celebrations_UserId",
                table: "daily_celebrations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_expense_proof_ExpenseId",
                table: "expense_proof",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_CategoryId",
                table: "expenses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_TravelerId",
                table: "expenses",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_TravelId",
                table: "expenses",
                column: "TravelId");

            migrationBuilder.CreateIndex(
                name: "IX_game_operation_window_GameId",
                table: "game_operation_window",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_game_slot_players_PlayerId",
                table: "game_slot_players",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_game_slot_players_SlotId",
                table: "game_slot_players",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_game_slot_waiting_GameSlotId",
                table: "game_slot_waiting",
                column: "GameSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_game_slot_waiting_RequestedById",
                table: "game_slot_waiting",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_game_slot_waiting_players_GameSlotWaitingId",
                table: "game_slot_waiting_players",
                column: "GameSlotWaitingId");

            migrationBuilder.CreateIndex(
                name: "IX_game_slot_waiting_players_PlayerId",
                table: "game_slot_waiting_players",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_game_slots_BookedById",
                table: "game_slots",
                column: "BookedById");

            migrationBuilder.CreateIndex(
                name: "IX_game_slots_GameId",
                table: "game_slots",
                column: "GameId");

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

            migrationBuilder.CreateIndex(
                name: "IX_notifications_NotifiedTo",
                table: "notifications",
                column: "NotifiedTo");

            migrationBuilder.CreateIndex(
                name: "IX_post_comments_CommentById",
                table: "post_comments",
                column: "CommentById");

            migrationBuilder.CreateIndex(
                name: "IX_post_comments_PostId",
                table: "post_comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_LikedById",
                table: "post_likes",
                column: "LikedById");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_PostId",
                table: "post_likes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_post_tags_PostId",
                table: "post_tags",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_post_tags_TagId",
                table: "post_tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_posts_PostById",
                table: "posts",
                column: "PostById");

            migrationBuilder.CreateIndex(
                name: "IX_requested_players_PlayerId",
                table: "requested_players",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_requested_players_SlotId",
                table: "requested_players",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_slot_offers_BookingSlotId",
                table: "slot_offers",
                column: "BookingSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_slot_offers_OffereTo",
                table: "slot_offers",
                column: "OffereTo");

            migrationBuilder.CreateIndex(
                name: "IX_travel_documents_TravelerId",
                table: "travel_documents",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_travel_documents_TravelId",
                table: "travel_documents",
                column: "TravelId");

            migrationBuilder.CreateIndex(
                name: "IX_travel_documents_UploadedBy",
                table: "travel_documents",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_travelers_TravelerId",
                table: "travelers",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_travelers_TravelId",
                table: "travelers",
                column: "TravelId");

            migrationBuilder.CreateIndex(
                name: "IX_travels_CreatedBy",
                table: "travels",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_user_game_interest_GameId",
                table: "user_game_interest",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_user_game_interest_UserId",
                table: "user_game_interest",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_game_state_GameId",
                table: "user_game_state",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_user_game_state_UserId",
                table: "user_game_state",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_DepartmentId",
                table: "users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_ReportTo",
                table: "users",
                column: "ReportTo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "booking_players");

            migrationBuilder.DropTable(
                name: "daily_celebrations");

            migrationBuilder.DropTable(
                name: "expense_proof");

            migrationBuilder.DropTable(
                name: "game_operation_window");

            migrationBuilder.DropTable(
                name: "game_slot_players");

            migrationBuilder.DropTable(
                name: "game_slot_waiting_players");

            migrationBuilder.DropTable(
                name: "job_referrals");

            migrationBuilder.DropTable(
                name: "job_reviewers");

            migrationBuilder.DropTable(
                name: "job_shared");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "post_comments");

            migrationBuilder.DropTable(
                name: "post_likes");

            migrationBuilder.DropTable(
                name: "post_tags");

            migrationBuilder.DropTable(
                name: "requested_players");

            migrationBuilder.DropTable(
                name: "slot_offers");

            migrationBuilder.DropTable(
                name: "travel_documents");

            migrationBuilder.DropTable(
                name: "travelers");

            migrationBuilder.DropTable(
                name: "user_game_interest");

            migrationBuilder.DropTable(
                name: "user_game_state");

            migrationBuilder.DropTable(
                name: "expenses");

            migrationBuilder.DropTable(
                name: "game_slot_waiting");

            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "booking_slot");

            migrationBuilder.DropTable(
                name: "expense_category");

            migrationBuilder.DropTable(
                name: "travels");

            migrationBuilder.DropTable(
                name: "game_slots");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "departments");
        }
    }
}
