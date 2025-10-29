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
    public interface IorderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);

    }
}
