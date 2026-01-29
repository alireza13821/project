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
    [Authorize(policy: "User")]
    public class UsersController : Controller
    {
        private MyDbContext _dbcontext;
        public UsersController(MyDbContext context)
        {
            _dbcontext = context;
        }

        [Authorize(policy: "Admin")]
        public IActionResult Index()
        {
            return View(_dbcontext.Users.ToList());
        }

        [Authorize(policy: "Admin")]
        public IActionResult Edit(int id)
        {
            return View(_dbcontext.Users.Find(id));
        }
        [Authorize(policy: "Admin")]
        [HttpPost]
        public IActionResult Edit(User model)
        {
            var user = _dbcontext.Users.Find(model.Id);
            user!.Name = model.Name;
            user.Role = model.Role;

            _dbcontext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Subscription()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var activeSubscription = _dbcontext.Subscriptions
                .FirstOrDefault(s => s.UserId == userId && s.IsActive);

            SubscriptionViewModel vm = new SubscriptionViewModel();

            if (activeSubscription != null)
            {
                vm.HasActiveSubscription = true;
                vm.ExpireDate = activeSubscription.EndDate;
                vm.RemainingDays = (activeSubscription.EndDate - DateTime.Now).Days;
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult BuySubscription()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = _dbcontext.Users.Find(userId);

            if (user == null)
                return RedirectToAction("Subscription");

            var oldSubscriptions = _dbcontext.Subscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToList();

            foreach (var sub in oldSubscriptions)
                sub.IsActive = false;

            var newSubscription = new Subscription
            {
                UserId = userId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3),
                Price = 800000,
                IsActive = true
            };

            user.IsPremium = true;

            _dbcontext.Subscriptions.Add(newSubscription);
            _dbcontext.SaveChanges();

            TempData["SuccessMessage"] = "????? ????? ?????? ??? ?? ?????? ???? ??!";

            return RedirectToAction("Subscription");
        }
    }
}