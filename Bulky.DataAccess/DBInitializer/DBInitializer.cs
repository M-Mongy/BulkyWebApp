using Bulky.Data.Data;
using Bulky.DataAccess.DBInitializer;
using Bulky.Model;
using Bulky.Model.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Bulky.DataAccess.DbInitializer
{
    public class DbInitializer : IDBInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDBContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDBContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {
            // 1. Apply any pending database migrations
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {
                // In a real application, you would log this exception
            }

            // 2. Create roles if they do not already exist
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
            }

            // 3. CRITICAL FIX: Create the admin user IF THEY DO NOT EXIST.
            // This check is now separate from the role creation.
            if (_userManager.FindByEmailAsync("admin@dotnetmastery.com").GetAwaiter().GetResult() == null)
            {
                // Create the admin user
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@dotnetmastery.com",
                    Email = "admin@dotnetmastery.com",
                    Name = "Admin User",
                    PhoneNumber = "1112223333",
                    StreetAddress = "123 Admin Ave",
                    State = "IL",
                    PostalCode = "60606",
                    City = "Chicago"
                }, "Admin123*").GetAwaiter().GetResult();

                // Find the newly created user and assign them to the Admin role
                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@dotnetmastery.com");
                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }
            }
        }
    }
}
