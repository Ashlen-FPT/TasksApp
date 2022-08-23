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
    public class BEsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BEsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BEs
        public async Task<IActionResult> Index()
        {
            return View(await _context.BEs.ToListAsync());
        }

        // GET: BEs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bE = await _context.BEs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bE == null)
            {
                return NotFound();
            }

            return View(bE);
        }

        // GET: BEs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BEs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("iD,Categories")] BE bE)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bE);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bE);
        }

        // GET: BEs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bE = await _context.BEs.FindAsync(id);
            if (bE == null)
            {
                return NotFound();
            }
            return View(bE);
        }

        // POST: BEs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("iD,Categories")] BE bE)
        {
            if (id != bE.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bE);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BEExists(bE.Id))
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
            return View(bE);
        }

        // GET: BEs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bE = await _context.BEs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bE == null)
            {
                return NotFound();
            }

            return View(bE);
        }

        // POST: BEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bE = await _context.BEs.FindAsync(id);
            _context.BEs.Remove(bE);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BEExists(int id)
        {
            return _context.BEs.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> AddEntity(string Category)
        {

            var Entity = new BE
            {
                Categories = Category

            };

            _context.Add(Entity);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Business Entity added!" });

        }
    }
}
