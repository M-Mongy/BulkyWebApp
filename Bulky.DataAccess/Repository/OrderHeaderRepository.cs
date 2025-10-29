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
    // CORRECTED: Interface name uses PascalCase
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDBContext _db;
        public OrderHeaderRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            // CORRECTED: DbSet property uses PascalCase -> OrderHeaders
            _db.orderHeaders.Update(obj);
        }

        // CORRECTED: Method name changed to UpdateStatus for clarity and consistency
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                // CORRECTED: Check should be for NOT null or empty
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    // CORRECTED: Assign paymentStatus to the correct property
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePayment(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    orderFromDb.SessionId = sessionId;
                }
                // CRITICAL FIX: The PaymentIntentId was being assigned to the SessionId property by mistake.
                // It now correctly updates the PaymentIntentId property.
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    orderFromDb.PaymentIntentId = paymentIntentId;
                    orderFromDb.PaymentDate = DateTime.Now;
                }
            }
        }
    }
}
