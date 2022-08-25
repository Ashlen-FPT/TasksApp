using System.Linq;
using TasksApp.Data;
using TasksApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace TasksApp.Controllers
{
    public class TemplateItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TemplateItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TemplateItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.TemplateItem.ToListAsync());
        }

        // GET: TemplateItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateItems = await _context.TemplateItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateItems == null)
            {
                return NotFound();
            }

            return View(templateItems);
        }

        // GET: TemplateItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TemplateItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Schedule,DayOfWeek,DateCreated,UserEmail")] TemplateItems templateItems)
        {
            if (ModelState.IsValid)
            {
                _context.Add(templateItems);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(templateItems);
        }

        // GET: TemplateItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateItems = await _context.TemplateItem.FindAsync(id);
            if (templateItems == null)
            {
                return NotFound();
            }
            return View(templateItems);
        }

        // POST: TemplateItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Schedule,DayOfWeek,DateCreated,UserEmail")] TemplateItems templateItems)
        {
            if (id != templateItems.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(templateItems);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TemplateItemsExists(templateItems.Id))
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
            return View(templateItems);
        }

        // GET: TemplateItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateItems = await _context.TemplateItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateItems == null)
            {
                return NotFound();
            }

            return View(templateItems);
        }

        // POST: TemplateItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var templateItems = await _context.TemplateItem.FindAsync(id);
            _context.TemplateItem.Remove(templateItems);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TemplateItemsExists(int id)
        {
            return _context.TemplateItem.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetMon()
        {

            return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Monday").ToList() });

        }

        [HttpGet]
        public IActionResult GetTue()
        {

            return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Tuesday").ToList() });

        }

        [HttpGet]
        public IActionResult GetWed()
        {

            return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Wednesday").ToList() });

        }

        [HttpGet]
        public IActionResult GetThu()
        {
            return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Thursday").ToList() });

        }

        [HttpGet]
        public IActionResult GetFri()
        {
            return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Friday").ToList() });

        }

        [HttpGet]
        public IActionResult GetSat()
        {
            return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Saturday").ToList() });

        }

        [HttpGet]
        public IActionResult GetSun()
        {
            return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Sunday").ToList() });

        }

        [HttpPost]
        public async Task<IActionResult> AddTask(string Schedule, string Main)
        {

            var Task = new TemplateItems
            {
                Main = Main,
                //Description = Desc,
                Schedule = Schedule,
                DateCreated = DateTime.Now,
                UserEmail = User.Identity.Name

            };

            _context.Add(Task);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task added!" });

        }

        [HttpPost]
        public async Task<IActionResult> AddSubItem(int Id, string Description, string Main, string Schedule)
        {
            var dailyCheck = new TemplateItems
            {
                Main = Main,
                Description = Description,
                Schedule = Schedule,
                DateCreated = DateTime.Now,
                UserEmail = User.Identity.Name
            };

            //ViewData["HeadingId"] = new SelectList(_context.TemplateDailyChecks, "Id", "Heading", dailyCheck.HeadingId);
            _context.Add(dailyCheck);
            await _context.SaveChangesAsync();

            return View();

        }

        [HttpPut]
        public async Task<IActionResult> EditTask(int Id, string Desc, string Schedule, string Main)
        {

            var templateTask = await _context.TemplateItem.FindAsync(Id);

            templateTask.Main = Main;
            templateTask.Description = Desc;
            templateTask.Schedule = Schedule;

            _context.Update(templateTask);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task Updated!" });

        }

        [HttpGet]
        public async Task<IActionResult> GetTaskAsync(int id)
        {
            var templateTask = await _context.TemplateItem.FindAsync(id);
            return Json(new { data = templateTask });

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var templateTask = await _context.TemplateItem.FindAsync(id);
            _context.TemplateItem.Remove(templateTask);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Task deleted!" });
        }

        #endregion
    }
}
