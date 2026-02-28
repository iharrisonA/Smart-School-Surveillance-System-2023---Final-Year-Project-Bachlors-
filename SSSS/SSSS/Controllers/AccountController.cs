using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSSS.Data;
using SSSS.Models;

namespace SSSS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db) => _db = db;

        // GET /Account/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
                return RedirectToDashboard();
            return View();
        }

        // POST /Account/Login
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToDashboard(user.Role);
        }

        // GET /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private IActionResult RedirectToDashboard(string? role = null)
        {
            role ??= HttpContext.Session.GetString("UserRole");
            return role switch
            {
                "Admin"   => RedirectToAction("Dashboard", "Admin"),
                "Teacher" => RedirectToAction("Dashboard", "Teacher"),
                "Student" => RedirectToAction("Dashboard", "Student"),
                _         => RedirectToAction("Login")
            };
        }
    }
}
