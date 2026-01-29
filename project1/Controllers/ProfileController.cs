using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using project1.Data;
using project1.Models;
using Microsoft.EntityFrameworkCore;

namespace project1.Controllers
{
    [Authorize(policy: "User")]
    public class ProfileController : Controller
    {
        private readonly MyDbContext _dbcontext;
        public ProfileController(MyDbContext context)
        {
            _dbcontext = context;
        }

        public IActionResult Index()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _dbcontext.Users.FirstOrDefault(x => x.Email == email);

            return View(user);
        }
        public IActionResult MyOrders()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var orders = _dbcontext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Where(o => o.UserId == userId && o.IsPaid)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return View(orders);
        }
        [HttpPost]
        public IActionResult Update(User model)
        {
            var user = _dbcontext.Users.Find(model.Id);
            user!.Name = model.Name;
            user.Email = model.Email;
            user.Password = model.Password;
            user.Phone = model.Phone;

            _dbcontext.SaveChanges();
            if (user.Role == "Admin")
                return RedirectToAction("Index", "Users");
            if (user.Role == "Librarian")
                return RedirectToAction("Index", "Librarian");
            return RedirectToAction("Index", "Home");
        }
    }
}
