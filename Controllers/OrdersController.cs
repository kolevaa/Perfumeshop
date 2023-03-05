using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Perfumeshop.Data;
using Perfumeshop.Models;
using Microsoft.AspNetCore.Authorization;
using Perfumeshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
namespace Perfumeshop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly PerfumeshopContext _context;
        public OrdersController(PerfumeshopContext context)
        {
            _context = context;
        }

        // GET: Orders
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var perfumeshopContext = _context.Order.Include(o => o.Perfume).Include(o => o.User);
            return View(await perfumeshopContext.ToListAsync());
        }

        // GET: Orders/Details/5
      //  [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Perfume)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            ViewData["PerfumeId"] = new SelectList(_context.Perfume, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create([Bind("Id,Status,Location,UserId,PerfumeId")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Status = "Pending Approval";
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PerfumeId"] = new SelectList(_context.Perfume, "Id", "Name", order.PerfumeId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["PerfumeId"] = new SelectList(_context.Perfume, "Id", "Name", order.PerfumeId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", order.UserId);
            ViewBag.Status = order.Status;
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,UserId,PerfumeId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    order.Status = "Approved";
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["PerfumeId"] = new SelectList(_context.Perfume, "Id", "Title", order.PerfumeId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Perfume)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'PerfumeshopContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Admin,User")]
        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Orders(int id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            IQueryable<Order> orders = _context.Order.Where(x => x.UserId == id)
                .Include(e => e.Perfume)
                .Include(e => e.User);
            ViewBag.User = user.FullName;
            ViewBag.UserId = user.Id;
            await _context.SaveChangesAsync();
            return View(await orders.ToListAsync());
        }
       
    }
}
