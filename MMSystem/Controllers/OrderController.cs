using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MMSystem.Models;
using MMSystem.Models.ViewModels;

namespace MMSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly TGManagementLLCContext _context;

        public OrderController(TGManagementLLCContext context)
        {
            _context = context;
        }

        // GET: Order
        public IActionResult Index()
        {
            Owner Owner = new Owner();
            var Orders = (from o in _context.Order
                          join h in _context.ZHCart on o.CartId equals h.CartId
                          join c in _context.Customer on o.CustomerId equals c.CustomerId
                          select new OrderListIndexVM() {
                              Order = o,
                              ZHCart = h,
                              Customer = c
                         });
            //for displaying the order info/summary, Just display the order #, cart total, date, and maybe item count
            return View(Orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Cart)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        //public IActionResult Create()
        //{
        //    ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "CartId");
        //    ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "Address");
        //    return View();
        //}

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("OrderId,CustomerId,CartId,CreatedOn")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "CartId", order.CartId);
        //    ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "Address", order.CustomerId);
        //    return View(order);
        //}

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "CartId", order.CartId);
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "Address", order.CustomerId);
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,CartId,CreatedOn")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CartId"] = new SelectList(_context.Cart, "CartId", "CartId", order.CartId);
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "Address", order.CustomerId);
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Cart)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
