using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Grand_Stone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
      
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

      
    }
}
