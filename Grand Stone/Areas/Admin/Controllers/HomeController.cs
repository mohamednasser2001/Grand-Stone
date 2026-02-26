using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Grand_Stone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var allProducts = (await _unitOfWork.Products.GetAllAsync())
                .OrderByDescending(p => p.Id)
                .ToList();

            var allCategories = (await _unitOfWork.Categories.GetAllAsync())
                .OrderByDescending(c => c.Id)
                .ToList();

            ViewBag.TotalProducts = allProducts.Count;
            ViewBag.TotalCategories = allCategories.Count;

            var products = allProducts.Take(4).ToList();
            var categories = allCategories.Take(5).ToList();

            return View((products, categories));
        }
    }
}
