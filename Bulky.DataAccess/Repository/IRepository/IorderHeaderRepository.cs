using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Model.Models;
using Bulky.Models;
namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);

        // Corrected method and parameter names
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

        // Corrected parameter names
        void UpdateStripePayment(int id, string sessionId, string paymentIntentId);
    }
}
