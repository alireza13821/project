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

    [Authorize(policy: "librarian")]
    public class librarianController : Controller
    {
        private MyDbContext _dbcontext;
        public librarianController(MyDbContext context)
        {
            _dbcontext = context;
        }

        //public IActionResult Index()
        //{

        //    var reserveViewModels = _dbcontext.Reserves
        //        .Where(r => r.IsFinaly && !r.IsApproved && r.Status == "در حال بررسی")
        //        .Include(r => r.User)
        //        .Include(r => r.ReserveItems)
        //        .ThenInclude(ri => ri.book)
        //        .Select(r => new ReserveViewModel
        //        {
        //            Id = r.Id,
        //            CreateDate = r.CreateDate,
        //            FullName = r.User != null ? $"{r.User.Name}" : "Unknown",
        //            ReserveItems = r.ReserveItems,
        //            Status = r.Status
        //        })
        //        .ToList();

        //    return View(reserveViewModels);
        //}

        //[HttpPost]
        //public IActionResult ApproveRequest(int reserveId)
        //{
        //    var reserve = _dbcontext.Reserves
        //        .Include(r => r.ReserveItems)
        //        .ThenInclude(ri => ri.book)
        //        .SingleOrDefault(r => r.Id == reserveId && !r.IsApproved && r.IsFinaly);

        //    if (reserve != null)
        //    {

        //        foreach (var reserveItem in reserve.ReserveItems)
        //        {
        //            var book = reserveItem.book;
        //            if (book.Quantity > 0)
        //            {
        //                book.Quantity--;  
        //            }
        //            else
        //            {
        //                return Content("موجودی یکی از کتاب‌ها کافی نیست.");
        //            }
        //        }

        //        reserve.IsApproved = true;
        //        reserve.Status = "تایید شده"; 
        //        reserve.IsRejected = false;
        //        _dbcontext.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public IActionResult RejectRequest(int reserveId, string reason)
        //{
        //    var reserve = _dbcontext.Reserves
        //        .SingleOrDefault(r => r.Id == reserveId && !r.IsApproved);

        //    if (reserve != null)
        //    {
        //        reserve.Status = "رد شده";  
        //        reserve.IsFinaly = true;
        //        reserve.IsApproved = false;
        //        reserve.IsRejected = true;
        //        reserve.RejectionReason = reason; 
        //        _dbcontext.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}
    }
}
