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
    public class CompanyController : Controller
    {
        private readonly IuintOfWork _unitOfWork;
        public CompanyController(IuintOfWork unitOfwork)
        {
            _unitOfWork = unitOfwork;
        }

        public IActionResult Index()
        {
            List<Company> ObjectCompanysList = _unitOfWork.Company.GetAll().ToList();
            return View(ObjectCompanysList);
        }
        public IActionResult Upsert(int? Id)
        {
            if (Id == null || Id==0){
                //create 
                return View(new Company());
            }
            else
            {
                //update
                Company CompanyObj = _unitOfWork.Company.Get(u => u.Id == Id);
                return View (CompanyObj);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {
                if (CompanyObj.Id == 0) // Create scenario
                {
                    _unitOfWork.Company.Add(CompanyObj);
                    TempData["success"] = "Company created successfully!";
                }
                else // Update scenario
                {
                    _unitOfWork.Company.Update(CompanyObj);
                    TempData["success"] = "Company updated successfully!";
                }
                _unitOfWork.Save();
                TempData["success"] = "company Created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(CompanyObj);

            }

          
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() {
            List<Company> ObjectCompanysList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = ObjectCompanysList });
        }
        [HttpDelete]
        public IActionResult Delete(int?id)
        {
            var projectToBedDeleted = _unitOfWork.Company.Get(c => c.Id == id);
            if (projectToBedDeleted== null) {
                return Json(new { success=false,messsge="Error while deleting"});
            }
            _unitOfWork.Company.Remove(projectToBedDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, messsge = "Error Successfully" });
        }
        #endregion
    }


}
