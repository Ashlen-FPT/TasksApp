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
    public class DailyChecksSubsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyChecksSubsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DailyChecksSubs
        public async Task<IActionResult> Index()
        {
            return View(await _context.DailyChecksSubs.ToListAsync());
        }

        // GET: DailyChecksSubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyChecksSub = await _context.DailyChecksSubs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyChecksSub == null)
            {
                return NotFound();
            }

            return View(dailyChecksSub);
        }

        // GET: DailyChecksSubs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DailyChecksSubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Heading,Description")] DailyChecksSub dailyChecksSub)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dailyChecksSub);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dailyChecksSub);
        }

        // GET: DailyChecksSubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyChecksSub = await _context.DailyChecksSubs.FindAsync(id);
            if (dailyChecksSub == null)
            {
                return NotFound();
            }
            return View(dailyChecksSub);
        }

        // POST: DailyChecksSubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Heading,Description")] DailyChecksSub dailyChecksSub)
        {
            if (id != dailyChecksSub.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dailyChecksSub);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DailyChecksSubExists(dailyChecksSub.Id))
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
            return View(dailyChecksSub);
        }

        // GET: DailyChecksSubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyChecksSub = await _context.DailyChecksSubs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyChecksSub == null)
            {
                return NotFound();
            }

            return View(dailyChecksSub);
        }

        // POST: DailyChecksSubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dailyChecksSub = await _context.DailyChecksSubs.FindAsync(id);
            _context.DailyChecksSubs.Remove(dailyChecksSub);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DailyChecksSubExists(int id)
        {
            return _context.DailyChecksSubs.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpPost]
        public async Task<IActionResult> AddSubItem(string Desc, string Head)
        {
            var dailyCheck = new DailyChecksSub
            {
                Heading = Head,
                Description = Desc,
            };

            ViewBag.Heading = new SelectList(_context.TemplateDailyChecks, "Heading", "Heading", dailyCheck.Heading);
            _context.Add(dailyCheck);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Sub-Task added!" });
        }
        #endregion
    }
}
