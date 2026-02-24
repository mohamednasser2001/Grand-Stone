using Microsoft.AspNetCore.Mvc;

namespace Grand_Stone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            if (id == 1)
            {
                ViewBag.Title = "NEGRO ZIMBABWE";
                ViewBag.Description = "A compact granite with a medium-fine grain that stands out for its brilliance and luminosity.";
                ViewBag.Image = "/images/products/negro-zimbabwe.jpg";
            }
            else if (id == 2)
            {
                ViewBag.Title = "PORTOBELLO";
                ViewBag.Description = "Elegant marble with soft veins and premium finish.";
                ViewBag.Image = "/images/products/portobello.jpg";
            }

            return View();
        }
    }
}
