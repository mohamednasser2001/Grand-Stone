using Microsoft.AspNetCore.Mvc;

namespace Grand_Stone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
