using Microsoft.AspNetCore.Http;
using MMSystem.Models.ViewModels;
using MMSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MMSystem.Models.Repository;

namespace MMSystem.Models
{
    public class CartRepository : ICartRepository
    {
        
        public TGManagementLLCContext _context;
        public readonly Owner TheOwner = new Owner();
        public CartRepository(TGManagementLLCContext context)
        {
            _context = context;
        }
        public ShoppingCart CheckCart(ShoppingCart ShoppingCart)
        {
            Owner Owner = new Owner();
            ZHCart ReturnCart = new ZHCart();
            ZHCart OldCart = new ZHCart();
            Boolean NeedNewCart = true;
            if (ShoppingCart != null)
            {
                OldCart = ShoppingCart.Cart;
                if (DateTime.Now.DayOfYear - Convert.ToDateTime(OldCart.CreatedOn).DayOfYear >= 1)
                {
                    Cart RemoveCart = _context.Cart.Where(x => x.CartId == ShoppingCart.Cart.CartId).Single();
                    _context.Cart.Remove(RemoveCart);
                    _context.SaveChanges();
                    //The DB Trigger should handle the cart to history cart
                }
                else
                {
                    NeedNewCart = false;
                    ReturnCart = OldCart;
                }
            }
            
            if(NeedNewCart == true)
            {
                var guid = Guid.NewGuid();
                var IsDuplicate = true;
                while (IsDuplicate)
                {
                    ZHCart TestHistoryCart = _context.ZHCart.Where(x => x.CartGuid == guid).SingleOrDefault();
                    Cart TestCart = _context.Cart.Where(x => x.CartGuid == guid).SingleOrDefault();
                    if (TestHistoryCart == null && TestCart == null)
                    {
                        IsDuplicate = false;
                    }
                    else
                    {
                        guid = Guid.NewGuid();
                    }
                }
                ReturnCart.CartGuid = guid;
                ReturnCart.CreatedOn = DateTime.Now;
                ReturnCart.OwnerId = TheOwner.OwnerId;
                ReturnCart.StatusId = 0;
                Cart Cart = new Cart { CartGuid = guid, OwnerId = Owner.OwnerId, StatusId = 0, UpdatedOn = DateTime.Now, CreatedOn = DateTime.Now, Total = 0, };
                _context.Cart.Add(Cart);
                _context.SaveChanges();
                //Need to do below because CartId isn't created until it is in the database
                ReturnCart.CartId = _context.Cart.Where(x => x.CartGuid == guid).Select(x => x.CartId).Single();
                if (ShoppingCart == null)
                {
                    ShoppingCart = new ShoppingCart();
                }
                else
                {
                    ShoppingCart.CartItems.Clear();
                    ShoppingCart.Total = 0;
                }
                
            }
            ShoppingCart.Cart = ReturnCart;
            return ShoppingCart;
        }
        public bool AddItemToCart(ShoppingCart ShoppingCart, ZHCart CurrentCart, MerchReplicasVM MerchReplicasVM, Boolean Print)
        {
            Boolean Result = false;
            var CartItem = new CartItem();
            try { 
                 if (Print)
                {
                    var ReplicaId = _context.Replica.Where(x => x.SizeId == MerchReplicasVM.Replica.SizeId && x.MaterialId == MerchReplicasVM.Replica.MaterialId && x.MerchId == MerchReplicasVM.Replica.MerchId).Select(x => x.ReplicaId).Single();
                    CartItem.CartId = CurrentCart.CartId;
                    CartItem.MerchId = ReplicaId;
                    _context.CartItem.Add(CartItem);
                }
                else
                {
                    var MerchId = MerchReplicasVM.Merch.MerchId;
                    CartItem.CartId = CurrentCart.CartId;
                    CartItem.MerchId = MerchId;
                    _context.CartItem.Add(CartItem);
                }
                 _context.SaveChanges();
                Result = true;
                //HttpContext.ISession.SetObject("ShoppingCart",ShoppingCart);
                ShoppingCart.CartItems.Add(CartItem);
                ShoppingCart.Cart.StatusId = 1;
                ShoppingCart.Total = ShoppingCart.Total + Convert.ToDecimal(CartItem.Price);
            }
            catch {

            }


            return Result;

}
        public ShoppingCart BuildShoppingCart(int CartId)
        {
            ShoppingCart ShoppingCart = new ShoppingCart();
            try
            {
                Cart Cart = _context.Cart.Where(x => x.CartId == CartId).SingleOrDefault();
                ShoppingCart.Cart = new ZHCart
                {
                    CartId = Cart.CartId,
                    CartGuid = Cart.CartGuid,
                    OwnerId = Cart.OwnerId,
                    Total = Cart.Total,
                    StatusId = Cart.StatusId,
                    CreatedOn = Cart.CreatedOn,
                    UpdatedOn = Convert.ToString(Cart.UpdatedOn)
                };
                ShoppingCart.CartItems = _context.CartItem.Where(x => x.CartId == CartId).ToList();
                foreach(CartItem TempCartItem in ShoppingCart.CartItems)
                {
                    ShoppingCart.Total = ShoppingCart.Total + Convert.ToDecimal(TempCartItem.Price);
                }
            }
            catch
            {

            }
            return ShoppingCart;
        }
    }

    


}
