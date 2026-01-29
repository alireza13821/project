using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using project1.Data;
using project1.Models;
using Microsoft.EntityFrameworkCore;

namespace project1.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly MyDbContext _dbcontext;

        public CartController(MyDbContext context)
        {
            _dbcontext = context;
        }

        // =========================
        // نمایش سبد خرید
        // =========================
        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var order = _dbcontext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefault(o => o.UserId == userId && !o.IsPaid);

            return View(order);
        }

        // =========================
        // افزودن کتاب به سبد خرید
        // =========================
        public IActionResult AddToCart(int bookId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var user = _dbcontext.Users.First(u => u.Id == userId);

            var book = _dbcontext.Books
                .FirstOrDefault(b => b.Id == bookId && b.Type == BookType.Sale && b.IsActive);

            if (book == null)
                return NotFound();

            var order = _dbcontext.Orders
                .FirstOrDefault(o => o.UserId == userId && !o.IsPaid);

            if (order == null)
            {
                order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    IsPaid = false
                };
                _dbcontext.Orders.Add(order);
                _dbcontext.SaveChanges();
            }

            var item = _dbcontext.OrderItems
                .FirstOrDefault(i => i.OrderId == order.Id && i.BookId == bookId);

            if (book.AvailableQuantity < 1)
                return BadRequest("موجودی کافی نیست");

            // 💰 قیمت با تخفیف اگر پریمیوم بود
            float finalPrice = book.Price;
            if (user.IsPremium)
                finalPrice *= 0.8f; // 20٪ تخفیف

            if (item == null)
            {
                item = new OrderItem
                {
                    OrderId = order.Id,
                    BookId = bookId,
                    Quantity = 1,
                    Price = finalPrice
                };
                _dbcontext.OrderItems.Add(item);
            }
            else
            {
                if (item.Quantity + 1 > book.AvailableQuantity)
                    return BadRequest("بیشتر از موجودی نمی‌توانید خرید کنید");

                item.Quantity++;
            }

            // 🔥 محاسبه صحیح جمع کل
            order.TotalPrice = _dbcontext.OrderItems
                .Where(i => i.OrderId == order.Id)
                .Sum(i => i.Quantity * i.Price);

            _dbcontext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult RemoveItem(int itemId)
        {
            var item = _dbcontext.OrderItems
                .Include(i => i.Order)
                .FirstOrDefault(i => i.Id == itemId);

            if (item == null)
                return NotFound();

            var order = item.Order;

            _dbcontext.OrderItems.Remove(item);
            _dbcontext.SaveChanges();

            order.TotalPrice = _dbcontext.OrderItems
                .Where(i => i.OrderId == order.Id)
                .Sum(i => i.Quantity * i.Price);

            _dbcontext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var order = _dbcontext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefault(o => o.UserId == userId && !o.IsPaid);

            if (order == null || !order.OrderItems.Any())
                return BadRequest("سبد خرید خالی است");

            foreach (var item in order.OrderItems)
            {
                if (item.Book.AvailableQuantity < item.Quantity)
                    return BadRequest("موجودی تغییر کرده");

                item.Book.AvailableQuantity -= item.Quantity;
            }

            order.TotalPrice = order.OrderItems
                .Sum(i => i.Quantity * i.Price);

            order.IsPaid = true;
            order.OrderDate = DateTime.Now;

            _dbcontext.SaveChanges();

            return RedirectToAction("MyOrders", "Profile");
        }
    }
}
