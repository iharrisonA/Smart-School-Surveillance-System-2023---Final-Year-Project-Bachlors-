using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSSS.Models
{
    // ── User (base for Admin / Teacher / Student) ──────────────────────────
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = "";

        [Required, MaxLength(100)]
        public string Email { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";

        [Required, MaxLength(20)]
        public string Role { get; set; } = ""; // Admin | Teacher | Student

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    // ── Class ──────────────────────────────────────────────────────────────
    public class Class
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = "";   // e.g. "10-A"

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<SubjectAssignment> SubjectAssignments { get; set; } = new List<SubjectAssignment>();
    }

    // ── Subject ────────────────────────────────────────────────────────────
    public class Subject
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(10)]
        public string? Code { get; set; }

        public ICollection<SubjectAssignment> Assignments { get; set; } = new List<SubjectAssignment>();
    }

    // ── SubjectAssignment (Teacher <-> Subject <-> Class) ──────────────────
    public class SubjectAssignment
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int ClassId { get; set; }
        public Class Class { get; set; } = null!;
    }

    // ── Teacher ────────────────────────────────────────────────────────────
    public class Teacher
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = "";

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Qualification { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

        public ICollection<SubjectAssignment> Assignments { get; set; } = new List<SubjectAssignment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public ICollection<LectureMaterial> LectureMaterials { get; set; } = new List<LectureMaterial>();
        public ICollection<QAQuestion> Questions { get; set; } = new List<QAQuestion>();
    }

    // ── Student ────────────────────────────────────────────────────────────
    public class Student
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = "";

        [MaxLength(20)]
        public string? RollNumber { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
        public ICollection<FeeVoucher> FeeVouchers { get; set; } = new List<FeeVoucher>();
        public ICollection<LeaveApplication> LeaveApplications { get; set; } = new List<LeaveApplication>();
        public ICollection<QAQuestion> QAQuestions { get; set; } = new List<QAQuestion>();
    }

    // ── Attendance ─────────────────────────────────────────────────────────
    public class Attendance
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public DateTime Date { get; set; } = DateTime.Today;

        [Required, MaxLength(10)]
        public string Status { get; set; } = "Present"; // Present | Absent | Late

        [MaxLength(200)]
        public string? Remarks { get; set; }
    }

    // ── Mark ───────────────────────────────────────────────────────────────
    public class Mark
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        [Required, MaxLength(50)]
        public string ExamType { get; set; } = ""; // Midterm | Final | Quiz | Assignment

        [Column(TypeName = "decimal(5,2)")]
        public decimal ObtainedMarks { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal TotalMarks { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [MaxLength(200)]
        public string? Remarks { get; set; }
    }

    // ── FeeVoucher ─────────────────────────────────────────────────────────
    public class FeeVoucher
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Month { get; set; } = "";

        public int Year { get; set; } = DateTime.Now.Year;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending | Paid | Overdue

        public DateTime IssuedDate { get; set; } = DateTime.Now;
        public DateTime? PaidDate { get; set; }

        [MaxLength(200)]
        public string? Remarks { get; set; }
    }

    // ── Announcement ───────────────────────────────────────────────────────
    public class Announcement
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Title { get; set; } = "";

        [Required]
        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string Audience { get; set; } = "All"; // All | Students | Teachers
    }

    // ── LectureMaterial ────────────────────────────────────────────────────
    public class LectureMaterial
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Title { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? FilePath { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }

    // ── LeaveApplication ───────────────────────────────────────────────────
    public class LeaveApplication
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Reason { get; set; } = "";

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending | Approved | Rejected

        public DateTime AppliedAt { get; set; } = DateTime.Now;

        [MaxLength(200)]
        public string? AdminRemarks { get; set; }
    }

    // ── Q&A ────────────────────────────────────────────────────────────────
    public class QAQuestion
    {
        public int Id { get; set; }

        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        [Required, MaxLength(300)]
        public string QuestionText { get; set; } = "";

        public DateTime AskedAt { get; set; } = DateTime.Now;

        public ICollection<QAAnswer> Answers { get; set; } = new List<QAAnswer>();
    }

    public class QAAnswer
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }
        public QAQuestion Question { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        [Required]
        public string AnswerText { get; set; } = "";

        public DateTime AnsweredAt { get; set; } = DateTime.Now;
    }

    // ── ViewModels ─────────────────────────────────────────────────────────
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }

    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalClasses { get; set; }
        public int TotalSubjects { get; set; }
        public int PendingApplications { get; set; }
        public int PendingFees { get; set; }
        public List<Announcement> RecentAnnouncements { get; set; } = new();
    }

    public class AttendanceViewModel
    {
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public List<AttendanceEntry> Entries { get; set; } = new();
    }

    public class AttendanceEntry
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public string Status { get; set; } = "Present";
        public string? Remarks { get; set; }
    }

    public class MarkEntryViewModel
    {
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public string ExamType { get; set; } = "";
        public decimal TotalMarks { get; set; }
        public List<MarkEntry> Entries { get; set; } = new();
    }

    public class MarkEntry
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public decimal ObtainedMarks { get; set; }
    }
}
