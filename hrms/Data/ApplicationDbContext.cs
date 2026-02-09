using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}
        public DbSet<User> Users { get; set; }
        public DbSet<Travel> Travels { get; set; }
        public DbSet<TravelDocument> TravelDocuments { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseProof> ExpenseProofs { get; set; }
        public DbSet<Traveler> Travelers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(u => u.Id);

                entity
                    .Property(u => u.Id)
                    .HasColumnName("pk_user_id");

                entity.Property(u => u.FullName)
                      .HasColumnName("full_name")
                      .IsRequired()
                      .HasMaxLength(150);

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.Email)
                      .HasColumnName("email")
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.HashPassword)
                    .HasColumnName("password")
                    .HasMaxLength(500);

                entity.Property(u => u.Image)
                    .HasColumnName("image_url")
                    .HasMaxLength(500);

                entity.Property(u => u.Role)
                    .HasColumnName("user_role")
                    .HasConversion<String>()
                    .HasMaxLength(15);

                entity.Property(u => u.DateOfBirth)
                    .IsRequired()
                    .HasColumnName("date_of_birth");

                entity.Property(u => u.DateOfJoin)
                    .IsRequired()
                    .HasColumnName("date_of_joining");

                entity.HasOne(u => u.Manager)
                      .WithMany(m => m.Employees)
                      .HasConstraintName("fk_managaer_user_id")
                      .HasForeignKey(u => u.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Travel>(entity =>
            {
                entity.ToTable("travels");

                entity.HasKey(t => t.Id);

                entity
                    .Property(t => t.Id)
                    .HasColumnName("pk_travel_id");

                entity
                    .Property(t => t.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(50);

                entity
                    .Property(t => t.Desciption)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(300);

                entity.Property(u => u.StartDate)
                    .IsRequired()
                    .HasColumnName("travel_start_date");

                entity.Property(u => u.EndDate)
                    .IsRequired()
                    .HasColumnName("trael_end_date");

                entity
                    .Property(t => t.Location)
                    .IsRequired()
                    .HasColumnName("location")
                    .HasMaxLength(50);

                entity
                    .Property(t => t.MaxAmountLimit)
                    .IsRequired()
                    .HasPrecision(10, 2)
                    .HasColumnName("expense_max_amount_limit");

                entity
                    .HasOne(t => t.Creater)
                    .WithMany()
                    .HasConstraintName("fk_created_user_by")
                    .HasForeignKey(u => u.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Traveler>(entity =>
            {
                entity.ToTable("travelers");

                entity.HasKey(t => t.Id);

                entity
                    .Property(t => t.Id)
                    .HasColumnName("pk_traveler_id");

                entity
                    .HasOne(t => t.Travel)
                    .WithMany(tr => tr.Travelers)
                    .HasConstraintName("fk_traveler_treavel_id")
                    .HasForeignKey(u => u.TravelId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity
                    .HasOne(t => t.Travelerr)
                    .WithMany()
                    .HasConstraintName("fk_traveler_user_id")
                    .HasForeignKey(u => u.TravelerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TravelDocument>(entity =>
            {
                entity.ToTable("travel_documents");

                entity.HasKey(td => td.Id);

                entity
                    .Property(td => td.Id)
                    .HasColumnName("pk_travel_document_id");

                entity
                    .Property(td => td.DocumentUrl)
                    .HasColumnName("document_url")
                    .IsRequired()
                    .HasMaxLength(400);

                entity
                    .Property(td => td.DocumentName)
                    .HasColumnName("document_name")
                    .IsRequired()
                    .HasMaxLength(30);

                entity
                    .Property(td => td.DocumentType)
                    .HasColumnName("document_type")
                    .IsRequired()
                    .HasMaxLength(50);

                entity
                    .HasOne(td => td.Travell)
                    .WithMany()
                    .HasConstraintName("fk_travel_document_id")
                    .HasForeignKey(t => t.TravelId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(td => td.Traveler)
                    .WithMany()
                    .HasConstraintName("fk_traveler_document_id")
                    .HasForeignKey(u => u.TravelerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(td => td.Uploader)
                    .WithMany()
                    .HasConstraintName("fk_uploaded_document_by")
                    .HasForeignKey(u => u.UploadedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExpenseCategory>(entity =>
            {
                entity.ToTable("expense_category");

                entity.HasKey(ec => ec.Id);

                entity
                    .Property(ec => ec.Id)
                    .HasColumnName("pk_expense_category_id");

                entity
                    .Property(ec => ec.Category)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.ToTable("expenses");

                entity.HasKey(e => e.Id);

                entity
                    .Property(e => e.Id)
                    .HasColumnName("pk_expense_id");

                entity
                    .Property(e => e.Amount)
                    .HasPrecision(10,2)
                    .HasColumnName("expense_amount")
                    .IsRequired();

                entity
                    .Property(e => e.Status)
                    .HasConversion<String>()
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(20);

                entity
                    .Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasMaxLength(100);

                entity
                    .HasOne(e => e.Travel)
                    .WithMany()
                    .HasForeignKey(t => t.TravelId)
                    .HasConstraintName("fk_travel_expense_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(e => e.Traveler)
                    .WithMany()
                    .HasForeignKey(u => u.TravelerId)
                    .HasConstraintName("fk_traveler_expense_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(c => c.CategoryId)
                    .HasConstraintName("fk_category_expense_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExpenseProof>(entity =>
            {
                entity.ToTable("expense_proof");

                entity.HasKey(ep => ep.Id);

                entity
                    .Property(ep => ep.Id)
                    .HasColumnName("pk_expense_proof_id");

                entity
                    .Property(ep => ep.ProofDocumentUrl)
                    .IsRequired()
                    .HasColumnName("proof_document_url")
                    .HasMaxLength(500);

                entity
                    .Property(ep => ep.DocumentType)
                    .IsRequired()
                    .HasColumnName("expense_document_type")
                    .HasMaxLength(50);

                entity
                    .Property(ep => ep.Remakrs)
                    .HasColumnName("remarks")
                    .HasMaxLength(200);

                entity
                    .HasOne(ep => ep.Expense)
                    .WithMany()
                    .HasForeignKey(e => e.ExpenseId)
                    .HasConstraintName("fk_expense_proof_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");

                entity.HasKey(n => n.Id);

                entity
                    .Property(n => n.Id)
                    .HasColumnName("pk_notification_id");

                entity
                    .Property(n => n.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(50);

                entity
                    .Property(n => n.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity
                    .Property(n => n.IsViewed)
                    .IsRequired()
                    .HasColumnName("is_viewed");

                entity
                    .Property(n => n.NotificationDate)
                    .IsRequired()
                    .HasColumnName("notification_date");

                entity
                    .HasOne(n => n.Notified)
                    .WithMany()
                    .HasForeignKey(n => n.NotifiedTo)
                    .HasConstraintName("fk_notified_user_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }

    }
}
