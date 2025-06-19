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
        void Save();
    }
}
