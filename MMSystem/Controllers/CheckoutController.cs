using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MMSystem.Models;
using MMSystem.Models.Repository;
using MMSystem.Models.ViewModels;
using Stripe;
using Stripe.Checkout;
using DotNetShipping;
using System.Collections.Specialized;
using DotNetShipping.ShippingProviders;
using System.Configuration;

namespace MMSystem.Controllers
{
    public class CheckoutController : Controller
    {
        
            private readonly TGManagementLLCContext _context;
        //private readonly IMerchRepository _repo;

        private readonly List<Size> SizeList = new List<Size>();
        private ICartRepository _CartRepo;


        public CheckoutController(TGManagementLLCContext context, ICartRepository ICartRepository)//, IMerchRepository repo
        {
            _context = context;
            _CartRepo = ICartRepository;
        }

        public async Task<ActionResult> CreateCustomer(int Id)//int CartID, int OwnerId
        {
            int CartId = Id;//This is Id because of the routing something in some config file
            //int CartID = 1;//This might come through as a parameter
            ShoppingCart ShoppingCart2 = HttpContext.Session.GetObject<ShoppingCart>("ShoppingCart");
            ShoppingCart ShoppingCart = _CartRepo.BuildShoppingCart(CartId);
            
            //Should be able to pull all information from the Cart/Owner
            
            int OwnerId = Convert.ToInt32(ShoppingCart.Cart.OwnerId);
            //
            
            ViewData["StateId"] = new SelectList(_context.Variable.Where(x => x.VariableGroupId == 3), "Value", "Name");
            ViewBag.PaymentAmount = ShoppingCart.Total;
            ViewBag.CartId = CartId;

            return View();
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomer([Bind("FirstName,LastName,Email,Phone,Address,City,ZipCode,StateId,CartId")] Models.Customer Customer)
        {
            try
            {
                Customer.CreatedOn = DateTime.Now;
                Customer.StatusId = 0;
                _context.Customer.Add(Customer);
                Cart Cart = _context.Cart.Where(x => x.CartId == Customer.CartId).Single();
                Cart.StatusId = 2;
                _context.Update(Cart);
                _context.SaveChanges();
                Customer.CustomerId = _context.Customer.Where(x => x.CartId == Cart.CartId).Select(x => x.CustomerId).Single();
                return RedirectToAction(nameof(CreatePayment), new {Id = Customer.CustomerId });
            }
            catch
            {
                return View();
            }
        }


        public ActionResult CreatePayment(int Id)
        {
            Models.Customer Customer = _context.Customer.Where(x => x.CustomerId == Id).Single();
            int CartId = Convert.ToInt32(Customer.CartId);
            ShoppingCart ShoppingCart = _CartRepo.BuildShoppingCart(CartId);
            decimal CartTotal = ShoppingCart.Total;
            var Quantity = ShoppingCart.CartItems.Count;

            //Need to learn and set this up but maybe
            //start with a random base shipping cost
            //double ShippingCost = CalculateShipping(Customer,ShoppingCart);

            StripeConfiguration.ApiKey = "sk_test_2FdAwKCfRfkX2aJrlzGm2gwm00BBKZyH8O";
            //StripeConfiguration.ApiKey = "pk_test_BjTmcRGv7N5C4w6p1Wpr4t5x0082lcBMjL";
            //
            //LOOKING AT SHIPPING COST CALCULATION AND SHIPPING COST FOR ARTIST
            //***SHIPPING HERE
            //
            List<SessionLineItemOptions> CartItemList =  new List<SessionLineItemOptions> {};
            
            foreach (CartItem CartItem in ShoppingCart.CartItems)
            {
                Merch Merch = _context.Merch.Where(x => x.MerchId == CartItem.MerchId).Single();
                SessionLineItemOptions Item = new SessionLineItemOptions
                {
                    Name = Merch.Title,
                    Description = Merch.Description,
                    Amount = Merch.Price,
                    Currency = "usd",
                    Quantity = 1,
                };
                
                if (CartItem.IsReplica == 1)
                {
                    Replica Replica = _context.Replica.Where(x => x.ReplicaId == Convert.ToInt32(CartItem.ReplicaId)).Single();
                    Item.Amount = Replica.Price;
                };
                Item.Amount = Item.Amount * 100;
                CartItemList.Add(Item);
            };

            var Options = new SessionCreateOptions
            {
                SuccessUrl = "https://ThePaymentBuilder.com/Checkout/RedirectStripe?SessionId={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://example.com/cancel",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = CartItemList
            };

            var Service = new SessionService();
            Session Session = Service.Create(Options);

            Cart Cart = _context.Cart.Where(x => x.CartId == ShoppingCart.Cart.CartId).Single();
            Cart.SessionId = Session.Id;
            Cart.StatusId = 3;
            _context.Update(Cart);
            _context.SaveChanges();
            return View(Customer);
            //return RedirectToAction(nameof(RedirectStripe),new {SessionId =  Session.Id});
            //Order isn't created here because it should be created when a payment is complete, not pending.
            

        }

        public ActionResult RedirectStripe(int Id)
        {
            int CustomerId = Id;
            Models.Customer Customer = _context.Customer.Where(x => x.CustomerId == CustomerId).Single();
            Customer.StatusId = 1;
            String SessionId = _context.Cart.Where(x => x.CartId == Customer.CartId).Select(x => x.SessionId).Single();
            ViewBag.SessionId = SessionId;
            _context.Update(Customer);
            _context.SaveChanges();
            return View();
        }

            //[HttpPost]
            //[ValidateAntiForgeryToken]
            //public async Task<IActionResult> Checkout()
            //{
            //    ShoppingCart ShoppingCart = HttpContext.Session.GetObject<ShoppingCart>("ShoppingCart");

            //    //CREATE PAYMENT STUFF HERE?!?!?!?!?
            //    return await;
            //}
            // GET: Checkout
            public ActionResult Index()
        {
            return View();
        }

        // GET: Checkout/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Checkout/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Checkout/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Checkout/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Checkout/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Checkout/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Checkout/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public Double CalculateShipping(Models.Customer customer,ShoppingCart ShoppingCart)
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;

            // You will need a license #, userid and password to utilize the UPS provider.
            string upsLicenseNumber = appSettings["UPSLicenseNumber"];
            string upsUserId = appSettings["UPSUserId"];
            string upsPassword = appSettings["UPSPassword"];

            // You will need an account # and meter # to utilize the FedEx provider.
            string fedexKey = appSettings["FedExKey"];
            string fedexPassword = appSettings["FedExPassword"];
            string fedexAccountNumber = appSettings["FedExAccountNumber"];
            string fedexMeterNumber = appSettings["FedExMeterNumber"];

            // You will need a userId to use the USPS provider. Your account will also need access to the production servers.
            string uspsUserId = appSettings["USPSUserId"];

            //***Maybe 

            // Setup package and destination/origin addresses
            var packages = new List<Package>();
            packages.Add(new Package(12, 12, 12, 35, 150));
            packages.Add(new Package(4, 4, 6, 15, 250));

            var origin = new DotNetShipping.Address("", "", "06405", "US");
            var destination = new DotNetShipping.Address("", "", "20852", "US"); // US Address

            // Create RateManager
            var rateManager = new RateManager();

            // Add desired DotNetShippingProviders
            rateManager.AddProvider(new UPSProvider(upsLicenseNumber, upsUserId, upsPassword));
            rateManager.AddProvider(new FedExProvider(fedexKey, fedexPassword, fedexAccountNumber, fedexMeterNumber));
            rateManager.AddProvider(new USPSProvider(uspsUserId));

            // (Optional) Add RateAdjusters
            //rateManager.AddRateAdjuster(new PercentageRateAdjuster(.9M));

            // Call GetRates()
            Shipment shipment = rateManager.GetRates(origin, destination, packages);

            // Iterate through the rates returned
            foreach (Rate rate in shipment.Rates)
            {
                Console.WriteLine(rate);
            }

            return 12.13;
        }
    }
}