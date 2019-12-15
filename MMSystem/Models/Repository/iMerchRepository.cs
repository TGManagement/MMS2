using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models.Repository
{
    public interface IMerchRepository
    {
        IQueryable<Merch> GetAllMerch();
    }
}
