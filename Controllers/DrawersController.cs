using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationDbContex.Data;
using BW5.Models;

namespace BW5.Controllers
{
    public class DrawersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DrawersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Drawers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Drawers.Include(d => d.Cabinet);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Drawers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drawer = await _context.Drawers
                .Include(d => d.Cabinet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drawer == null)
            {
                return NotFound();
            }

            return View(drawer);
        }

        // GET: Drawers/Create
        public IActionResult Create()
        {
            ViewData["CabinetId"] = new SelectList(_context.Cabinets, "Id", "Code");
            return View();
        }

        // POST: Drawers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,CabinetId")] Drawer drawer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(drawer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CabinetId"] = new SelectList(_context.Cabinets, "Id", "Code", drawer.CabinetId);
            return View(drawer);
        }

        // GET: Drawers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drawer = await _context.Drawers.FindAsync(id);
            if (drawer == null)
            {
                return NotFound();
            }
            ViewData["CabinetId"] = new SelectList(_context.Cabinets, "Id", "Code", drawer.CabinetId);
            return View(drawer);
        }

        // POST: Drawers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,CabinetId")] Drawer drawer)
        {
            if (id != drawer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drawer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrawerExists(drawer.Id))
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
            ViewData["CabinetId"] = new SelectList(_context.Cabinets, "Id", "Code", drawer.CabinetId);
            return View(drawer);
        }

        // GET: Drawers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drawer = await _context.Drawers
                .Include(d => d.Cabinet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drawer == null)
            {
                return NotFound();
            }

            return View(drawer);
        }

        // POST: Drawers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drawer = await _context.Drawers.FindAsync(id);
            if (drawer != null)
            {
                _context.Drawers.Remove(drawer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DrawerExists(int id)
        {
            return _context.Drawers.Any(e => e.Id == id);
        }
    }
}
