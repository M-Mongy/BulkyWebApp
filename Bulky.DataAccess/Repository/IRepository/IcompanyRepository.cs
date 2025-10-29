using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.Model.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IcompanyRepository : IRepository<Company>
    {
        void Update(Company obj);
    }
}
