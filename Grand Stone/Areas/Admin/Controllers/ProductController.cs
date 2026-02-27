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

        public async Task<IActionResult> Index(string q, int page = 1, int pageSize = 6)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 6;

            var all = await _unitOfWork.Products.GetAllAsync(include: x => x.Include(p => p.Category));

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();
                all = all.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(q)) ||
                    (p.Description != null && p.Description.ToLower().Contains(q)) ||
                    (p.Category != null && p.Category.Name != null && p.Category.Name.ToLower().Contains(q))
                );
            }

            var totalCount = all.Count();

            var items = all
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Q = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(items);
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

        // GET: /Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int id, string returnUrl = null)
        {
            var product = await _unitOfWork.Products.GetAsync(p => p.Id == id);
            if (product == null) return NotFound();

            ViewBag.ReturnUrl = returnUrl ?? Request.Headers["Referer"].ToString();

            var categories = await _unitOfWork.Categories.GetAllAsync();
            ViewBag.Categories = categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            return View(product);
        }

        // POST: /Admin/Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, int categoryId, IFormFile? imageFile, string returnUrl)
        {
            product.CategoryId = categoryId;

            if (product.CategoryId <= 0)
                ModelState.AddModelError("CategoryId", "Please select a category.");

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;

                var categories = await _unitOfWork.Categories.GetAllAsync();
                ViewBag.Categories = categories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();

                return View(product);
            }

            // لو المستخدم رفع صورة جديدة، نحفظها ونحدث ImageUrl
            if (imageFile != null && imageFile.Length > 0)
            {
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
            }
            else
            {
              
                var old = await _unitOfWork.Products.GetAsync(p => p.Id == product.Id);
                if (old != null)
                    product.ImageUrl = old.ImageUrl;
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            TempData["ToastMessage"] = "Product updated successfully.";
            TempData["ToastType"] = "success";

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string returnUrl = null)
        {
            var product = await _unitOfWork.Products.GetAsync(p => p.Id == id);
            if (product == null) return NotFound();

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.CompleteAsync();

            TempData["ToastMessage"] = "Product deleted successfully.";
            TempData["ToastType"] = "warning";

            // يرجع لنفس الصفحة اللي جي منها
            var url = returnUrl ?? Request.Headers["Referer"].ToString();
            if (!string.IsNullOrWhiteSpace(url))
                return Redirect(url);

            return RedirectToAction("Index", "Product", new { area = "Admin" });
        }
    }


}
