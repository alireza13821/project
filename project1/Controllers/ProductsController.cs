using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace project1.Controllers
{
    [Authorize(policy:"User")]
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
