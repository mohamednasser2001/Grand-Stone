using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Grand_Stone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _unitOfWork.Products.GetAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
