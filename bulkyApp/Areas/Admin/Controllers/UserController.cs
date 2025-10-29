using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Model;
using Bulky.Model.Models;
using Bulky.Model.Models.view_models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BulkyApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IuintOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IuintOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            var user = _unitOfWork.applicationUser.Get(u => u.Id == userId, includeProperties: "Company");
            if (user == null)
            {
                return NotFound();
            }

            // Get the user's roles and select the first one.
            var roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
            user.Role = roles.FirstOrDefault();

            RoleManagementVM RoleVM = new()
            {
                applicationUser = user,
                Roles = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            ApplicationUser applicationUser = _unitOfWork.applicationUser.Get(u => u.Id == roleManagementVM.applicationUser.Id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            var oldRole = _userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult().FirstOrDefault();
            var newRole = roleManagementVM.applicationUser.Role;

            if (newRole != oldRole)
            {
                if (newRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.applicationUser.CompanyId;
                }
                else
                {
                    // If the new role is not "Company", ensure the CompanyId is cleared.
                    applicationUser.CompanyId = null;
                }

                // Remove from the old role and add to the new one.
                if (oldRole != null)
                {
                    _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                }
                _userManager.AddToRoleAsync(applicationUser, newRole).GetAwaiter().GetResult();

                _unitOfWork.applicationUser.Update(applicationUser);
                _unitOfWork.Save();
            }

            TempData["success"] = "Role Updated Successfully.";
            return RedirectToAction("Index");
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _unitOfWork.applicationUser.GetAll(includeProperties: "Company").ToList();

            foreach (var user in userList)
            {
                // GetRolesAsync returns a list, we take the first one.
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
            }

            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var userFromDb = _unitOfWork.applicationUser.Get(u => u.Id == id);
            if (userFromDb == null)
            {
                return Json(new { success = false, message = "Error: User not found." });
            }

            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                // User is currently locked, so we will unlock them
                userFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                // User is not locked, so we will lock them
                userFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            }

            _unitOfWork.applicationUser.Update(userFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Operation Successful." });
        }
        #endregion
    }
}
