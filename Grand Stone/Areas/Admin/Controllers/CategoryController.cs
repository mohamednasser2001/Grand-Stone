using System.Threading.Tasks;
using DataAccess.UnitOfWork;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Grand_Stone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> Index(string q)
        {
            var categories = (await _unitOfWork.Categories.GetAllAsync())
                .OrderByDescending(c => c.Id)
                .ToList();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();
                categories = categories
                    .Where(c => c.Name != null && c.Name.ToLower().Contains(q))
                    .OrderByDescending(c => c.Id)
                    .ToList();
            }

            ViewBag.Q = q;
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ReturnUrl = Request.Headers["Referer"].ToString();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl; 
                return View(category);
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();
            TempData["ToastMessage"] = "Category added successfully.";
            TempData["ToastType"] = "success";
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);
       
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Categories.GetAsync(c => c.Id == id);
            if (category == null) return NotFound();

            ViewBag.ReturnUrl = Request.Headers["Referer"].ToString();
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(category);
            }

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();
            TempData["ToastMessage"] = "Category updated successfully.";
            TempData["ToastType"] = "success";
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _unitOfWork.Categories.GetAsync(c => c.Id == id);
            if (category == null) return NotFound();

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.CompleteAsync();
            TempData["ToastMessage"] = "Category deleted successfully.";
            TempData["ToastType"] = "warning";
            var returnUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

          
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}
