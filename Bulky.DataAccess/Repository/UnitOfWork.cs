using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.Data.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Model.Models;
using Bulky.Models;


namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IuintOfWork
    {
        private ApplicationDBContext _db;
        public IcategoriesRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IcompanyRepository Company { get; private set; }
        public IshoppingCartRepository shoppingCart { get; private set; }
        public IapplicationUsersRepository applicationUser { get; private set; }

        public UnitOfWork(ApplicationDBContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            shoppingCart = new ShoppingCartRepository(_db);
            applicationUser = new ApplicationUserRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}

