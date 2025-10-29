using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.Data.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Model.Models;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>,IshoppingCartRepository
    {
        private ApplicationDBContext _db;
        public ShoppingCartRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void save()
        {
            _db.SaveChanges();
        }

        public void Update(ShoppingCart obj)
        {
           _db.shoppingCarts.Update(obj);  
        }
    }
}
