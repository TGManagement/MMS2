using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MMSystem.Models;
using MMSystem.Models.ViewModels;
using MMSystem.Models.Repository;
using Microsoft.AspNetCore.Http;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace MMSystem.Controllers
{
    public class MerchController : Controller
    {
        private readonly TGManagementLLCContext _context;
        //private readonly IMerchRepository _repo;
        private readonly Owner TheOwner = new Owner();
        public MerchController(TGManagementLLCContext context)//, IMerchRepository repo
        {
            _context = context;
           // _repo = repo;
        }

        // GET: Merch
        public async Task<IActionResult> Index(string SortOrder, string SearchString, string CurrentFilter, int? PageNumber)
        {
            //var test = _repo.GetAllMerch();
            //var tGManagementLLCContext = _context.Merch.Include(m => m.Owner).Include(m => m.Size);
            //return View(await tGManagementLLCContext.ToListAsync());

            
            var TheMerch = (from m in _context.Merch
                            join si in _context.Size on m.SizeId equals si.SizeId
                            join st in _context.Variable on m.StatusId equals st.Value
                            where st.VariableGroupId == 1 && m.OwnerId == TheOwner.OwnerId//TGMGMTLLC Template
                            select new MerchIndexListVm()
                            {
                                Merch = m,
                                SizeName = si.Name,
                                StatusIdName = st.Name,

                            });


            if (SearchString != null)
            {
                PageNumber = 1;
            }
            else
            {
                SearchString = CurrentFilter;
            }
            ViewData["CurrentSort"] = SortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(SortOrder) ? "NameDesc" : "";
            ViewData["DateSortParm"] = SortOrder == "Date" ? "DateDesc" : "Date";
            ViewData["CurrentFilter"] = SearchString;
            if (!String.IsNullOrEmpty(SearchString))
            {
                TheMerch = TheMerch.Where(s => s.Merch.Title.Contains(SearchString));
            }
            switch (SortOrder)
            {
                case "NameDesc":
                    TheMerch = TheMerch.OrderByDescending(m => m.Merch.Title); break;
                case "Date":
                    TheMerch = TheMerch.OrderBy(m => m.Merch.CreatedOn); break;
                case "DateDesc":
                    TheMerch = TheMerch.OrderByDescending(m => m.Merch.CreatedOn); break;
                default:
                    TheMerch = TheMerch.OrderBy(m => m.Merch.Title);break;
            }

            

            int pageSize = 10;
            return View(await PagedList<MerchIndexListVm>.CreateAsync(TheMerch, PageNumber ?? 1, pageSize));

            //return View(await TheMerch.ToListAsync());
            //return View(await TheMerch.AsNoTracking().ToListAsync());
        }

        // GET: Merch/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merch = await _context.Merch
                .Include(m => m.Owner)
                .Include(m => m.Size)
                .FirstOrDefaultAsync(m => m.MerchId == id);
            if (merch == null)
            {
                return NotFound();
            }

            var merch2 = (from m in _context.Merch
                          join s in _context.Size on m.SizeId equals s.SizeId
                          join sta in _context.Variable on m.StatusId equals sta.Value
                          join mt in _context.MerchType on m.MerchTypeId equals mt.MerchTypeId
                          where sta.VariableGroupId == 1 && m.MerchId == id && m.OwnerId == TheOwner.OwnerId //TGMGMTLLC Template
                          select new MerchIndexListVm()
                          {
                              Merch = m,
                              MerchTypeName = mt.Name,
                              StatusIdName = sta.Name,
                              SizeName = s.Name,
                          }
                            );

            if (merch2 == null)
            {
                return NotFound();
            }

            return View(merch2.Single());
        }

        // GET: Merch/Create
        public IActionResult Create()
        {
            
            /*var NewMerch = (from m in _context.Merch
                            join s in _context.Size on m.SizeId equals s.SizeId
                            select new Merch()
                            {

                            });*/
            ViewData["MerchTypeId"] = new SelectList(_context.MerchType, "MerchTypeId", "Name");
            ViewData["StatusIdName"] = new SelectList(_context.Variable.Where(x => x.VariableGroupId == 1), "Value", "Value2");
            ViewData["SizeId"] = new SelectList(_context.Size, "SizeId", "Name");
            return View();
        }

        // POST: Merch/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("MerchId,Title,Description,SizeId,Price,Status,PictureUrl")] Merch merch)
        {
            if (ModelState.IsValid)
            {
                //var TheOwner = new Owner();
                merch.OwnerId = TheOwner.OwnerId;  
                merch.MerchTypeId = 1; //TGMGMTLLC Template 
                merch.PictureUrl = "https://www.ThePaymentBuilder.com/../HtmlTemplate/images/Paintings/billyberkesfineart/" + merch.PictureUrl; //TGMGMTLLC Template 
                merch.CreatedOn = DateTime.Now;
                _context.Add(merch);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StatusIdName"] = new SelectList(_context.Variable, "Value", "Value2",merch.StatusId);
            ViewData["SizeId"] = new SelectList(_context.Size, "SizeId", "Name", merch.SizeId);
            return View(merch);
        }


        /* //TGMGMTLLC Template Just in case they sell differenet types of Merch
         * div class="form-group"> 
                <label asp-for="MerchTypeId" class="control-label"></label>
                <select asp-for="MerchTypeId" class="form-control" asp-items="ViewBag.MerchTypeId"></select>
                <span asp-validation-for="MerchTypeId" class="text-danger"></span>
            </div>*/



        // GET: Merch/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merch = await _context.Merch.FindAsync(id);
            if (merch == null || merch.OwnerId != TheOwner.OwnerId)//TGMGMTLLC Template
            {
                return NotFound();
            }
            ViewData["PictureUrl"] = merch.PictureUrl;
            ViewData["MerchTypeId"] = new SelectList(_context.MerchType, "MerchTypeId", "Name");
            ViewData["StatusIdName"] = new SelectList(_context.Variable.Where(x => x.VariableGroupId == 1), "Value", "Value2");
            ViewData["SizeId"] = new SelectList(_context.Size, "SizeId", "Name");
            return View(merch);
        }

        // POST: Merch/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MerchId,Title,Description,MerchTypeId,SizeId,Price,Status,PictureUrl,PurchasedOn,OwnerId,UpdatedOn")] Merch merch)
        {
            var MerchCheck = (from m in _context.Merch
                              where m.MerchId == merch.MerchId
                              select new Merch()
                              {
                                  OwnerId = m.OwnerId,
                                  PictureUrl = m.PictureUrl
                              }).Single();

            if (id != merch.MerchId || MerchCheck.OwnerId != TheOwner.OwnerId)//TGMGMTLLC Template
            {
                return NotFound();
            }

            merch.OwnerId = MerchCheck.OwnerId;

            if (merch.PictureUrl == null)
            {
                merch.PictureUrl = MerchCheck.PictureUrl;
            }
            else
            {
                merch.PictureUrl = "https://www.ThePaymentBuilder.com/images/Paintings/billyberkesfineart/" + merch.PictureUrl; //TGMGMTLLC Template 
            }

            if (ModelState.IsValid)
            {
                merch.UpdatedOn = DateTime.Now;
                try
                {
                    _context.Update(merch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MerchExists(merch.MerchId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MerchTypeId"] = new SelectList(_context.MerchType, "MerchTypeId", "Name",merch.MerchTypeId);
            ViewData["StatusIdName"] = new SelectList(_context.Variable.Where(x => x.VariableGroupId == 1), "Value", "Value2",merch.StatusId);
            ViewData["SizeId"] = new SelectList(_context.Size, "SizeId", "Name", merch.SizeId);


            return View(merch);
        }

        // GET: Merch/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merch = await _context.Merch
                .Include(m => m.Owner)
                .Include(m => m.Size)
                .FirstOrDefaultAsync(m => m.MerchId == id);
            if (merch == null)
            {
                return NotFound();
            }

            var merch2 = (from m in _context.Merch
                          join s in _context.Size on m.SizeId equals s.SizeId
                          join sta in _context.Variable on m.StatusId equals sta.Value
                          join mt in _context.MerchType on m.MerchTypeId equals mt.MerchTypeId
                          where sta.VariableGroupId == 1 && m.MerchId == id && m.OwnerId == TheOwner.OwnerId //TGMGMTLLC Template
                          select new MerchIndexListVm()
                          {
                             Merch = m,
                              MerchTypeName = mt.Name,
                              StatusIdName = sta.Name,
                              SizeName = s.Name
                          }
                            );

            if (merch2 == null)
            {
                return NotFound();
            }


           

            return View(merch2.Single());
        }

        // POST: Merch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var merch = await _context.Merch.FindAsync(id);
            _context.Merch.Remove(merch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MerchExists(int id)
        {
            return _context.Merch.Any(e => e.MerchId == id);
        }
    }
}
