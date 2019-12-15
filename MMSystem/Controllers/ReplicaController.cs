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

namespace MMSystem.Controllers
{
    public class ReplicaController : Controller
    {
        private readonly TGManagementLLCContext _context;
        private readonly Owner TheOwner = new Owner();

        public ReplicaController(TGManagementLLCContext context)
        {
            _context = context;
        }

        // GET: Replica
        public async Task<IActionResult> Index(int Id)
        {
            
            var Replica = (from r in _context.Replica
                               join s in _context.Size on r.SizeId equals s.SizeId
                               join ma in _context.Material on r.MaterialId equals ma.MaterialId
                               join m in _context.Merch on r.MerchId equals m.MerchId
                               where r.MerchId == Id && m.OwnerId == TheOwner.OwnerId //TGMGMTLLC Template
                               select new ReplicaIndexListVm()
                               {
                                   MaterialName = ma.Name,
                                   SizeName = s.Name,
                                   Replica = r,
                                   PictureUrl = m.PictureUrl

                               });
            var MerchInfo= _context.Merch.Where(x => x.MerchId == Id).Single();
            ViewData["PictureUrl"] = MerchInfo.PictureUrl;
            ViewData["MerchId"] = MerchInfo.MerchId.ToString();
            return View(await Replica.ToListAsync());
        }

        // GET: Replica/Details/5
        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var Replica = await _context.Replica
                .FirstOrDefaultAsync(m => m.ReplicaId == Id);
            if (Replica == null)
            {
                return NotFound();
            }

            return View(Replica);
        }

        [HttpGet]
        // GET: Replica/Create
        public IActionResult Create(int Id)
        {

                var MerchVm = (from m in _context.Merch
                             where m.MerchId == Id && m.OwnerId == TheOwner.OwnerId //TGMGMTLLC Template
                               select new ReplicaIndexListVm()
                             {
                                 Merch = m
                            }).Single();
                ViewData["SizeId"] = new SelectList(_context.Size.Where(x => x.MerchTypeId == MerchVm.Merch.MerchTypeId), "SizeId","Name");
                ViewData["MaterialId"] = new SelectList(_context.Material.Where(x => x.MerchTypeId == MerchVm.Merch.MerchTypeId), "MaterialId", "Name");
                ViewData["PictureUrl"] = MerchVm.Merch.PictureUrl;
                ViewData["MerchId"] = MerchVm.Merch.MerchId;





            return View(MerchVm.Replica);
        }

        // POST: Replica/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReplicaId,MerchId,SizeId,MaterialId,Price")] Replica replica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(replica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(replica);
        }

        // GET: Replica/Edit/5
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var Replica = await _context.Replica.FindAsync(Id);
            Replica.Merch = await _context.Merch.FindAsync(Replica.MerchId);

            ViewData["SizeId"] = new SelectList(_context.Size.Where(x => x.MerchTypeId == Replica.Merch.MerchTypeId), "SizeId", "Name");
            ViewData["MaterialId"] = new SelectList(_context.Material.Where(x => x.MerchTypeId == Replica.Merch.MerchTypeId), "MaterialId", "Name");
            ViewData["PictureUrl"] = Replica.Merch.PictureUrl;

            if (Replica == null)
            {
                return NotFound();
            }
            return View(Replica);
        }
            
        // POST: Replica/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, [Bind("ReplicaId,MerchId,SizeId,MaterialId,Price")] Replica Replica)
        {
            if (Id != Replica.ReplicaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Replica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReplicaExists(Replica.ReplicaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { Id = Replica.ReplicaId});
            }
            return View(Replica);
        }

        // GET: Replica/Delete/5
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var Replica = await _context.Replica
                .FirstOrDefaultAsync(m => m.ReplicaId == Id);
            if (Replica == null)
            {
                return NotFound();
            }

            return View(Replica);
        }

        // POST: Replica/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            var Replica = await _context.Replica.FindAsync(Id);
            ViewData["MerchId"] = Convert.ToInt32(Replica.MerchId);
            _context.Replica.Remove(Replica);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new {Id = ViewData["MerchId"] });
        }

        private bool ReplicaExists(int Id)
        {
            return _context.Replica.Any(e => e.ReplicaId == Id);
        }

    }
}
