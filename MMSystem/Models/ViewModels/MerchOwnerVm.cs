using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models.ViewModels
{
    public class MerchOwnerVm
    {
        public IQueryable<MerchIndexListVm> MerchIndexListVm { get; set; }
        public Owner Owner { get; set; }
        public string SizeName { get; set; }
        public ShoppingCart ShoppinCart { get; set; }
    }
}
