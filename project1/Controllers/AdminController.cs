using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project1.Data;
using project1.Models;
using System.Security.Claims;

namespace project1.Controllers
{
    
    public class AdminController : Controller
    {
        private MyDBContext _dbContext;
        public AdminController(MyDBContext context)
        {

            _dbContext = context;

        }

        public IActionResult AdminLogin(AdminLoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login); 
            }

            
            var user = _dbContext.Users.SingleOrDefault(u => u.Email == login.Email && u.Password == login.Password);

            if (user == null || user.Role != "Admin") 
            {
                ModelState.AddModelError("Email", "اطلاعات ورود صحیح نیست یا شما ادمین نیستید.");
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
            return RedirectToAction("Index", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminLogout()
        {

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("AdminLogin", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = _dbContext.Users.ToList();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult PromoteToLibrarian(int userId)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Role = "librarian"; 
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBook(AddBookViewModel AddBookViewModel)
        {
            var book = new Book();
            book.Name = AddBookViewModel.Name;
            book.Description = AddBookViewModel.Description;
            book.Quantity=AddBookViewModel.Quantity;

            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();

            if (AddBookViewModel.picture?.Length > 0)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "img",
                    book.Id + Path.GetExtension(AddBookViewModel.picture.FileName));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    AddBookViewModel.picture.CopyTo(stream);
                }
            }
            return RedirectToAction("Index");
        }

    }
}
