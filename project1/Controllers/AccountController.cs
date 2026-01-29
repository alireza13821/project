using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Data;
using project1.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace project1.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyDbContext _dbcontext;

        public AccountController(MyDbContext context)
        {
            _dbcontext = context;
        }

        // Register
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
            User user = new User();
            user.Name = register.Name;
            user.Email = register.Email!.ToLower();
            user.Password = register.Password!;
            user.RegisterDate = DateTime.Now;
            user.Phone = register.Phone;
            user.Role = "User";

            _dbcontext.Users.Add(user);
            _dbcontext.SaveChanges();
            return RedirectToAction("Welcome");
        }

        public IActionResult IdentifyDuplicateEmail(string email)
        {
            if (!email.EndsWith(".ir") && (!email.EndsWith(".com")))
            {
                return Json("ایمیل می‌بایست با ir. یا com. خاتمه یابد");
            }
            if (_dbcontext.Users.Any(c => c.Email == email))
            {
                return Json("کاربر فعلی با این ایمیل قبلا ثبت نام شده است!");
            }
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult VerifyPhone(string phone)
        {
            if (_dbcontext.Users.Any(u => u.Phone == phone))
            {
                return Json("این شماره تلفن قبلاً ثبت شده است");
            }
            // بررسی شروع با 09
            if (!phone.StartsWith("09"))
            {
                return Json("شماره همراه باید با 09 شروع شود");
            }
            // بررسی طول 11 رقمی
            if (phone.Length != 11)
            {
                return Json("شماره همراه باید 11 رقمی باشد");
            }
            // بررسی اینکه فقط شامل رقم باشد
            if (!Regex.IsMatch(phone, @"^\d+$"))
            {
                return Json("شماره همراه باید فقط شامل رقم باشد");
            }        
            return Json(true);
        }

        public IActionResult Welcome()
        {
            return View();
        }

        // Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            else
            {
                var user = _dbcontext.Users.SingleOrDefault(i =>
                i.Password == login.Password && i.Email == login.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Password", "کلمه عبور نادرست می‌باشد");
                    return View(login);
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("Role", user.Role!),
                        new Claim("UserId", user.Id!.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(principal);
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public IActionResult VerifyEmail(string email)
        {
            if (_dbcontext.Users.Any(c => c.Email == email))
            {
                return Json(true);
            }
            return Json("ایمیل نادرست وارد شده است!");
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home/Index");
        }

        // لیست کاربران (فقط ادمین)
        [Authorize(policy: "Admin")]
        public IActionResult SearchUsers(string q)
        {
            var users = _dbcontext.Users
                .Where(u =>
                    u.Name.Contains(q) ||
                    u.Email.Contains(q) ||
                    u.Phone!.Contains(q))
                .ToList();
            return View(users);
        }
        public IActionResult UserList()
        {
            var users = _dbcontext.Users.ToList();
            return View(users);
        }

        // نمایش صفحه ویرایش کاربر
        [Authorize(policy: "Admin")]
        public IActionResult EditUser(int id)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            return View("~/Views/Account/EditUser.cshtml", user);
        }
        // ذخیره تغییرات کاربر
        [HttpPost]
        [Authorize(policy: "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(int id, project1.Models.User model)
        {
            if (id != model.Id) return BadRequest();
            var user = _dbcontext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            user.Name = user.Name;
            user.Email = user.Email;
            user.Password = model.Password;
            user.Role = model.Role;
            user.Phone = user.Phone;
            user.IsPremium = user.IsPremium;
            user.IsActive = model.IsActive;
            user.TotalFineAmount = user.TotalFineAmount;
            user.IsBlocked = user.IsBlocked;

            _dbcontext.SaveChanges();
            return RedirectToAction("Index", "Users");
        }
    }
}

