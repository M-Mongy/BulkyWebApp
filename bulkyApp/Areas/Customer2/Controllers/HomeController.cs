using System.Diagnostics;
using System.Security.Claims;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Model.Models;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
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
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productID, includeProperties: "category"),
                count = 1,
                ProductId = productID
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserID=userId;
            ShoppingCart CartFromDB = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserID == userId && u.ProductId == cart.ProductId);
            
            if (CartFromDB != null)
            {
                CartFromDB.count += cart.count;
                _unitOfWork.ShoppingCart.Update(CartFromDB);

            }
            else
            {
                _unitOfWork.ShoppingCart.Add(cart);
            }
            TempData["success"] = "cart updated successfully";
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
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
