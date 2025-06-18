using Bulky.Data.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.DataAccess.Repository;
using Bulky.Model.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Model.Models.view_models;
using Microsoft.AspNetCore.Hosting;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
namespace BulkyWep.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IuintOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IuintOfWork unitOfwork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfwork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> ObjectProductsList = _unitOfWork.Product.GetAll(includeProperties: "category").ToList();
            return View(ObjectProductsList);
        }
        public IActionResult Upsert(int? Id)
        {
            ProductVM productvm = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (Id == null || Id==0){
                //create 
                return View(productvm);
            }
            else
            {
                //update
                productvm.Product = _unitOfWork.Product.Get(u => u.Id == Id);
                return View (productvm);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productvm,IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                productvm.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productvm); // Return view with current model to show errors
            }

            string wwwRootpath = _webHostEnvironment.WebRootPath;

            if (file != null) // Check if a new file was uploaded
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productDirectoryPath = Path.Combine(wwwRootpath, @"Images\Product");

                // Ensure the directory exists
                if (!Directory.Exists(productDirectoryPath))
                {
                    Directory.CreateDirectory(productDirectoryPath);
                }

                // Delete old image if it exists and this is an update operation
                // and there was an existing image path
                if (productvm.Product.Id != 0 && !string.IsNullOrEmpty(productvm.Product.imageUrl))
                {
                    var oldImagePath = Path.Combine(wwwRootpath, productvm.Product.imageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save the new image
                string filePath = Path.Combine(productDirectoryPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                productvm.Product.imageUrl = @"\Images\Product\" + fileName; // Update imageUrl to new file path
            }
            // If 'file' is null and it's an update, productvm.Product.imageUrl will retain
            // its existing value from the hidden input in the form. No action needed here.

            if (productvm.Product.Id == 0) // Create scenario
            {
                _unitOfWork.Product.Add(productvm.Product);
                TempData["success"] = "Product created successfully!";
            }
            else // Update scenario
            {
                _unitOfWork.Product.Update(productvm.Product);
                TempData["success"] = "Product updated successfully!";
            }

            // Save all changes to the database
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() {
            List<Product> ObjectProductsList = _unitOfWork.Product.GetAll(includeProperties: "category").ToList();
            return Json(new { data = ObjectProductsList });
        }
        [HttpDelete]
        public IActionResult Delete(int?id)
        {
            var projectToBedDeleted = _unitOfWork.Product.Get(c => c.Id == id);
            if (projectToBedDeleted== null) {
                return Json(new { success=false,messsge="Error while deleting"});
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, projectToBedDeleted.imageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(projectToBedDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, messsge = "Error Successfully" });
        }
        #endregion
    }


}
