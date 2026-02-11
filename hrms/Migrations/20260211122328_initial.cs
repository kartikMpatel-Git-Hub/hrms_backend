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
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    designation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                        name: "fk_managaer_user_id",
                        column: x => x.ManagerId,
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
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_notifications_NotifiedTo",
                table: "notifications",
                column: "NotifiedTo");

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
                name: "IX_users_DepartmentId",
                table: "users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_ManagerId",
                table: "users",
                column: "ManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expense_proof");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "travel_documents");

            migrationBuilder.DropTable(
                name: "travelers");

            migrationBuilder.DropTable(
                name: "expenses");

            migrationBuilder.DropTable(
                name: "expense_category");

            migrationBuilder.DropTable(
                name: "travels");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "departments");
        }
    }
}
