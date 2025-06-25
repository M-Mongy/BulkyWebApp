using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IuintOfWork
    {
        IcategoriesRepository Category { get; }
        IProductRepository Product { get; }
        IcompanyRepository Company { get; }
        IshoppingCartRepository ShoppingCart { get; }
        IapplicationUsersRepository applicationUser { get; }
        IorderDetailRepository orderDetail { get; }
        IOrderHeaderRepository orderHeader { get; }
        IproductImagesRepository ProductImages { get; }
        void Save();
    }
}
