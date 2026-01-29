using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Data;
using project1.Models;
using project1.Helpers;
using System.Diagnostics;
using System.Security.Claims;

namespace project1.Controllers
{
    [Authorize(policy: "User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        private void ApplyLateFines()
        {
            var today = DateTime.Now.Date;
            var lateBorrows = _context.Borrows
                .Include(b => b.User)
                .Where(b => !b.IsReturned && b.DueDate.Date < today)
                .ToList();
            foreach (var borrow in lateBorrows)
            {
                int daysLate = (today - borrow.DueDate.Date).Days;
                // آیا قبلاً جریمه براش ثبت شده؟
                bool alreadyFineExists = _context.Fines
                    .Any(f => f.BorrowId == borrow.Id && !f.IsPaid);
                if (alreadyFineExists)
                    continue;
                int amount = daysLate * 1000;
                var fine = new Fine
                {
                    UserId = borrow.UserId,
                    BorrowId = borrow.Id,
                    DaysLate = daysLate,
                    Amount = amount,
                    IsPaid = false,
                    CreatedAt = DateTime.Now
                };
                borrow.User.TotalFineAmount += amount;
                // اگر از سقف رد شد، بلاک شود
                if (borrow.User.TotalFineAmount >= 20000)
                    borrow.User.IsBlocked = true;
                _context.Fines.Add(fine);
            }
            _context.SaveChanges();
        }

        // =========================
        // صفحه اصلی
        // =========================
        [AllowAnonymous]
        public IActionResult Index()
        {
            var books = _context.Books
                .Include(b => b.Reserves)
                .Where(b => b.IsActive)
                .ToList();
            return View(books);
        }

        // =========================
        // جستجو (عمومی)
        // =========================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return View(new List<Book>());
            var results = _context.Books
                .Where(b =>
                    b.IsActive &&
                    (b.Name.Contains(q) ||
                     b.Author.Contains(q) ||
                     b.Description!.Contains(q)))
                .ToList();
            return View(results);
        }

        // =========================
        // افزودن به سبد امانت
        // =========================
        [HttpPost]
        public IActionResult AddToBorrow(int bookId)
        {
            ApplyLateFines();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cart = HttpContext.Session.GetObject<List<int>>("BorrowCart")
                       ?? new List<int>();
            if (!cart.Contains(bookId))
                cart.Add(bookId);
            HttpContext.Session.SetObject("BorrowCart", cart);
            return RedirectToAction("Borrows");
        }
        [HttpPost]
        public IActionResult RemoveFromBorrow(int bookId)
        {
            var cart = HttpContext.Session.GetObject<List<int>>("BorrowCart")
                       ?? new List<int>();

            if (cart.Contains(bookId))
            {
                cart.Remove(bookId);
                HttpContext.Session.SetObject("BorrowCart", cart);
            }
            return RedirectToAction("Borrows");
        }

        // =========================
        // صفحه امانت‌ها
        // =========================
        public IActionResult Borrows()
        {
            ApplyLateFines();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cart = HttpContext.Session.GetObject<List<int>>("BorrowCart")
                       ?? new List<int>();
            var selectedBooks = _context.Books
                .Where(b => cart.Contains(b.Id))
                .ToList();
            var activeBorrows = _context.Borrows
                .Include(b => b.Book)
                .Where(b => b.UserId == userId && !b.IsReturned)
                .ToList();
            ViewBag.ActiveBorrows = activeBorrows;

            var user = _context.Users.Find(userId);
            ViewBag.IsBlocked = user?.IsBlocked ?? false;
            ViewBag.TotalFine = user?.TotalFineAmount ?? 0;
            ViewBag.IsPremium = user?.IsPremium ?? false;
            return View(selectedBooks);
        }

        // =========================
        // ثبت نهایی امانت
        // =========================
        [HttpPost]
        public IActionResult ConfirmBorrows()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.Find(userId);
            var cart = HttpContext.Session.GetObject<List<int>>("BorrowCart");
            if (cart == null || !cart.Any())
                return RedirectToAction("Borrows");
            int days = user != null && user.IsPremium ? 30 : 14;
            foreach (var bookId in cart)
            {
                var book = _context.Books.Find(bookId);
                if (book == null || book.AvailableQuantity < 1)
                    continue;
                _context.Borrows.Add(new Borrow
                {
                    UserId = userId,
                    BookId = bookId,
                    BorrowDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(days),
                    RenewedCount = 0,
                    IsReturned = false
                });
                book.AvailableQuantity--;

                // فعال‌سازی نوبت رزرو بعدی
                var nextReserve = _context.Reserves
                    .Where(r => r.BookId == book.Id && r.IsActive)
                    .OrderBy(r => r.ReservationDate)
                    .FirstOrDefault();
                if (nextReserve != null)
                    nextReserve.ExpireDate = DateTime.Now.AddDays(5);
            }
            _context.SaveChanges();
            HttpContext.Session.Remove("BorrowCart");
            return RedirectToAction("Borrows");
        }

        // =========================
        // تمدید امانت
        // =========================
        public IActionResult RenewBorrow(int id)
        {
            var borrow = _context.Borrows
                .FirstOrDefault(b => b.Id == id && !b.IsReturned);
            if (borrow == null || borrow.RenewedCount >= 2)
                return RedirectToAction("Borrows");
            borrow.DueDate = borrow.DueDate.AddDays(7);
            borrow.RenewedCount++;
            _context.SaveChanges();
            return RedirectToAction("Borrows");
        }

        // =========================
        // افزودن به رزرو
        // =========================
        [HttpPost]
        public IActionResult AddToReserve(int bookId)
        {
            ApplyLateFines();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            //var user = _context.Users.Find(userId);
            //if (user!.IsBlocked)
            //    return BadRequest("به دلیل بدهی، امکان رزرو ندارید");

            var book = _context.Books
                .Include(b => b.Reserves)
                .FirstOrDefault(b => b.Id == bookId && b.Type == BookType.Borrow);
            if (book == null)
                return NotFound();
            if (book.Reserves!.Count(r => r.IsActive) >= 2)
                return BadRequest("ظرفیت رزرو تکمیل شده");
            bool alreadyReserved = _context.Reserves
                .Any(r => r.BookId == bookId && r.UserId == userId && r.IsActive);
            if (alreadyReserved)
                return RedirectToAction("Reserves");
            _context.Reserves.Add(new Reserve
            {
                UserId = userId,
                BookId = bookId,
                ReservationDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddDays(5),
                IsActive = true
            });
            _context.SaveChanges();
            return RedirectToAction("Reserves");
        }

        // =========================
        // صفحه رزروها
        // =========================
        public IActionResult Reserves()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            AutoCancelExpiredReserves();
            var reserves = _context.Reserves
                .Include(r => r.Book)
                .Where(r => r.UserId == userId && r.IsActive)
                .OrderBy(r => r.ReservationDate)
                .ToList();
            var user = _context.Users.Find(userId);
            ViewBag.IsBlocked = user?.IsBlocked ?? false;
            ViewBag.TotalFine = user?.TotalFineAmount ?? 0;

            return View(reserves);
        }

        public IActionResult CancelReserve(int id)
        {
            var reserve = _context.Reserves.Find(id);
            if (reserve == null)
                return NotFound();
            reserve.IsActive = false;
            _context.SaveChanges();
            return RedirectToAction("Reserves");
        }

        private void AutoCancelExpiredReserves()
        {
            var expired = _context.Reserves
                .Where(r => r.IsActive && r.ExpireDate < DateTime.Now)
                .ToList();
            foreach (var r in expired)
                r.IsActive = false;
            _context.SaveChanges();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}

