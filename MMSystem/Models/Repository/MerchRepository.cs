using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models.Repository
{
    public class MerchRepository : IMerchRepository
    {
        private TGManagementLLCContext _context;
        private readonly Owner TheOwner = new Owner();
        
        public IQueryable<Merch> GetAllMerch()
        {
            return _context.Merch.Where(x => x.OwnerId == TheOwner.OwnerId);//TGMGMTLLC Template
        }
    }
}
