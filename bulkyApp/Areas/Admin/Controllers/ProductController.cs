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
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
                productvm.Product = _unitOfWork.Product.Get(u => u.Id == Id,includeProperties: "ProductImages");
                return View (productvm);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files)
        {
            // First, check if the model state is valid. All logic should be inside this block.
            if (ModelState.IsValid)
            {
                // Check if this is a new product or an update
                if (productVM.Product.Id == 0)
                {
                    // This is a new product, so add it to the database first.
                    // This is necessary to get a Product.Id for creating the image folder path.
                    _unitOfWork.Product.Add(productVM.Product);
                    _unitOfWork.Save();
                }
                else
                {
                    // This is an update, so update the main product entity.
                    _unitOfWork.Product.Update(productVM.Product);
                }

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    // The path for the product's image folder
                    string productPath = @"images\products\product-" + productVM.Product.Id;
                    string finalProductPath = Path.Combine(wwwRootPath, productPath);

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(finalProductPath))
                    {
                        Directory.CreateDirectory(finalProductPath);
                    }

                    // Loop through each uploaded file
                    foreach (IFormFile file in files)
                    {
                        // Create a unique file name for each image
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(finalProductPath, fileName);

                        // Save the physical file to the server
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        // Create a new ProductImage object
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        // CRITICAL FIX: Explicitly add the new ProductImage record to the database context.
                        _unitOfWork.ProductImages.Add(productImage);
                    }
                }

                // Save all changes (product update and new images) to the database.
                _unitOfWork.Save();

                TempData["success"] = "Product created/updated successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                // If the model state is not valid, re-populate the CategoryList
                // and return the view with the validation errors.
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
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

            string productPath = @"images\products\product-" + id;
            string finalProductPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
            _unitOfWork.Product.Remove(projectToBedDeleted);

            if (!Directory.Exists(finalProductPath))
            {
                string[] filePaths=Directory.GetFiles(finalProductPath);
                foreach (string filepath in filePaths)
                {
                    System.IO.File.Delete(filepath);
                }
                Directory.Delete(finalProductPath);
            }
            _unitOfWork.Save();
            return Json(new { success = true, messsge = "Error Successfully" });
        }

        public IActionResult DeleteImage(int imageId)
        {
            // Find the specific image record in the database using its ID.
            var imageToBeDeleted = _unitOfWork.ProductImages.Get(u => u.Id == imageId);

            // Store the productId before deleting the image, so we can redirect back to the correct product page.
            int productId = imageToBeDeleted.ProductId;

            if (imageToBeDeleted != null)
            {
                // Check if the ImageUrl is not null or empty to prevent errors.
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    // Construct the absolute path to the physical image file on the server.
                    // TrimStart('\\') removes any leading backslashes from the stored URL to prevent path errors.
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                                                    imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    // Check if the physical file actually exists on the server before trying to delete it.
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // After deleting the physical file, remove the image record from the database.
                _unitOfWork.ProductImages.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully";
            }

            // Redirect the user back to the Upsert page for the same product they were editing.
            return RedirectToAction(nameof(Upsert), new { id = productId });
        }
        #endregion
    }


}
