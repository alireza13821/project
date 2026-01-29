using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Data;
using project1.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace project1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyDBContext _dbContext;

        public HomeController(ILogger<HomeController> logger, MyDBContext Context)
        {
            _logger = logger;
            _dbContext = Context;
        }

        public IActionResult Index()
        {
            var allBooks = _dbContext.Books.ToList();
            return View(allBooks);
        }

        [Authorize(Roles = "User")]
        public IActionResult AddToReserve(int bookId, bool remove = false)
        {
            var book = _dbContext.Books.Find(bookId);
            if (book == null || book.Quantity <= 0)  
            {
                return RedirectToAction("Index");
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());

            var barrow = _dbContext.Reserves
                .Include(r => r.ReserveItems)
                .SingleOrDefault(o => o.UserId == userId && !o.IsFinaly);

            if (remove)
            {
                if (barrow != null)
                {
                    var itemToRemove = barrow.ReserveItems.SingleOrDefault(ri => ri.BookId == bookId);
                    if (itemToRemove != null)
                    {
                        _dbContext.ReserveItems.Remove(itemToRemove);
                        _dbContext.SaveChanges();
                    }
                }
            }
            else
            {
                if (barrow != null)
                {
                    var barrowDetail = barrow.ReserveItems.SingleOrDefault(d => d.BookId == bookId);
                    if (barrowDetail != null)
                    {
                        return RedirectToAction("MyBarrow");
                    }
                    else
                    {
                        var newReserveItem = new ReserveItem
                        {
                            BookId = bookId,
                            ReserveId = barrow.Id
                        };
                        _dbContext.ReserveItems.Add(newReserveItem);
                    }
                }
                else
                {
                    var newReserve = new Reserve
                    {
                        UserId = userId,
                        CreateDate = DateTime.Now,
                        IsFinaly = false,
                        Status="ثبت نشده",
                        ReserveItems = new List<ReserveItem>
                        {
                            new ReserveItem
                            {
                                BookId = bookId
                            }
                        }
                    };

                    _dbContext.Reserves.Add(newReserve);
                }
            }

            _dbContext.SaveChanges();
            return RedirectToAction("MyBarrow");
        }


        [Authorize(Roles = "User")]
        public IActionResult MyBarrow()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());


            var reserve = _dbContext.Reserves
                .Where(o => o.UserId == userId && !o.IsFinaly)
                .Include(r => r.ReserveItems)
                .ThenInclude(ri => ri.book)
                .SingleOrDefault();

            if (reserve == null)
            {
                return View(new Reserve { ReserveItems = new List<ReserveItem>() });
            }

            return View(reserve);
        }


        [Authorize(Roles = "User")]
        public IActionResult FinalizeReserve()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());


            var reserve = _dbContext.Reserves
                .Where(o => o.UserId == userId && !o.IsFinaly && !o.IsApproved)
                .Include(r => r.ReserveItems)
                .SingleOrDefault();

            if (reserve != null)
            {

                reserve.IsFinaly = true;
                reserve.IsApproved = false;
                reserve.Status = "در حال بررسی";
                _dbContext.SaveChanges();
            }


            return RedirectToAction("ApprovalMessage");
        }

        [Authorize(Roles = "User")]



        [Authorize(Roles = "User")]
        public IActionResult ApprovalMessage()
        {
            ViewBag.Message = "درخواست شما با موفقیت ارسال شد و در انتظار تأیید است";
            return View();
        }
        
        [Authorize(Roles = "User")]
        public IActionResult PreviousReserves()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());


            var previousReserves = _dbContext.Reserves
                .Include(r => r.ReserveItems)
                .ThenInclude(ri => ri.book)
                .Where(r => r.UserId == userId && r.IsFinaly)
                .ToList();

            return View(previousReserves);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}