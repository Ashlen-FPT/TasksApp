using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TasksApp.Data;
using TasksApp.Models;

namespace TasksApp.Controllers
{
    public class DailyWeighsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyWeighsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DailyWeighs
        public async Task<IActionResult> Index()
        {
            return View(await _context.DailyWeighs.ToListAsync());
        }

        // GET: DailyWeighs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyWeigh = await _context.DailyWeighs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyWeigh == null)
            {
                return NotFound();
            }

            return View(dailyWeigh);
        }

        // GET: DailyWeighs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DailyWeighs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Time,username,Gross,Tare,Net,Observation")] DailyWeigh dailyWeigh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dailyWeigh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dailyWeigh);
        }

        // GET: DailyWeighs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyWeigh = await _context.DailyWeighs.FindAsync(id);
            if (dailyWeigh == null)
            {
                return NotFound();
            }
            return View(dailyWeigh);
        }

        // POST: DailyWeighs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Time,username,Gross,Tare,Net,Observation")] DailyWeigh dailyWeigh)
        {
            if (id != dailyWeigh.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dailyWeigh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DailyWeighExists(dailyWeigh.Id))
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
            return View(dailyWeigh);
        }

        // GET: DailyWeighs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyWeigh = await _context.DailyWeighs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyWeigh == null)
            {
                return NotFound();
            }

            return View(dailyWeigh);
        }

        // POST: DailyWeighs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dailyWeigh = await _context.DailyWeighs.FindAsync(id);
            _context.DailyWeighs.Remove(dailyWeigh);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DailyWeighExists(int id)
        {
            return _context.DailyWeighs.Any(e => e.Id == id);
        }
    }
}
