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
    public class MReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MReports
        public async Task<IActionResult> Index()
        {
            return View(await _context.mreports.ToListAsync());
        }

        // GET: MReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mReport = await _context.mreports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mReport == null)
            {
                return NotFound();
            }

            return View(mReport);
        }

        // GET: MReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Checklist,DateCreated,DateCompleted,Status,AssignedTo,UserName,TaskCompleted")] MReport mReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mReport);
        }

        // GET: MReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mReport = await _context.mreports.FindAsync(id);
            if (mReport == null)
            {
                return NotFound();
            }
            return View(mReport);
        }

        // POST: MReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Checklist,DateCreated,DateCompleted,Status,AssignedTo,UserName,TaskCompleted")] MReport mReport)
        {
            if (id != mReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MReportExists(mReport.Id))
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
            return View(mReport);
        }

        // GET: MReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mReport = await _context.mreports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mReport == null)
            {
                return NotFound();
            }

            return View(mReport);
        }

        // POST: MReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mReport = await _context.mreports.FindAsync(id);
            _context.mreports.Remove(mReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MReportExists(int id)
        {
            return _context.mreports.Any(e => e.Id == id);
        }
    }
}
