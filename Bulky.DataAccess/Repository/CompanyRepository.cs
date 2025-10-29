using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.Data.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Model.Models;

namespace Bulky.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>,IcompanyRepository
    {
        private readonly ApplicationDBContext _db;

        public CompanyRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;   
        }

        public void save()
        {
            _db.SaveChanges();
        }
        public void Update(Company obj)
        {
            _db.companies.Update(obj);
        }
    }
}
