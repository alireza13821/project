using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project1.Data;
using project1.Models;

namespace project1.Controllers
{
    [Authorize(policy:"Admin")]
    public class BookController : Controller
    {
        private MyDbContext _dbcontext;
        public BookController(MyDbContext context)
        {
            _dbcontext = context;
        }

        //  سرچ کتاب (برای همه)
        [AllowAnonymous]
        public IActionResult Index(string search)
        {
            var books = _dbcontext.Books.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                books = books.Where(b => b.Name!.Contains(search));

            return View(books.ToList());
        }

        //  افزودن کتاب (فقط ادمین)
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(AddBookViewModel newbook)
        {
            if (!ModelState.IsValid)
                return View(newbook);
            Book book = new Book();
            book.Name = newbook.Name!;
            book.Description= newbook.Description!;
            book.Author = newbook.Author!;
            book.PublishedYear=newbook.PublishedYear!;
            book.TotalQuantity = newbook.Quantity;
            book.AvailableQuantity= newbook.Quantity;
            book.Price = newbook.Price!;
            book.IsActive = true;
            //if(newbook.Type.ToString()=="Sale")
            //{
            //    book.Type = BookType.Sale;
            //}
            //book.Type = BookType.Borrow;
            book.Type=newbook.Type;

            _dbcontext.Books.Add(book);
            _dbcontext.SaveChanges();
            return RedirectToAction("Index");
        }

        ////  ویرایش کتاب (ادمین)
        //public IActionResult Edit(int id)
        //{
        //    var book = _dbcontext.Books.Find(id);
        //    return View(book);
        //}
        //[HttpPost]
        //public IActionResult Edit(Book book)
        //{
        //    _dbcontext.Books.Update(book);
        //    _dbcontext.SaveChanges();
        //    return RedirectToAction("Index");
        //}
    }
}
