using System.Diagnostics;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Model.Models;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWep.Areas.Customer2.Controllers
{
    [Area("Customer2")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IuintOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IuintOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList= _unitOfWork.Product.GetAll(includeProperties:"category");
            return View(productList);
        }
        public IActionResult Details(int productID)
        {
            Product product = _unitOfWork.Product.Get(u=>u.Id== productID, includeProperties: "category");
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
