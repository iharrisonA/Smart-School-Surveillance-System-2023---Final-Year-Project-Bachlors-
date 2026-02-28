using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSS.Data;
using SSSS.Models;

namespace SSSS.Controllers
{
    // ══════════════════════════════════════════════════════════════════════
    //  TEACHER CONTROLLER
    // ══════════════════════════════════════════════════════════════════════
    public class TeacherController : Controller
    {
        private readonly AppDbContext _db;
        public TeacherController(AppDbContext db) => _db = db;

        private bool IsTeacher() => HttpContext.Session.GetString("UserRole") == "Teacher";
        private IActionResult Deny() => RedirectToAction("Login", "Account");

        private async Task<Teacher?> CurrentTeacher()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            return await _db.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
        }

        // ── Dashboard ──────────────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            if (!IsTeacher()) return Deny();
            var teacher = await CurrentTeacher();
            if (teacher == null) return Deny();

            ViewBag.TeacherName    = teacher.FullName;
            ViewBag.TotalStudents  = await _db.Students.CountAsync();
            ViewBag.MyClasses      = await _db.SubjectAssignments
                .Where(sa => sa.TeacherId == teacher.Id)
                .Select(sa => sa.ClassId).Distinct().CountAsync();
            ViewBag.MySubjects     = await _db.SubjectAssignments
                .Where(sa => sa.TeacherId == teacher.Id)
                .Select(sa => sa.SubjectId).Distinct().CountAsync();
            ViewBag.RecentAnnouncements = await _db.Announcements
                .Include(a => a.Teacher)
                .OrderByDescending(a => a.CreatedAt).Take(3).ToListAsync();
            return View();
        }

        // ── Attendance ─────────────────────────────────────────────────────
        public async Task<IActionResult> Attendance()
        {
            if (!IsTeacher()) return Deny();
            var teacher = await CurrentTeacher();
            if (teacher == null) return Deny();

            var assignments = await _db.SubjectAssignments
                .Where(sa => sa.TeacherId == teacher.Id)
                .Include(sa => sa.Subject).Include(sa => sa.Class).ToListAsync();
            ViewBag.Assignments = assignments;
            ViewBag.Subjects    = new SelectList(assignments.Select(a => a.Subject).DistinctBy(s => s.Id), "Id", "Name");
            ViewBag.Classes     = new SelectList(assignments.Select(a => a.Class).DistinctBy(c => c.Id), "Id", "Name");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAttendance(AttendanceViewModel vm)
        {
            if (!IsTeacher()) return Deny();
            var teacher = await CurrentTeacher();
            if (teacher == null) return Deny();

            foreach (var entry in vm.Entries)
            {
                var existing = await _db.Attendances.FirstOrDefaultAsync(a =>
                    a.StudentId == entry.StudentId &&
                    a.SubjectId == vm.SubjectId &&
                    a.Date.Date == vm.Date.Date);

                if (existing != null)
                {
                    existing.Status  = entry.Status;
                    existing.Remarks = entry.Remarks;
                }
                else
                {
                    _db.Attendances.Add(new Attendance
                    {
                        StudentId = entry.StudentId,
                        TeacherId = teacher.Id,
                        SubjectId = vm.SubjectId,
                        Date      = vm.Date,
                        Status    = entry.Status,
                        Remarks   = entry.Remarks
                    });
                }
            }
            await _db.SaveChangesAsync();
            TempData["Success"] = "Attendance saved.";
            return RedirectToAction("Attendance");
        }

        // ── Marks ──────────────────────────────────────────────────────────
        public async Task<IActionResult> Marks()
        {
            if (!IsTeacher()) return Deny();
            var teacher = await CurrentTeacher();
            if (teacher == null) return Deny();

            var assignments = await _db.SubjectAssignments
                .Where(sa => sa.TeacherId == teacher.Id)
                .Include(sa => sa.Subject).Include(sa => sa.Class).ToListAsync();
            ViewBag.Subjects = new SelectList(assignments.Select(a => a.Subject).DistinctBy(s => s.Id), "Id", "Name");
            ViewBag.Classes  = new SelectList(assignments.Select(a => a.Class).DistinctBy(c => c.Id), "Id", "Name");
            ViewBag.ExamTypes = new List<string> { "Quiz", "Assignment", "Midterm", "Final" };
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitMarks(MarkEntryViewModel vm)
        {
            if (!IsTeacher()) return Deny();
            var teacher = await CurrentTeacher();
            if (teacher == null) return Deny();

            foreach (var entry in vm.Entries)
            {
                _db.Marks.Add(new Mark
                {
                    StudentId      = entry.StudentId,
                    TeacherId      = teacher.Id,
                    SubjectId      = vm.SubjectId,
                    ExamType       = vm.ExamType,
                    ObtainedMarks  = entry.ObtainedMarks,
                    TotalMarks     = vm.TotalMarks,
                    Date           = DateTime.Now
                });
            }
            await _db.SaveChangesAsync();
            TempData["Success"] = "Marks saved.";
            return RedirectToAction("Marks");
        }

        // ── Announcements ──────────────────────────────────────────────────
        public async Task<IActionResult> Announcements()
        {
            if (!IsTeacher()) return Deny();
            var teacher = await CurrentTeacher();
            if (teacher == null) return Deny();
            return View(await _db.Announcements.Where(a => a.TeacherId == teacher.Id)
                .OrderByDescending(a => a.CreatedAt).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PostAnnouncement(string title, string content, string audience)
        {
            if (!IsTeacher()) return Deny();
            var teacher = await CurrentTeacher();
            if (teacher == null) return Deny();
            _db.Announcements.Add(new Announcement
                { TeacherId = teacher.Id, Title = title, Content = content, Audience = audience });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Announcement posted.";
            return RedirectToAction("Announcements");
        }

        // ── Lecture Materials ──────────────────────────────────────────────
        public async Task<IActionResult> Lectures()
        {
            if (!IsTeacher()) return Deny();
            var teacher = await CurrentTeacher();
            if (teacher == null) return Deny();

            ViewBag.Materials = await _db.LectureMaterials
                .Where(m => m.TeacherId == teacher.Id)
                .Include(m => m.Subject).OrderByDescending(m => m.UploadedAt).ToListAsync();
            ViewBag.Subjects = new SelectList(await _db.Subjects.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLecture(int subjectId, string title, string? description, IFormFile? file)
        {
            if (!IsTeacher()) return Deny();
            var teacher = await CurrentTeacher();
            if (teacher == null) return Deny();

            string? filePath = null;
            if (file != null && file.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsDir);
                filePath = Path.Combine("uploads", $"{Guid.NewGuid()}_{file.FileName}");
                using var stream = new FileStream(Path.Combine("wwwroot", filePath), FileMode.Create);
                await file.CopyToAsync(stream);
            }

            _db.LectureMaterials.Add(new LectureMaterial
            {
                TeacherId   = teacher.Id,
                SubjectId   = subjectId,
                Title       = title,
                Description = description,
                FilePath    = filePath
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Material uploaded.";
            return RedirectToAction("Lectures");
        }

        // ── Q&A ────────────────────────────────────────────────────────────
        public async Task<IActionResult> QA()
        {
            if (!IsTeacher()) return Deny();
            return View(await _db.QAQuestions
                .Include(q => q.Subject)
                .Include(q => q.Student)
                .Include(q => q.Answers)
                .OrderByDescending(q => q.AskedAt).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PostAnswer(int questionId, string answerText)
        {
            if (!IsTeacher()) return Deny();
            var teacher = await CurrentTeacher();
            if (teacher == null) return Deny();
            _db.QAAnswers.Add(new QAAnswer { QuestionId = questionId, TeacherId = teacher.Id, AnswerText = answerText });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Answer posted.";
            return RedirectToAction("QA");
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    //  STUDENT CONTROLLER
    // ══════════════════════════════════════════════════════════════════════
    public class StudentController : Controller
    {
        private readonly AppDbContext _db;
        public StudentController(AppDbContext db) => _db = db;

        private bool IsStudent() => HttpContext.Session.GetString("UserRole") == "Student";
        private IActionResult Deny() => RedirectToAction("Login", "Account");

        private async Task<Student?> CurrentStudent()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            return await _db.Students.Include(s => s.Class).FirstOrDefaultAsync(s => s.UserId == userId);
        }

        // ── Dashboard ──────────────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            if (!IsStudent()) return Deny();
            var student = await CurrentStudent();
            if (student == null) return Deny();

            ViewBag.StudentName  = student.FullName;
            ViewBag.ClassName    = student.Class?.Name ?? "Not Assigned";
            ViewBag.Announcements = await _db.Announcements
                .Include(a => a.Teacher)
                .OrderByDescending(a => a.CreatedAt).Take(5).ToListAsync();
            ViewBag.PendingFees  = await _db.FeeVouchers
                .CountAsync(f => f.StudentId == student.Id && f.Status == "Pending");
            ViewBag.TotalPresent = await _db.Attendances
                .CountAsync(a => a.StudentId == student.Id && a.Status == "Present");
            ViewBag.TotalAbsent  = await _db.Attendances
                .CountAsync(a => a.StudentId == student.Id && a.Status == "Absent");
            return View();
        }

        // ── Attendance ─────────────────────────────────────────────────────
        public async Task<IActionResult> Attendance()
        {
            if (!IsStudent()) return Deny();
            var student = await CurrentStudent();
            if (student == null) return Deny();
            var records = await _db.Attendances
                .Where(a => a.StudentId == student.Id)
                .Include(a => a.Subject)
                .OrderByDescending(a => a.Date).ToListAsync();
            return View(records);
        }

        // ── Marks ──────────────────────────────────────────────────────────
        public async Task<IActionResult> Marks()
        {
            if (!IsStudent()) return Deny();
            var student = await CurrentStudent();
            if (student == null) return Deny();
            var marks = await _db.Marks
                .Where(m => m.StudentId == student.Id)
                .Include(m => m.Subject)
                .OrderByDescending(m => m.Date).ToListAsync();
            return View(marks);
        }

        // ── Lecture Materials ──────────────────────────────────────────────
        public async Task<IActionResult> Materials()
        {
            if (!IsStudent()) return Deny();
            var materials = await _db.LectureMaterials
                .Include(m => m.Subject)
                .Include(m => m.Teacher)
                .OrderByDescending(m => m.UploadedAt).ToListAsync();
            return View(materials);
        }

        // ── Fee Vouchers ───────────────────────────────────────────────────
        public async Task<IActionResult> Fees()
        {
            if (!IsStudent()) return Deny();
            var student = await CurrentStudent();
            if (student == null) return Deny();
            var vouchers = await _db.FeeVouchers
                .Where(f => f.StudentId == student.Id)
                .OrderByDescending(f => f.IssuedDate).ToListAsync();
            return View(vouchers);
        }

        // ── Leave Application ──────────────────────────────────────────────
        public async Task<IActionResult> Applications()
        {
            if (!IsStudent()) return Deny();
            var student = await CurrentStudent();
            if (student == null) return Deny();
            return View(await _db.LeaveApplications
                .Where(a => a.StudentId == student.Id)
                .OrderByDescending(a => a.AppliedAt).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyLeave(string reason, DateTime fromDate, DateTime toDate)
        {
            if (!IsStudent()) return Deny();
            var student = await CurrentStudent();
            if (student == null) return Deny();
            _db.LeaveApplications.Add(new LeaveApplication
                { StudentId = student.Id, Reason = reason, FromDate = fromDate, ToDate = toDate });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Leave application submitted.";
            return RedirectToAction("Applications");
        }

        // ── Q&A ────────────────────────────────────────────────────────────
        public async Task<IActionResult> QA()
        {
            if (!IsStudent()) return Deny();
            ViewBag.Subjects = new SelectList(await _db.Subjects.ToListAsync(), "Id", "Name");
            return View(await _db.QAQuestions
                .Include(q => q.Subject)
                .Include(q => q.Answers).ThenInclude(a => a.Teacher)
                .OrderByDescending(q => q.AskedAt).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AskQuestion(int subjectId, string questionText)
        {
            if (!IsStudent()) return Deny();
            var student = await CurrentStudent();
            if (student == null) return Deny();
            _db.QAQuestions.Add(new QAQuestion
                { StudentId = student.Id, SubjectId = subjectId, QuestionText = questionText });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Question posted.";
            return RedirectToAction("QA");
        }
    }
}
