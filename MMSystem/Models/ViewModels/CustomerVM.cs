using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models.ViewModels
{
    public class CustomerVM
    {
        public ShoppingCart ShoppingCart { get; set; }

        public Customer Customer { get; set; }
    }
}
