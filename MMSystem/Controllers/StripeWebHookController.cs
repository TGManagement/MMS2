using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MMSystem.Models;

namespace MMSystem.Controllers
{
    [Route("api/[controller]")]
    public class StripeWebHook : Controller
    {
        // You can find your endpoint's secret in your webhook settings
        const string secret = "whsec_...";
        private readonly TGManagementLLCContext _context;
        [HttpPost]
        public IActionResult Index()
        {

            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";

            var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], secret);

                // Handle the checkout.session.completed event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var Session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    // Fulfill the purchase...
                    HandleCheckoutSession(Session);
                }
                else
                {
                    //Cart Cart = _context.Cart.Where(x=>x.CartId = Sess)
                    return Ok();
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

        private void HandleCheckoutSession(Stripe.Checkout.Session Session)
        {
            try
            { 
            Cart Cart = _context.Cart.Where(x => x.SessionId == Session.Id).Single();
            Models.Customer Customer = _context.Customer.Where(x => x.CartId == Cart.CartId).Single();

            Models.Order Order = new Models.Order();
            Order.CartId = Cart.CartId;
            Order.CustomerId = Customer.CustomerId;
            Order.OwnerId = Cart.OwnerId;

            Cart.StatusId = 4;

            _context.Add(Cart);
            _context.Add(Order);

            
            _context.SaveChanges();
                //Send Email
            }
            catch (Exception Ex)
            {
                //Send failure email
                //Log a bad order creation and cart update
            }
            throw new NotImplementedException();
        }
    }
}
