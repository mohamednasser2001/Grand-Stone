using DataAccess.UnitOfWork;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Grand_Stone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Products.GetAllAsync(include: q => q.Include(p => p.Category));
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl ?? Request.Headers["Referer"].ToString();

            var categories = await _unitOfWork.Categories.GetAllAsync();
            ViewBag.Categories = categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            return View(new Product { IsAvailable = true });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, int categoryId, IFormFile imageFile, string returnUrl)
        {
            product.CategoryId = categoryId;

            if (product.CategoryId <= 0)
                ModelState.AddModelError("CategoryId", "Please select a category.");

            if (imageFile == null || imageFile.Length == 0)
                ModelState.AddModelError("ImageUrl", "Please upload an image.");

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;

                var categories = await _unitOfWork.Categories.GetAllAsync();
                ViewBag.Categories = categories
                    .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(product);
            }

            // حفظ الصورة
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
            Directory.CreateDirectory(uploadsFolder);

            var fileExt = Path.GetExtension(imageFile.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExt}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            product.ImageUrl = $"/images/products/{fileName}";

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            TempData["ToastMessage"] = "Product added successfully.";
            TempData["ToastType"] = "success";
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }
    }
}
