using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using project1.Data;
using project1.Models;
using System.Security.Claims;

namespace project1.Controllers
{
    public class AccountController : Controller
    {
        private MyDBContext _dbContext;
        public AccountController(MyDBContext context)
        {

            _dbContext = context;

        }

        public IActionResult AddUser()
        {
            return View(new AddUserViewModel());
        }
        [HttpPost]
        public IActionResult AddUser(AddUserViewModel model)
        {
            if (_dbContext.Users.Any(c => c.Email==model.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "ایمیل تکراری است");
                return View(model);
            }

            if (ModelState.IsValid)
            {

                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {

                        _dbContext.Users.Add(new User
                        {
                            
                            Name = model.Name,
                            LastName = model.LastName,
                            Email = model.Email,
                            Password = model.Password,
                            Role="User"
                        });

                        _dbContext.SaveChanges();
                        transaction.Commit();

                        return RedirectToAction("confirmedMessage");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Content($"خطا در ثبت کاربر : {ex.Message}");
                    }
                }

            }

            return View(model);
        }

        public IActionResult confirmedMessage()
        {
            ViewBag.Message = "کاربر با موفقیت ثبت شد ";
            return View();
        }


        public IActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login); 
            }

            var user = _dbContext.Users.SingleOrDefault(u => u.Email == login.Email && u.Password == login.Password);
            if (user == null || user.Role != "User")
            {
                ModelState.AddModelError("Email", "اطلاعات صحیح نیست");
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

            return Redirect("/");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home/Index");
        }

 
        public IActionResult Index()
        {
            return View();
        }
    }
}
