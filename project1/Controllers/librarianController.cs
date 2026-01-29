using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Data;
using project1.Models;
using System.Security.Claims;

namespace project1.Controllers
{
    
    public class librarianController : Controller
    {
        private MyDBContext _dbContext;
        public librarianController(MyDBContext context)
        {

            _dbContext = context;

        }

        public IActionResult librarianLogin(librarianLoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }


            var user = _dbContext.Users.SingleOrDefault(u => u.Email == login.Email && u.Password == login.Password);

            if (user == null || user.Role != "librarian")
            {
                ModelState.AddModelError("Email", "اطلاعات ورود صحیح نیست یا شما کتابدار نیستید.");
                return View(login);
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = login.RememberMe
            };

            HttpContext.SignInAsync(principal, properties);
            return RedirectToAction("Index", "librarian");
        }

        [Authorize(Roles = "librarian")]
        public IActionResult librarianLogout()
        {

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("AdminLogin", "Admin");
        }

        [Authorize(Roles = "librarian")]
        public IActionResult Index()
        {

            var reserveViewModels = _dbContext.Reserves
                .Where(r => r.IsFinaly && !r.IsApproved && r.Status == "در حال بررسی")
                .Include(r => r.User)
                .Include(r => r.ReserveItems)
                .ThenInclude(ri => ri.book)
                .Select(r => new ReserveViewModel
                {
                    Id = r.Id,
                    CreateDate = r.CreateDate,
                    FullName = r.User != null ? $"{r.User.Name} {r.User.LastName}" : "Unknown",
                    ReserveItems = r.ReserveItems,
                    Status = r.Status
                })
                .ToList();

            return View(reserveViewModels);
        }


        [Authorize(Roles = "librarian")]
        [HttpPost]
        public IActionResult ApproveRequest(int reserveId)
        {
            var reserve = _dbContext.Reserves
                .Include(r => r.ReserveItems)
                .ThenInclude(ri => ri.book)
                .SingleOrDefault(r => r.Id == reserveId && !r.IsApproved && r.IsFinaly);

            if (reserve != null)
            {

                foreach (var reserveItem in reserve.ReserveItems)
                {
                    var book = reserveItem.book;
                    if (book.Quantity > 0)
                    {
                        book.Quantity--;  
                    }
                    else
                    {
                        return Content("موجودی یکی از کتاب‌ها کافی نیست.");
                    }
                }

                reserve.IsApproved = true;
                reserve.Status = "تایید شده"; 
                reserve.IsRejected = false;
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "librarian")]
        [HttpPost]
        public IActionResult RejectRequest(int reserveId, string reason)
        {
            var reserve = _dbContext.Reserves
                .SingleOrDefault(r => r.Id == reserveId && !r.IsApproved);

            if (reserve != null)
            {
                reserve.Status = "رد شده";  
                reserve.IsFinaly = true;
                reserve.IsApproved = false;
                reserve.IsRejected = true;
                reserve.RejectionReason = reason; 
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
