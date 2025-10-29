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
    public class OrderDetailRepository : Repository<OrderDetail>, IorderDetailRepository
    {
        private ApplicationDBContext _db;
        public OrderDetailRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void save()
        {
            _db.SaveChanges();
        }

        public void Update(OrderDetail obj)
        {
           _db.orderDetails.Update(obj);  
        }
    }
}
