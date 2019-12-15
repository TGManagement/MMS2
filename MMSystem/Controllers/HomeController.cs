using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MMSystem.Models;
using MMSystem.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using MMSystem.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace MMSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly TGManagementLLCContext _context;
        //private readonly IMerchRepository _repo;

        private readonly List<Size> SizeList = new List<Size>();
        private ICartRepository _CartRepo;

        
        public HomeController(TGManagementLLCContext context, ICartRepository ICartRepository)//, IMerchRepository repo
        {
            _context = context;
            _CartRepo = ICartRepository;
        }

            public IActionResult Index()
            {
            ShoppingCart ShoppingCart = HttpContext.Session.GetObject<ShoppingCart>("ShoppingCart");
            if (ShoppingCart == null)
            {
                ShoppingCart = new ShoppingCart();
            }

            var MerchIndexListVm = (from m in _context.Merch
                                    join s in _context.Size on m.SizeId equals s.SizeId
                                    where m.OwnerId == 1//TGMGMTLLC Template
                                    select new MerchIndexListVm() {
                                        Merch = m,
                                        SizeName = s.Name
                                       
                                    });

            MerchOwnerVm MerchOwnerVm = new MerchOwnerVm();
            Owner Owner = _context.Owner.Where(x => x.OwnerId == 1).Single();//TGMGMTLLC Template
            MerchOwnerVm.MerchIndexListVm = MerchIndexListVm;
            MerchOwnerVm.Owner = Owner;
            MerchOwnerVm.ShoppinCart = ShoppingCart;
            return View(MerchOwnerVm);
        }

        public IActionResult ViewCart()
        {
            ShoppingCart ShoppingCart = HttpContext.Session.GetObject<ShoppingCart>("ShoppingCart");
            Merch Merch;
            string SizeName;
            string MaterialName;
            //List<Size> Sizes = _context.Size.Where(x=>ShoppingCart.CartItems.Contains(x.)
            foreach(CartItem TempCartItem in ShoppingCart.CartItems)
            {
                Merch = _context.Merch.Where(x => x.MerchId == TempCartItem.MerchId).Single();
                ViewData["MerchName"+TempCartItem.CartItemId.ToString()] = Merch.Title;
                if(TempCartItem.IsReplica == 1)
                {
                    Replica Replica = _context.Replica.Where(x => x.ReplicaId == TempCartItem.ReplicaId).Single();
                    SizeName = _context.Size.Where(x => x.SizeId == Replica.SizeId).Select(x => x.Name).Single();
                    MaterialName = _context.Material.Where(x => x.MaterialId == Replica.MaterialId).Select(x => x.Name).Single();

                    ViewData["MerchSizeName" + TempCartItem.CartItemId.ToString()] =  SizeName;
                    ViewData["MerchMaterialName" + TempCartItem.CartItemId.ToString()] = MaterialName;
                    ViewData["Replica" + TempCartItem.CartItemId.ToString()] = "Replica";
                }
                else
                {
                    SizeName = _context.Size.Where(x => x.SizeId == Merch.SizeId).Select(x => x.Name).Single();
                    MaterialName = _context.Material.Where(x => x.MaterialId == Merch.MaterialId).Select(x => x.Name).Single();

                    ViewData["MerchSizeName" + TempCartItem.CartItemId.ToString()] = SizeName;
                    ViewData["MerchMaterialName" + TempCartItem.CartItemId.ToString()] = MaterialName;
                    ViewData["Replica" + TempCartItem.CartItemId.ToString()] = "";
                }
                ViewData["MerchImage" + TempCartItem.CartItemId.ToString()] = Merch.PictureUrl;

            }


            var test = ShoppingCart.Cart.CartId;
            return View(ShoppingCart);
        }

        // GET: Replica/Details/5
        public async Task<IActionResult> Details(int Id)
        {

            var TheMerch = (from m in _context.Merch
                            join si in _context.Size on m.SizeId equals si.SizeId
                            where m.MerchId == Id &&  m.OwnerId == 1//TGMGMTLLC Template
                            select new MerchReplicasVM()
                            {
                                Merch = m,
                                SizeName = si.Name

                            }).Single();
            
            //TheMerch.MaterialIdName = _context.Material.Where(x => x.MaterialId == TheMerch.Merch.MaterialId).First().Name;
            

            List<Replica> Replicas = _context.Replica.Where(x => x.MerchId == TheMerch.Merch.MerchId).ToList();
            MaterialSizeVM MaterialSizeVM = new MaterialSizeVM();
            MaterialSizeVM.SizeName = TheMerch.SizeName;
            MaterialSizeVM.MerchId = TheMerch.Merch.MerchId;
            MaterialSizeVM.Merch = TheMerch.Merch;
            HttpContext.Session.SetString("MerchId", Id.ToString());
            ViewData["ReplicaCount"] = 0;
            if (Replicas.Count() > 0)
            { 

                //TheMerch.ReplicaList = Replicas;
                ViewData["ReplicaCount"] = Convert.ToInt32(Replicas.Count());
                //SelectList list = _context.Replica.Select(x => new { x.MaterialId}).ToList();
                List<int> MaterialIdList = Replicas.Select(x => x.MaterialId).ToList();

                MaterialSizeVM.SizeIdList = _context.Size.Where(x => x.SizeId == 0).Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = Convert.ToString(x.SizeId)
                }).ToList();


                if (MaterialIdList.Count > 0)
                {
                    MaterialSizeVM.MaterialIdList = _context.Material.Where(x => MaterialIdList.Contains(x.MaterialId)).Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = Convert.ToString(x.MaterialId)
                    }).ToList();
                    ViewData["MaterialId"] = new SelectList(_context.Material.Where(x => MaterialIdList.Contains(x.MaterialId)), "MaterialId", "Name").Prepend(new SelectListItem("Select One", "0"));

                    //, "MaterialId", "Name").Prepend(new SelectListItem("Select One", "0"));
                }
                else
                {
                    ViewData["MaterialId"] = new SelectList(_context.Material.Where(x => MaterialIdList.Contains(x.MaterialId)), "MaterialId", "Name").Prepend(new SelectListItem("Coming Soon", "0"));
                
                }
            }

            if (TheMerch == null)
            {
                return NotFound();
            }
            return View(MaterialSizeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddToCart([Bind("Replica.SizeId,Replica.MaterialId")] MerchReplicasVM MerchReplicasVM,Boolean Print)
        public async Task<IActionResult> AddToCart(Boolean Print)
        {
            int MerchId = Convert.ToInt32(HttpContext.Session.GetString("MerchId"));//Convert.ToInt32(Request.Form["MerchId"].ToString());
            
            //int MaterialId,int MerchId,
            if (ModelState.IsValid)
            {
                Decimal Price = 0;
                ShoppingCart ShoppingCart =  HttpContext.Session.GetObject<ShoppingCart>("ShoppingCart");
                ShoppingCart = _CartRepo.CheckCart(ShoppingCart);
                CartItem CartItem;
                if (Print)
                {
                    int MaterialId = Convert.ToInt32(Request.Form["DDLMaterial"].ToString());
                    
                    var TempHolder= Request.Form["DDLSize"].ToString().Split("-");
                    string SizeName = TempHolder.GetValue(0).ToString().TrimEnd();

                    int SizeId = _context.Size.Where(x => x.Name == SizeName).Select(x => x.SizeId).Single();
                    int ReplicaId = _context.Replica.Where(x => x.SizeId == SizeId && x.MaterialId == MaterialId && x.MerchId == MerchId).Select(x => x.ReplicaId).Single();
                    Price = _context.Replica.Where(x => x.ReplicaId == ReplicaId).Select(x => x.Price).Single();
                    //var ReplicaId = _context.Replica.Where(x => x.SizeId == SizeId && x.MaterialId == MaterialId && x.MerchId == MerchId).Select(x => x.ReplicaId).Single();
                    //*************    
                    //Need _context.Cart here
                    //maybe use cart repository
                    //*************
                    CartItem = new CartItem { CartId = ShoppingCart.Cart.CartId, IsReplica = 1,MerchId = MerchId, ReplicaId = ReplicaId, Price = Price };
                }
                else
                {
                    Price = Convert.ToInt32(_context.Merch.Where(x => x.MerchId == MerchId).Select(x => x.Price).Single());
                    CartItem = new CartItem
                    {
                        CartId = ShoppingCart.Cart.CartId,
                        IsReplica = 0,
                        MerchId = MerchId,
                        Price = Price
                    };
                }
                DbContextOptionsBuilder DbContextOptionsBuilder = new DbContextOptionsBuilder();
                DbContextOptionsBuilder.EnableSensitiveDataLogging();
                ShoppingCart.CartItems.Add(CartItem);
                ShoppingCart.Total = ShoppingCart.Total + Price;
                _context.CartItem.Add(CartItem);
                Cart Cart = _context.Cart.Where(x => x.CartId == ShoppingCart.Cart.CartId).Single();
                Cart.StatusId = 2;
                Cart.UpdatedOn = DateTime.Now;
                Cart.Total = ShoppingCart.Total;
                _context.Cart.Update(Cart);
                //_context.Add(replica);
                _context.SaveChanges();
                HttpContext.Session.SetObject("ShoppingCart", ShoppingCart);
                return RedirectToAction(nameof(Index));
            }

            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void SetSocialMediaVariables(int OwnerId)
        {
            var Owner = _context.Owner.Where(x => x.OwnerId == OwnerId).SingleOrDefault();
            HttpContext.Session.SetString("Facebook",Owner.Facebook);
            HttpContext.Session.SetString("Twitter", Owner.Twitter);
            HttpContext.Session.SetString("Instagram", Owner.Instagram);
            HttpContext.Session.SetString("Pintrest", Owner.Pintrest);
        }
        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateReplicaSize(int MaterialId)
        {
            var MerchId = Convert.ToInt32(HttpContext.Session.GetString("MerchId"));
            //List<int> SizeIdList = Replicas.Select(x => x.SizeId).ToList();
            List<int> SizeIds = _context.Replica.Where(x => x.MaterialId == MaterialId && x.MerchId == MerchId).Select(x=>x.SizeId).ToList();
            var SizeNames = _context.Size.Where(x => SizeIds.Contains(x.SizeId)).Select(x => x.Name);
            SizeNames = (from s in _context.Size
                        join r in _context.Replica on s.SizeId equals r.SizeId
                        where SizeIds.Contains(s.SizeId) && r.MaterialId == MaterialId
                        select new Size
                        {
                            Name = s.Name +" - $"+ r.Price.ToString()
                        }).Select(x => x.Name);
            return Json(SizeNames);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnableAddToCart(int Id)
        {
            var Result = false;
            if (Id > 0)
            {
                Result = true;
            }
            return Json(Result);
        }


        //public void UpdateReplicaSize2(int MerchId, int MaterialId)
        //{
        //    var Merch = _context.Merch.Where(x => x.MerchId == MerchId).First();
        //    ViewData["SizeId"] = new SelectList(_context.Size.Where(x => x.MerchTypeId == Merch.MerchTypeId), "SizeId", "Name");
        //}

        //[HttpPost]
        //public void UpdateReplicaSize3(int MaterialId)
        //{
        //    var MaterialIdtest = Request.Form["Material"];
        //    var Replicas = _context.Replica.Where(x => x.MerchId == Convert.ToInt32(ViewData["MerchId"]));
        //    List<int> SizeIdList = Replicas.Select(x => x.SizeId).ToList();
        //    ViewData["SizeId"] = new SelectList(_context.Size.Where(x=> SizeIdList.Contains(x.SizeId)),"SizeId","Name");
        //    var SizeIds = _context.Replica.Where(x => x.MaterialId == MaterialId).ToList();
        //    //var dictionary = _context.Size.ToDictionary<int, string>(k => k.AuthorId, v => v.Name);
        //    //Options = new SelectList(dictionary, "Key", "Value");

        //    //JsonResult
        //    //return Json(id, JsonRequestBehavior.AllowGet);
        //}

        public IActionResult Checkout()
        {
            //public IActionResult Get([FromQuery(Name = "page")] string page)
            //int test = Convert.ToInt32(HttpContext.Request.Query["CartId"].ToString());
            //String CompanyName = HttpContext.Request.Query["CompanyName"].ToString();
            //String Description = HttpContext.Request.Query["Description"].ToString();

            ShoppingCart ShoppinCart = HttpContext.Session.GetObject<ShoppingCart>("ShoppingCart");
            int CartID = Convert.ToInt32(ShoppinCart.Cart.CartId);//FIGURE OUT QUERY STRING HERE
            //ShoppingCart ShoppingCart = _CartRepo.BuildShoppingCart(CartID);

            ViewBag.PaymentAmount = "test";//ShoppingCart.Total;
            ViewBag.CompanyName = "test";//CompanyName;
            ViewBag.Description = "test";//Description;

            return View();// ShoppingCart);
        }
    }
}
