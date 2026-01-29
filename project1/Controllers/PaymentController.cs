using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Data;
using project1.Models;
using System.Security.Claims;

namespace project1.Controllers
{
    [Authorize(policy: "User")]
    public class PaymentController : Controller
    {
        private readonly MyDbContext _context;

        public PaymentController(MyDbContext context)
        {
            _context = context;
        }

        // =========================
        // صفحه پرداخت
        // =========================
        public IActionResult Pay(string type, int? id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var vm = new PaymentViewModel
            {
                PaymentType = type,
                RefId = id,
                Receiver = "کتابخانه آبی",
                SecurityCode = new Random().Next(10000, 99999).ToString()
            };

            if (type == "Fine")
            {
                // ✅ مبلغ دقیق جریمه
                vm.Title = "پرداخت بدهی";
                vm.Amount = _context.Fines
                    .Where(f => f.UserId == userId && !f.IsPaid)
                    .Sum(f => f.Amount);
            }
            else if (type == "Order")
            {
                var order = _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.Id == id);

                vm.Title = "پرداخت خرید";
                vm.Amount = (int)order!.OrderItems
                    .Sum(i => i.Quantity * i.Price);
            }
            else if (type == "Subscription")
            {
                vm.Title = "پرداخت اشتراک ویژه";
                vm.Amount = 800000;
            }

            return View(vm);
        }

        // =========================
        // ثبت پرداخت
        // =========================
        [HttpPost]
        public IActionResult Pay(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.Find(userId)!;

            if (model.PaymentType == "Fine")
            {
                var fines = _context.Fines
                    .Where(f => f.UserId == userId && !f.IsPaid)
                    .ToList();

                foreach (var fine in fines)
                    fine.IsPaid = true;

                user.TotalFineAmount = 0;
                user.IsBlocked = false;
            }
            else if (model.PaymentType == "Order")
            {
                var order = _context.Orders.Find(model.RefId);
                order!.IsPaid = true;
                order.OrderDate = DateTime.Now;
            }
            else if (model.PaymentType == "Subscription")
            {
                var oldSubs = _context.Subscriptions
                    .Where(s => s.UserId == userId)
                    .ToList();

                foreach (var s in oldSubs)
                    s.IsActive = false;

                _context.Subscriptions.Add(new Subscription
                {
                    UserId = userId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(3),
                    Price = 800000,
                    IsActive = true
                });

                user.IsPremium = true;
            }

            _context.SaveChanges();
            return RedirectToAction("Success");
        }

        // =========================
        // صفحه موفقیت
        // =========================
        public IActionResult Success()
        {
            return View();
        }
    }
}
