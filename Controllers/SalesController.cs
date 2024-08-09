using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationDbContex.Data;
using BW5.Models;
using Microsoft.AspNetCore.Authorization;

namespace BW5.Controllers
{
    [Authorize(Roles = "Farmacista")]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sales.Include(s => s.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales
                .Include(s => s.Product)
                .ThenInclude(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sales == null)
            {
                return NotFound();
            }

            return View(sales);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerFiscalCode,ProductId,PrescriptionNumber,SaleDate")] Sales sales)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sales);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", sales.ProductId);
            return View(sales);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Product)
                    .ThenInclude(p => p.Drawer)
                        .ThenInclude(d => d.Cabinet)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            var selectedCabinetId = sale.Product?.Drawer?.CabinetId;
            var selectedDrawerId = sale.Product?.DrawerId;

            var cabinets = await _context.Cabinets.ToListAsync();
            var drawers = selectedCabinetId.HasValue
                ? await _context.Drawers.Where(d => d.CabinetId == selectedCabinetId.Value).ToListAsync()
                : new List<Drawer>();
            var products = selectedDrawerId.HasValue
                ? await _context.Products.Where(p => p.DrawerId == selectedDrawerId.Value).ToListAsync()
                : new List<Product>();

            ViewBag.Cabinets = cabinets;
            ViewBag.Drawers = drawers;
            ViewBag.Products = products;

            return View(sale);
        }

        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerFiscalCode,ProductId,PrescriptionNumber,SaleDate")] Sales sale)
        {
            if (id != sale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesExists(sale.Id))
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

            var selectedProduct = await _context.Products
                .Include(p => p.Drawer)
                    .ThenInclude(d => d.Cabinet)
                .FirstOrDefaultAsync(p => p.Id == sale.ProductId);

            var selectedCabinetId = selectedProduct?.Drawer?.CabinetId;
            var selectedDrawerId = selectedProduct?.DrawerId;

            var cabinets = await _context.Cabinets.ToListAsync();
            var drawers = selectedCabinetId.HasValue
                ? await _context.Drawers.Where(d => d.CabinetId == selectedCabinetId.Value).ToListAsync()
                : new List<Drawer>();
            var products = selectedDrawerId.HasValue
                ? await _context.Products.Where(p => p.DrawerId == selectedDrawerId.Value).ToListAsync()
                : new List<Product>();

            ViewBag.Cabinets = cabinets;
            ViewBag.Drawers = drawers;
            ViewBag.Products = products;

            return View(sale);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sales == null)
            {
                return NotFound();
            }

            return View(sales);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sales = await _context.Sales.FindAsync(id);
            if (sales != null)
            {
                _context.Sales.Remove(sales);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }

        // GET: Sales/CustomReport
        public async Task<IActionResult> CustomReport()
        {
            // Logica per generare il report personalizzato
            var reportData = await _context.Sales
                .Include(s => s.Product)
                .ThenInclude(p => p.Supplier)
                .ToListAsync();

            return View(reportData);
        }

        // GET: Sales/DrawerAndCabinetPartial
        public async Task<IActionResult> DrawerAndCabinetPartial()
        {
            // Logica per ottenere i dati di Drawer e Cabinet
            var drawerAndCabinetData = await _context.Drawers
                .Include(d => d.Cabinet)
                .ToListAsync();

            return PartialView("_DrawerAndCabinetPartial", drawerAndCabinetData);
        }

        // GET: Sales/GetSalesByDate
        public async Task<IActionResult> GetSalesByDate(DateTime date)
        {
            var sales = await _context.Sales
                .Include(s => s.Product)
                .Where(s => s.SaleDate.Date == date.Date)
                .ToListAsync();

            return PartialView("_SalesByDatePartial", sales);
        }

        // GET: Sales/GetSalesByCustomer
        public async Task<IActionResult> GetSalesByCustomer(string fiscalCode)
        {
            var sales = await _context.Sales
                .Include(s => s.Product)
                .Where(s => s.CustomerFiscalCode == fiscalCode)
                .ToListAsync();

            return PartialView("_SalesByCustomerPartial", sales);
        }
    }
}
