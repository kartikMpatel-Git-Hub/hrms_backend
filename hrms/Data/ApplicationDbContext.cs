using hrms.Model;
using Microsoft.EntityFrameworkCore;

namespace hrms.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Travel> Travels { get; set; }
        public DbSet<TravelDocument> TravelDocuments { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseProof> ExpenseProofs { get; set; }
        public DbSet<Traveler> Travelers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobReviewer> Reviewers { get; set; }
        public DbSet<JobReferral> Referrals { get; set; }
        public DbSet<JobShared> SharedJobs { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<UserGameInterest> UserGameInterests { get; set; }
        public DbSet<UserGameState> UserGameStates { get; set; }
        public DbSet<GameSlot> GameSlots { get; set; }
        public DbSet<GameQueue> GameQueues { get; set; }
        public DbSet<BookingSlot> BookingSlots { get; set; }
        public DbSet<BookingPlayer> BookingPlayers { get; set; }
        public DbSet<SlotOffere> SlotOffers { get; set; }
        public DbSet<RequestedPlayer> RequestedPlayers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("departments");

                entity.HasKey(d => d.Id);

                entity
                    .Property(d => d.Id)
                    .HasColumnName("pk_department_id");

                entity
                    .Property(d => d.DepartmentName)
                    .IsRequired()
                    .HasMaxLength(30);
            });

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

                entity.Property(u => u.Designation)
                    .HasColumnName("designation")
                    .HasMaxLength(30);

                entity.HasOne(u => u.Reported)
                      .WithMany(m => m.Employees)
                      .HasConstraintName("fk_reported_user_id")
                      .HasForeignKey(u => u.ReportTo)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.Department)
                      .WithMany()
                      .HasConstraintName("fk_department_id")
                      .HasForeignKey(u => u.DepartmentId)
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
                    .Property(td => td.UploadedAt)
                    .HasColumnName("uploaded_at")
                    .IsRequired();

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
                    .HasPrecision(10, 2)
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
                    .Property(e => e.Details)
                    .HasColumnName("details")
                    .HasMaxLength(200);

                entity
                    .Property(e => e.ExpenseDate)
                    .HasColumnName("expense_date");

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
                    .WithMany(e => e.Proofs)
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

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("jobs");

                entity.HasKey(j => j.Id);

                entity
                    .Property(j => j.Id)
                    .HasColumnName("pk_job_id");

                entity
                    .Property(j => j.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(50);

                entity
                    .Property(j => j.JobRole)
                    .IsRequired()
                    .HasColumnName("job_role")
                    .HasMaxLength(20);

                entity
                    .Property(j => j.Place)
                    .IsRequired()
                    .HasColumnName("place")
                    .HasMaxLength(30);

                entity
                    .Property(j => j.Requirements)
                    .IsRequired()
                    .HasColumnName("requirements")
                    .HasMaxLength(300);

                entity
                    .Property(j => j.JdUrl)
                    .IsRequired()
                    .HasColumnName("jd_url")
                    .HasMaxLength(500);

                entity
                    .Property(j => j.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active");

                entity
                    .HasOne(n => n.Creater)
                    .WithMany()
                    .HasForeignKey(n => n.CreatedBy)
                    .HasConstraintName("fk_job_creater_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(n => n.Contact)
                    .WithMany()
                    .HasForeignKey(n => n.ContactTo)
                    .HasConstraintName("fk_job_contact_to_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<JobReviewer>(entity =>
            {
                entity.ToTable("job_reviewers");

                entity.HasKey(jr => jr.Id);

                entity
                    .Property(jr => jr.Id)
                    .HasColumnName("pk_job_reviewer_id");

                entity
                    .HasOne(n => n.Reviewer)
                    .WithMany()
                    .HasForeignKey(n => n.ReviewerId)
                    .HasConstraintName("fk_job_reviewer_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(n => n.Job)
                    .WithMany(j => j.Reviewers)
                    .HasForeignKey(n => n.JobId)
                    .HasConstraintName("fk_job_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<JobReferral>(entity =>
            {
                entity.ToTable("job_referrals");

                entity.HasKey(jr => jr.Id);

                entity
                    .Property(jr => jr.Id)
                    .HasColumnName("pk_job_referral_id");

                entity
                    .Property(jr => jr.ReferedPersonName)
                    .IsRequired()
                    .HasColumnName("refered_person_name")
                    .HasMaxLength(50);

                entity
                    .Property(jr => jr.ReferedPersonEmail)
                    .IsRequired()
                    .HasColumnName("refered_person_email")
                    .HasMaxLength(50);

                entity
                    .Property(jr => jr.CvUrl)
                    .IsRequired()
                    .HasColumnName("cv_url")
                    .HasMaxLength(500);

                entity
                    .Property(jr => jr.Note)
                    .IsRequired()
                    .HasColumnName("note")
                    .HasMaxLength(500);

                entity
                    .Property(jr => jr.Status)
                    .HasConversion<String>()
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(20);

                entity
                    .Property(jr => jr.ReferedAt)
                    .IsRequired()
                    .HasColumnName("refered_at");

                entity
                    .HasOne(jr => jr.Referer)
                    .WithMany()
                    .HasForeignKey(jr => jr.ReferedBy)
                    .HasConstraintName("fk_job_referer_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(jr => jr.Job)
                    .WithMany()
                    .HasForeignKey(jr => jr.JobId)
                    .HasConstraintName("fk_refere_job_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<JobShared>(entity =>
            {
                entity.ToTable("job_shared");

                entity.HasKey(js => js.Id);

                entity
                    .Property(js => js.Id)
                    .HasColumnName("pk_job_shared_id");

                entity
                    .Property(js => js.SharedTo)
                    .IsRequired()
                    .HasColumnName("shared_to")
                    .HasMaxLength(50);

                entity
                    .Property(js => js.SharedBy)
                    .IsRequired()
                    .HasColumnName("shared_at");

                entity
                    .HasOne(js => js.Job)
                    .WithMany()
                    .HasForeignKey(jr => jr.JobId)
                    .HasConstraintName("fk_job_shared_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(js => js.Shared)
                    .WithMany()
                    .HasForeignKey(jr => jr.SharedBy)
                    .HasConstraintName("fk_job_shared_user_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("games");

                entity.HasKey(g => g.Id);

                entity
                    .Property(g => g.Id)
                    .HasColumnName("pk_game_id");

                entity
                    .Property(g => g.Name)
                    .IsRequired()
                    .HasColumnName("game_name")
                    .HasMaxLength(30);

                entity
                    .Property(g => g.MaxPlayer)
                    .HasColumnName("max_player_per_game")
                    .IsRequired();

                entity
                    .Property(g => g.MinPlayer)
                    .HasColumnName("min_player_per_game")
                    .IsRequired();
            });

            modelBuilder.Entity<UserGameInterest>(entity =>
            {
                entity.ToTable("user_game_interest");

                entity.HasKey(ug => ug.Id);

                entity
                    .Property(ug => ug.Id)
                    .HasColumnName("pk_user_game_id");

                entity
                    .Property(gq => gq.Status)
                    .HasConversion<String>()
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(20);

                entity
                    .HasOne(ug => ug.Game)
                    .WithMany()
                    .HasForeignKey(ug => ug.GameId)
                    .HasConstraintName("fk_user_game_game_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(ug => ug.User)
                    .WithMany()
                    .HasForeignKey(ug => ug.UserId)
                    .HasConstraintName("fk_user_game_user_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserGameState>(entity =>
            {
                entity.ToTable("user_game_state");

                entity.HasKey(ug => ug.Id);

                entity
                    .Property(ug => ug.Id)
                    .HasColumnName("pk_user_game_state_id");

                entity
                    .Property(ug => ug.LastPlayedAt)
                    .IsRequired()
                    .HasColumnName("last_played_at");

                entity
                    .Property(ug => ug.GamePlayed)
                    .IsRequired()
                    .HasColumnName("game_played");

                entity
                    .HasOne(ug => ug.Game)
                    .WithMany()
                    .HasForeignKey(ug => ug.GameId)
                    .HasConstraintName("fk_user_state_game_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(ug => ug.User)
                    .WithMany()
                    .HasForeignKey(ug => ug.UserId)
                    .HasConstraintName("fk_game_state_user_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GameSlot>(entity =>
            {
                entity.ToTable("game_slots");

                entity.HasKey(gs => gs.Id);

                entity
                    .Property(gs => gs.Id)
                    .HasColumnName("pk_game_slot_id");

                entity
                    .Property(gs => gs.StartTime)
                    .IsRequired()
                    .HasColumnName("start_time");

                entity
                    .Property(gs => gs.EndTime)
                    .IsRequired()
                    .HasColumnName("end_time");

                entity
                    .HasOne(ug => ug.Game)
                    .WithMany(g => g.Slots)
                    .HasForeignKey(ug => ug.GameId)
                    .HasConstraintName("fk_user_slot_game_id")
                    .OnDelete(DeleteBehavior.Restrict);


            });

            modelBuilder.Entity<GameQueue>(entity =>
            {
                entity.ToTable("game_queue");

                entity.HasKey(gq => gq.Id);

                entity
                    .Property(gq => gq.Id)
                    .HasColumnName("pk_game_queue_id");

                entity
                    .HasOne(gq => gq.Game)
                    .WithMany()
                    .HasForeignKey(gq => gq.GameId)
                    .HasConstraintName("fk_game_queue_game_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(gq => gq.Player)
                    .WithMany()
                    .HasForeignKey(gq => gq.PlayerId)
                    .HasConstraintName("fk_game_queue_player_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .Property(gq => gq.Status)
                    .HasConversion<String>()
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(20);

                entity
                    .Property(gq => gq.EnqueueAt)
                    .IsRequired()
                    .HasColumnName("enqueue_at");
            });

            modelBuilder.Entity<BookingSlot>(entity =>
            {
                entity.ToTable("booking_slot");

                entity.HasKey(wgs => wgs.Id);

                entity
                    .Property(wgs => wgs.Id)
                    .HasColumnName("pk_booking_slot_id");

                entity
                    .HasOne(wgs => wgs.Game)
                    .WithMany(g => g.BookingSlots)
                    .HasForeignKey(gq => gq.GameId)
                    .HasConstraintName("fk_bookin_slot_game_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(wgs => wgs.Booked)
                    .WithMany()
                    .HasForeignKey(wgs => wgs.BookedBy)
                    .HasConstraintName("fk_booking_slot_booked_by_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .Property(gq => gq.Status)
                    .HasConversion<String>()
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(20);

                entity
                     .Property(gq => gq.CurrentPriorityOrder)
                     .IsRequired()
                     .HasColumnName("current_priority_order");

                entity
                    .Property(gq => gq.StartTime)
                    .IsRequired()
                    .HasColumnName("start_time");

                entity
                    .Property(gq => gq.EndTime)
                    .IsRequired()
                    .HasColumnName("end_time");

                entity
                    .Property(gq => gq.Date)
                    .IsRequired()
                    .HasColumnName("date");

            });

            modelBuilder.Entity<BookingPlayer>(entity =>
            {
                entity.ToTable("booking_players");

                entity.HasKey(bp => bp.Id);

                entity
                    .Property(bp => bp.Id)
                    .HasColumnName("pk_booking_id");

                entity
                    .HasOne(bp => bp.Slot)
                    .WithMany(wgb => wgb.Players)
                    .HasForeignKey(bp => bp.SlotId)
                    .HasConstraintName("fk_game_slot_slot_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(bp => bp.Player)
                    .WithMany()
                    .HasForeignKey(bp => bp.PlayerId)
                    .HasConstraintName("fk_game_slot_player_id")
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<SlotOffere>(entity =>
            {
                entity.ToTable("slot_offers");

                entity.HasKey(so => so.Id);

                entity
                    .Property(so => so.Id)
                    .HasColumnName("pk_slot_offer_id");

                entity
                    .HasOne(so => so.Slot)
                    .WithMany(s => s.Offers)
                    .HasForeignKey(so => so.BookingSlotId)
                    .HasConstraintName("fk_slot_offer_booking_slot_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(so => so.Offered)
                    .WithMany()
                    .HasForeignKey(so => so.OffereTo)
                    .HasConstraintName("fk_slot_offer_offered_to_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .Property(so => so.PriorityOrder)
                    .IsRequired()
                    .HasColumnName("priority_order");

                entity
                    .Property(so => so.CreatedAt)
                    .IsRequired()
                    .HasColumnName("created_at");

                entity
                    .Property(so => so.ExpiredAt)
                    .HasColumnName("expired_at");

                entity
                    .Property(so => so.Status)
                    .HasConversion<String>()
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<RequestedPlayer>(entity =>
            {
                entity.ToTable("requested_players");

                entity.HasKey(rp => rp.Id);

                entity
                    .Property(rp => rp.Id)
                    .HasColumnName("pk_requested_player_id");

                entity
                    .HasOne(rp => rp.Slot)
                    .WithMany(sbr => sbr.RequestedPlayers)
                    .HasForeignKey(rp => rp.SlotId)
                    .HasConstraintName("fk_requested_player_slot_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(rp => rp.Player)
                    .WithMany()
                    .HasForeignKey(rp => rp.PlayerId)
                    .HasConstraintName("fk_requested_player_player_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tags");

                entity.HasKey(t => t.Id);

                entity
                    .Property(t => t.Id)
                    .HasColumnName("pk_tag_id");

                entity
                    .Property(t => t.TagName)
                    .IsRequired()
                    .HasColumnName("tag_name")
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("posts");

                entity.HasKey(p => p.Id);

                entity
                    .Property(p => p.Id)
                    .HasColumnName("pk_post_id");

                entity
                    .Property(p => p.PostUrl)
                    .IsRequired()
                    .HasColumnName("post_url")
                    .HasMaxLength(500);
                entity
                     .Property(p => p.Title)
                     .IsRequired()
                     .HasColumnName("title")
                     .HasMaxLength(100);

                entity.Property(p => p.Description)
                 .HasColumnName("description")
                 .HasMaxLength(1000);

                entity
                    .Property(p => p.IsPublic)
                    .IsRequired()
                    .HasColumnName("is_public");

                entity
                    .Property(p => p.InAppropriate)
                    .IsRequired()
                    .HasColumnName("is_inappropriate");

                entity
                    .HasOne(p => p.PostByUser)
                    .WithMany()
                    .HasForeignKey(p => p.PostById)
                    .HasConstraintName("fk_post_by_user_id");
            });
       
            modelBuilder.Entity<PostComment>(entity =>
            {
                entity.ToTable("post_comments");

                entity.HasKey(pc => pc.Id);

                entity
                    .Property(pc => pc.Id)
                    .HasColumnName("pk_post_comment_id");

                entity
                    .Property(pc => pc.Comment)
                    .IsRequired()
                    .HasColumnName("comment")
                    .HasMaxLength(500);

                entity
                    .HasOne(pc => pc.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(pc => pc.PostId)
                    .HasConstraintName("fk_post_comment_post_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(pc => pc.CommentBy)
                    .WithMany()
                    .HasForeignKey(pc => pc.CommentById)
                    .HasConstraintName("fk_post_comment_by_user_id")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PostLike>(entity =>
            {
                entity.ToTable("post_likes");

                entity.HasKey(pl => pl.Id);

                entity
                    .Property(pl => pl.Id)
                    .HasColumnName("pk_post_like_id");

                entity
                    .HasOne(pl => pl.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(pl => pl.PostId)
                    .HasConstraintName("fk_post_like_post_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(pl => pl.LikedBy)
                    .WithMany()
                    .HasForeignKey(pl => pl.LikedById)
                    .HasConstraintName("fk_post_like_by_user_id")
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity  
                    .Property(pl => pl.IsDeleted)
                    .IsRequired()
                    .HasColumnName("is_deleted");
            });
        
            modelBuilder.Entity<PostTag>(entity =>
            {
                entity.ToTable("post_tags");

                entity.HasKey(pt => pt.Id);

                entity
                    .Property(pt => pt.Id)
                    .HasColumnName("pk_post_tag_id");

                entity
                    .HasOne(pt => pt.Post)
                    .WithMany(p => p.PostTags)
                    .HasForeignKey(pt => pt.PostId)
                    .HasConstraintName("fk_post_tag_post_id")
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(pt => pt.Tag)
                    .WithMany()
                    .HasForeignKey(pt => pt.TagId)
                    .HasConstraintName("fk_post_tag_tag_id")
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity  
                    .Property(pt => pt.IsDeleted)
                    .IsRequired()
                    .HasColumnName("is_deleted");
            });
        }
    }
}
