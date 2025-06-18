﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IcategoriesRepository :IRepository<Category>
    {
        void Update(Category obj);

    }
}
