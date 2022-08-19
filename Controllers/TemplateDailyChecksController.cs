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
    public class TemplateDailyChecksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TemplateDailyChecksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TemplateDailyChecks
        public async Task<IActionResult> Index()
        {
            return View(await _context.TemplateDailyChecks.ToListAsync());
        }

        // GET: TemplateDailyChecks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateDailyCheck = await _context.TemplateDailyChecks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateDailyCheck == null)
            {
                return NotFound();
            }

            return View(templateDailyCheck);
        }

        // GET: TemplateDailyChecks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TemplateDailyChecks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HeadNo,Heading,Description,DateCreated")] TemplateDailyCheck templateDailyCheck)
        {
            if (ModelState.IsValid)
            {
                _context.Add(templateDailyCheck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(templateDailyCheck);
        }

        // GET: TemplateDailyChecks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateDailyCheck = await _context.TemplateDailyChecks.FindAsync(id);
            if (templateDailyCheck == null)
            {
                return NotFound();
            }
            return View(templateDailyCheck);
        }

        // POST: TemplateDailyChecks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HeadNo,Heading,Description,DateCreated")] TemplateDailyCheck templateDailyCheck)
        {
            if (id != templateDailyCheck.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(templateDailyCheck);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TemplateDailyCheckExists(templateDailyCheck.Id))
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
            return View(templateDailyCheck);
        }

        // GET: TemplateDailyChecks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateDailyCheck = await _context.TemplateDailyChecks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateDailyCheck == null)
            {
                return NotFound();
            }

            return View(templateDailyCheck);
        }

        // POST: TemplateDailyChecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var templateDailyCheck = await _context.TemplateDailyChecks.FindAsync(id);
            _context.TemplateDailyChecks.Remove(templateDailyCheck);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TemplateDailyCheckExists(int id)
        {
            return _context.TemplateDailyChecks.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetDailyCheck()
        {
            return Json(new { data = _context.TemplateDailyChecks.ToList() });

        }

        [HttpPost]
        public async Task<IActionResult> AddDailyCheck(int H_No, string Desc, string Head, bool Sub)
        {

            var dailyCheck = new TemplateDailyCheck
            {
                HeadNo = H_No,
                Heading = Head,
                Description = Desc,
                DateCreated = DateTime.Now,
                SubItems = Sub
            };

            _context.Add(dailyCheck);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Sub-Task added!" });
        }

        [HttpPost]
        public async Task<IActionResult> AddSubItem(int id, string Desc, string Head)
        {

            var dailyCheck = new TemplateDailyCheck
            {
                Heading = Head,
                Description = Desc,  
            };

            var q = from temp in _context.TemplateDailyChecks select temp.Heading;
            ViewBag.Heading = q;
            //ViewData.["HeadingId"] = new SelectList(_context.TemplateDailyChecks, "Heading", "HeadNo");
            _context.Add(dailyCheck);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Sub-Task added!" });

        }

        [HttpPut]
        public async Task<IActionResult> EditDailyCheck(int Id, int H_No, string Desc, string Head, bool Sub)
        {

            var templateDailyCheck = await _context.TemplateDailyChecks.FindAsync(Id);

            templateDailyCheck.HeadNo = H_No;
            templateDailyCheck.Description = Desc;
            templateDailyCheck.Heading = Head;
            templateDailyCheck.SubItems = Sub;

            _context.Update(templateDailyCheck);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Sub-Task Updated!" });

        }

        //[HttpGet]
        //public async Task<IActionResult> GetBobCatAsync(int id)
        //{
        //    var templateBobcat = await _context.TemplateBobCat.FindAsync(id);
        //    return Json(new { data = templateBobcat });

        //}

        [HttpDelete]
        public async Task<IActionResult> DeleteDailyCheck(int id)
        {
            var templateDailyCheck = await _context.TemplateDailyChecks.FindAsync(id);
            _context.TemplateDailyChecks.Remove(templateDailyCheck);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Sub-Task deleted!" });
        }

        #endregion
    }
}
