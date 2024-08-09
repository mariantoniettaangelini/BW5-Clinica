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
    [Authorize(Roles = "Vet")]
    public class RecoveriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecoveriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Recoveries
        public async Task<IActionResult> Index()
        {
            return View(await _context.Shelters.ToListAsync());
        }

        // GET: Recoveries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recovery = await _context.Shelters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recovery == null)
            {
                return NotFound();
            }

            return View(recovery);
        }

        // GET: Recoveries/Create
        public IActionResult Create()
        {
            ViewBag.AnimalTypes = new SelectList(new[] { "Cane", "Gatto", "Cavallo", "Topo", "Pappagallo" });
            return View();
        }

        // POST: Recoveries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RegistrationDate,Name,Type,CoatColor,BirthDate,Microchip,AdmissionDate,DischargeDate,PhotoUrl")] Recovery recovery)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recovery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recovery);
        }

        // GET: Recoveries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recovery = await _context.Shelters.FindAsync(id);
            if (recovery == null)
            {
                return NotFound();
            }
            ViewBag.AnimalTypes = new SelectList(new[] { "Cane", "Gatto", "Cavallo", "Topo", "Pappagallo" });
            return View(recovery);
        }

        // POST: Recoveries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RegistrationDate,Name,Type,CoatColor,BirthDate,Microchip,AdmissionDate,DischargeDate,PhotoUrl")] Recovery recovery)
        {
            if (id != recovery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recovery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecoveryExists(recovery.Id))
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
            ViewBag.AnimalTypes = new SelectList(new[] { "Cane", "Gatto", "Cavallo", "Topo", "Pappagallo" });
            return View(recovery);
        }

        // GET: Recoveries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recovery = await _context.Shelters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recovery == null)
            {
                return NotFound();
            }

            return View(recovery);
        }

        // POST: Recoveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recovery = await _context.Shelters.FindAsync(id);
            if (recovery != null)
            {
                _context.Shelters.Remove(recovery);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecoveryExists(int id)
        {
            return _context.Shelters.Any(e => e.Id == id);
        }

        // GET: Recoveries/RecoveryActive
        public async Task<IActionResult> RecoveryActive()
        {
            var activeRecoveries = await _context.Shelters
                .Where(s => s.DischargeDate == null)
                .ToListAsync();
            return View(activeRecoveries);
        }

        // GET: Recoveries/GetActiveShelters
        public async Task<IActionResult> GetActiveShelters(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Shelters.AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(s =>
                    (s.AdmissionDate >= startDate.Value && s.AdmissionDate <= endDate.Value) ||
                    (s.DischargeDate.HasValue && s.DischargeDate.Value >= startDate.Value && s.DischargeDate.Value <= endDate.Value) ||
                    (s.AdmissionDate <= endDate.Value && (s.DischargeDate == null || s.DischargeDate >= startDate.Value)));
            }

            var activeShelters = await query
                .Select(s => new
                {
                    Name = s.Name,
                    Type = s.Type,
                    CoatColor = s.CoatColor,
                    AdmissionDate = s.AdmissionDate,
                    DischargeDate = s.DischargeDate,
                    PhotoUrl = s.PhotoUrl
                })
                .ToListAsync();

            return Json(activeShelters);
        }

        // GET: Recoveries/Search
        public IActionResult Search()
        {
            return View();
        }

        // GET: Recoveries/SearchByMicrochip
        public async Task<IActionResult> SearchByMicrochip(string microchip)
        {
            if (string.IsNullOrEmpty(microchip))
            {
                return Json(new { found = false, message = "Numero di microchip non fornito." });
            }

            var animal = await _context.Shelters
                .Where(s => s.Microchip == microchip && s.DischargeDate == null)
                .Select(s => new
                {
                    s.Name,
                    s.Type,
                    s.CoatColor,
                    s.AdmissionDate,
                    s.PhotoUrl
                })
                .FirstOrDefaultAsync();

            if (animal == null)
            {
                return Json(new { found = false, message = "Animale non trovato o non ricoverato." });
            }

            return Json(new { found = true, animal });
        }
    }
}
