using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Data;
using project1.Models;

namespace project1.Controllers
{
    [Authorize(policy: "Admin")]
    public class AdminController : Controller
    {
        private readonly MyDbContext _dbcontext;

        public AdminController(MyDbContext context)
        {
            _dbcontext = context;
        }

        // 📚 صفحه مدیریت کتاب‌ها
        public IActionResult Index()
        {
            var books = _dbcontext.Books.ToList();
            return View(books);
        }

        // 💰 گزارش‌گیری مالی
        public IActionResult FinancialReport()
        {
            var bookOrders = _dbcontext.Orders
                .Where(o => o.IsPaid)
                .Include(o => o.User)
                .ToList();
            var subscriptions = _dbcontext.Subscriptions
                .Include(s => s.User)
                .ToList();
            var paidFines = _dbcontext.Fines
                .Where(f => f.IsPaid)
                .Include(f => f.User)
                .ToList();
            var unpaidFines = _dbcontext.Fines
                .Where(f => !f.IsPaid)
                .Include(f => f.User)
                .ToList();
            var viewModel = new FinancialReportViewModel
            {
                BookOrders = bookOrders,
                Subscriptions = subscriptions,
                PaidFines = paidFines,
                UnpaidFines = unpaidFines,

                TotalBookSales = bookOrders.Sum(o => o.TotalPrice),
                TotalSubscriptionSales = subscriptions.Sum(s => s.Price),
                TotalPaidFines = paidFines.Sum(f => f.Amount),
                TotalUnpaidFines = unpaidFines.Sum(f => f.Amount)
            };
            return View(viewModel);
        }

        // 🔍 سرچ کتاب‌ها (ادمین)
        [HttpGet]
        public IActionResult SearchBooks(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return View(new List<Book>());
            q = q.Trim();
            var books = _dbcontext.Books
                .Where(b =>
                    b.Name.Contains(q) ||
                    b.Author.Contains(q))
                .ToList();
            return View(books);
        }

        // 🔍 سرچ کاربران (GET – مخصوص Enter و URL)
        [AllowAnonymous]   
        public IActionResult SearchUser(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return View(new List<User>());
            search = search.Trim();
            var users = _dbcontext.Users
                .Where(u => (u.Role == "User" || u.Role == "Librarian")&&
                    (u.Name.Contains(search) ||
                     u.Email.Contains(search)))
                .ToList();
            return View(users);
        }

        // ➕ افزودن کتاب
        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBook(AddBookViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var book = new Book
            {
                Name = model.Name!,
                Description = model.Description,
                TotalQuantity = model.Quantity,
                AvailableQuantity = model.Quantity,
                Author = model.Author,
                PublishedYear = model.PublishedYear,
                Type = model.Type,
                Price = model.Price,
                IsActive = true
            };
            _dbcontext.Books.Add(book);
            _dbcontext.SaveChanges();
            if (model.Picture?.Length > 0)
            {
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "img",
                    book.Id + Path.GetExtension(model.Picture.FileName));

                using var stream = new FileStream(filePath, FileMode.Create);
                model.Picture.CopyTo(stream);
            }
            return RedirectToAction("Index");
        }

        // ✏️ ویرایش کتاب
        public IActionResult EditBook(int id)
        {
            var book = _dbcontext.Books.Find(id);
            if (book == null) return NotFound();

            var model = new AddBookViewModel
            {
                Id = book.Id,
                Name = book.Name,
                Description = book.Description,
                Author = book.Author,
                Type = book.Type,
                Quantity = book.TotalQuantity,
                PublishedYear = book.PublishedYear,
                Price = book.Price
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditBook(AddBookViewModel model)
        {
            var book = _dbcontext.Books.Find(model.Id);
            if (book == null) return NotFound();

            book.Name = model.Name!;
            book.Description = model.Description;
            book.Author = model.Author;
            book.Type = model.Type;
            book.TotalQuantity = model.Quantity;
            book.PublishedYear = model.PublishedYear;
            book.Price = model.Price;
            if (book.AvailableQuantity > model.Quantity)
                book.AvailableQuantity = model.Quantity;
            else
                book.AvailableQuantity = book.TotalQuantity;
            _dbcontext.SaveChanges();

            if (model.Picture?.Length > 0)
            {
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "img",
                    model.Id + Path.GetExtension(model.Picture.FileName));
                using var stream = new FileStream(filePath, FileMode.Create);
                model.Picture.CopyTo(stream);
            }
            return RedirectToAction("Index");
        }
    }
}
