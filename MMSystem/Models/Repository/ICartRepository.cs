using MMSystem.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models.Repository
{
    public interface ICartRepository
    {
        ShoppingCart CheckCart(ShoppingCart ShoppingCart);
        bool AddItemToCart(ShoppingCart ShoppingCart, ZHCart CurrentCart, MerchReplicasVM MerchReplicasVM, Boolean Print);
        ShoppingCart BuildShoppingCart(int CartId);
    }
}
