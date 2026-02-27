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

        public async Task<IActionResult> Index(string q)
        {
            var allProducts = (await _unitOfWork.Products.GetAllAsync())
                .OrderByDescending(p => p.Id)
                .ToList();

            var allCategories = (await _unitOfWork.Categories.GetAllAsync())
                .OrderByDescending(c => c.Id)
                .ToList();

            // Search
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();

                allProducts = allProducts
                    .Where(p =>
                        (p.Name != null && p.Name.ToLower().Contains(q)) ||
                        (p.Description != null && p.Description.ToLower().Contains(q))
                    )
                    .OrderByDescending(p => p.Id)
                    .ToList();

                allCategories = allCategories
                    .Where(c => c.Name != null && c.Name.ToLower().Contains(q))
                    .OrderByDescending(c => c.Id)
                    .ToList();
            }

            ViewBag.Q = q;

            // Counts (بعد الفلترة لو فيه q)
            ViewBag.TotalProducts = allProducts.Count;
            ViewBag.TotalCategories = allCategories.Count;

            // Dashboard preview
            var products = allProducts.Take(4).ToList();
            var categories = allCategories.Take(5).ToList();

            return View((products, categories));
        }
    }
}
