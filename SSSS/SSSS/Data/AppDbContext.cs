using Microsoft.EntityFrameworkCore;
using SSSS.Models;

namespace SSSS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectAssignment> SubjectAssignments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<FeeVoucher> FeeVouchers { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<LectureMaterial> LectureMaterials { get; set; }
        public DbSet<LeaveApplication> LeaveApplications { get; set; }
        public DbSet<QAQuestion> QAQuestions { get; set; }
        public DbSet<QAAnswer> QAAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique email per user
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // SubjectAssignment composite uniqueness
            modelBuilder.Entity<SubjectAssignment>()
                .HasIndex(sa => new { sa.TeacherId, sa.SubjectId, sa.ClassId })
                .IsUnique();

            // Seed default admin
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "Administrator",
                Email = "admin@ssss.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = new DateTime(2024, 1, 1)
            });

            // Seed sample classes
            modelBuilder.Entity<Class>().HasData(
                new Class { Id = 1, Name = "9-A", Description = "Grade 9 Section A" },
                new Class { Id = 2, Name = "9-B", Description = "Grade 9 Section B" },
                new Class { Id = 3, Name = "10-A", Description = "Grade 10 Section A" },
                new Class { Id = 4, Name = "10-B", Description = "Grade 10 Section B" }
            );

            // Seed sample subjects
            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = 1, Name = "Mathematics", Code = "MATH" },
                new Subject { Id = 2, Name = "English", Code = "ENG" },
                new Subject { Id = 3, Name = "Physics", Code = "PHY" },
                new Subject { Id = 4, Name = "Chemistry", Code = "CHEM" },
                new Subject { Id = 5, Name = "Computer Science", Code = "CS" },
                new Subject { Id = 6, Name = "Biology", Code = "BIO" }
            );
        }
    }
}
