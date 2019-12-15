using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models
{
    public class ShoppingCart
    {
        public ZHCart Cart { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal Total { get; set; } = 0m;
    }
}
