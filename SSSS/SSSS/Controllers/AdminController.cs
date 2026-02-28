using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSS.Data;
using SSSS.Models;

namespace SSSS.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db) => _db = db;

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";
        private IActionResult Deny() => RedirectToAction("Login", "Account");

        // ── Dashboard ──────────────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin()) return Deny();
            var vm = new DashboardViewModel
            {
                TotalStudents       = await _db.Students.CountAsync(),
                TotalTeachers       = await _db.Teachers.CountAsync(),
                TotalClasses        = await _db.Classes.CountAsync(),
                TotalSubjects       = await _db.Subjects.CountAsync(),
                PendingApplications = await _db.LeaveApplications.CountAsync(a => a.Status == "Pending"),
                PendingFees         = await _db.FeeVouchers.CountAsync(f => f.Status == "Pending"),
                RecentAnnouncements = await _db.Announcements
                    .Include(a => a.Teacher)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(5).ToListAsync()
            };
            return View(vm);
        }

        // ── Students ───────────────────────────────────────────────────────
        public async Task<IActionResult> Students()
        {
            if (!IsAdmin()) return Deny();
            var students = await _db.Students.Include(s => s.Class).ToListAsync();
            return View(students);
        }

        public async Task<IActionResult> AddStudent()
        {
            if (!IsAdmin()) return Deny();
            ViewBag.Classes = new SelectList(await _db.Classes.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(Student student, string password)
        {
            if (!IsAdmin()) return Deny();
            var user = new User
            {
                FullName     = student.FullName,
                Email        = student.Email ?? $"student{DateTime.Now.Ticks}@ssss.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role         = "Student"
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            student.UserId = user.Id;
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Student added successfully.";
            return RedirectToAction("Students");
        }

        public async Task<IActionResult> EditStudent(int id)
        {
            if (!IsAdmin()) return Deny();
            var student = await _db.Students.FindAsync(id);
            if (student == null) return NotFound();
            ViewBag.Classes = new SelectList(await _db.Classes.ToListAsync(), "Id", "Name", student.ClassId);
            return View(student);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(Student student)
        {
            if (!IsAdmin()) return Deny();
            _db.Students.Update(student);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Student updated.";
            return RedirectToAction("Students");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            if (!IsAdmin()) return Deny();
            var student = await _db.Students.FindAsync(id);
            if (student != null) { _db.Students.Remove(student); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Student deleted.";
            return RedirectToAction("Students");
        }

        // ── Teachers ───────────────────────────────────────────────────────
        public async Task<IActionResult> Teachers()
        {
            if (!IsAdmin()) return Deny();
            return View(await _db.Teachers.ToListAsync());
        }

        public IActionResult AddTeacher()
        {
            if (!IsAdmin()) return Deny();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeacher(Teacher teacher, string password)
        {
            if (!IsAdmin()) return Deny();
            var user = new User
            {
                FullName     = teacher.FullName,
                Email        = teacher.Email ?? $"teacher{DateTime.Now.Ticks}@ssss.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role         = "Teacher"
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            teacher.UserId = user.Id;
            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Teacher added successfully.";
            return RedirectToAction("Teachers");
        }

        public async Task<IActionResult> EditTeacher(int id)
        {
            if (!IsAdmin()) return Deny();
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeacher(Teacher teacher)
        {
            if (!IsAdmin()) return Deny();
            _db.Teachers.Update(teacher);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Teacher updated.";
            return RedirectToAction("Teachers");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            if (!IsAdmin()) return Deny();
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher != null) { _db.Teachers.Remove(teacher); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Teacher deleted.";
            return RedirectToAction("Teachers");
        }

        // ── Classes ────────────────────────────────────────────────────────
        public async Task<IActionResult> Classes()
        {
            if (!IsAdmin()) return Deny();
            return View(await _db.Classes.Include(c => c.Students).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClass(string name, string? description)
        {
            if (!IsAdmin()) return Deny();
            _db.Classes.Add(new Class { Name = name, Description = description });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Class added.";
            return RedirectToAction("Classes");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClass(int id)
        {
            if (!IsAdmin()) return Deny();
            var cls = await _db.Classes.FindAsync(id);
            if (cls != null) { _db.Classes.Remove(cls); await _db.SaveChangesAsync(); }
            return RedirectToAction("Classes");
        }

        // ── Subjects ───────────────────────────────────────────────────────
        public async Task<IActionResult> Subjects()
        {
            if (!IsAdmin()) return Deny();
            return View(await _db.Subjects.ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubject(string name, string? code)
        {
            if (!IsAdmin()) return Deny();
            _db.Subjects.Add(new Subject { Name = name, Code = code });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Subject added.";
            return RedirectToAction("Subjects");
        }

        // ── Assign Subject ─────────────────────────────────────────────────
        public async Task<IActionResult> AssignSubject()
        {
            if (!IsAdmin()) return Deny();
            ViewBag.Teachers  = new SelectList(await _db.Teachers.ToListAsync(), "Id", "FullName");
            ViewBag.Subjects  = new SelectList(await _db.Subjects.ToListAsync(), "Id", "Name");
            ViewBag.Classes   = new SelectList(await _db.Classes.ToListAsync(),  "Id", "Name");
            ViewBag.Existing  = await _db.SubjectAssignments
                .Include(sa => sa.Teacher).Include(sa => sa.Subject).Include(sa => sa.Class)
                .ToListAsync();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignSubject(int teacherId, int subjectId, int classId)
        {
            if (!IsAdmin()) return Deny();
            var exists = await _db.SubjectAssignments.AnyAsync(
                sa => sa.TeacherId == teacherId && sa.SubjectId == subjectId && sa.ClassId == classId);
            if (!exists)
            {
                _db.SubjectAssignments.Add(new SubjectAssignment
                    { TeacherId = teacherId, SubjectId = subjectId, ClassId = classId });
                await _db.SaveChangesAsync();
                TempData["Success"] = "Subject assigned.";
            }
            else TempData["Error"] = "This assignment already exists.";
            return RedirectToAction("AssignSubject");
        }

        // ── Fee Vouchers ───────────────────────────────────────────────────
        public async Task<IActionResult> Fees()
        {
            if (!IsAdmin()) return Deny();
            var vouchers = await _db.FeeVouchers.Include(f => f.Student).OrderByDescending(f => f.IssuedDate).ToListAsync();
            ViewBag.Students = new SelectList(await _db.Students.ToListAsync(), "Id", "FullName");
            return View(vouchers);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateVoucher(int studentId, string month, int year, decimal amount)
        {
            if (!IsAdmin()) return Deny();
            _db.FeeVouchers.Add(new FeeVoucher
            {
                StudentId = studentId, Month = month, Year = year, Amount = amount
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Fee voucher generated.";
            return RedirectToAction("Fees");
        }

        public async Task<IActionResult> ViewVoucher(int id)
        {
            if (!IsAdmin()) return Deny();
            var voucher = await _db.FeeVouchers.Include(f => f.Student).ThenInclude(s => s.Class)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkFeePaid(int id)
        {
            if (!IsAdmin()) return Deny();
            var voucher = await _db.FeeVouchers.FindAsync(id);
            if (voucher != null) { voucher.Status = "Paid"; voucher.PaidDate = DateTime.Now; await _db.SaveChangesAsync(); }
            return RedirectToAction("Fees");
        }

        // ── Leave Applications ─────────────────────────────────────────────
        public async Task<IActionResult> Applications()
        {
            if (!IsAdmin()) return Deny();
            var apps = await _db.LeaveApplications.Include(a => a.Student)
                .OrderByDescending(a => a.AppliedAt).ToListAsync();
            return View(apps);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewApplication(int id, string status, string? remarks)
        {
            if (!IsAdmin()) return Deny();
            var app = await _db.LeaveApplications.FindAsync(id);
            if (app != null) { app.Status = status; app.AdminRemarks = remarks; await _db.SaveChangesAsync(); }
            TempData["Success"] = $"Application {status.ToLower()}.";
            return RedirectToAction("Applications");
        }

        // ── Announcements ──────────────────────────────────────────────────
        public async Task<IActionResult> Announcements()
        {
            if (!IsAdmin()) return Deny();
            return View(await _db.Announcements.Include(a => a.Teacher)
                .OrderByDescending(a => a.CreatedAt).ToListAsync());
        }

        // ── Add User (shortcut) ────────────────────────────────────────────
        public IActionResult AddUser()
        {
            if (!IsAdmin()) return Deny();
            return View();
        }
    }
}
