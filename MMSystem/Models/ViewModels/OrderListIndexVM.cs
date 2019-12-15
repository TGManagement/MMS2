using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models.ViewModels
{
    public class OrderListIndexVM
    {
        public Order Order { get; set; }
        public Customer Customer { get; set; }
        public ZHCart ZHCart { get; set; }
    }
}
