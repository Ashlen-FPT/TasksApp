using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TasksApp.Data;
using TasksApp.Models;

namespace TasksApp.Controllers
{
    public class TemplateTasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TemplateTasksController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context; 
            _userManager = userManager;
        }

        // GET: TemplateTasks
        public async Task<IActionResult> Index()
        {
            return View(await _context.TemplateTasks.ToListAsync());
        }

        // GET: TemplateTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateTask = await _context.TemplateTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateTask == null)
            {
                return NotFound();
            }

            return View(templateTask);
        }

        // GET: TemplateTasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TemplateTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Schedule,DateCreated,UserEmail")] TemplateTask templateTask)
        {
            if (ModelState.IsValid)
            {
               
                _context.Add(templateTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(templateTask);
        }

        // GET: TemplateTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateTask = await _context.TemplateTasks.FindAsync(id);
            if (templateTask == null)
            {
                return NotFound();
            }
            return View(templateTask);
        }

        // POST: TemplateTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Schedule,DateCreated,UserEmail")] TemplateTask templateTask)
        {
            if (id != templateTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(templateTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TemplateTaskExists(templateTask.Id))
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
            return View(templateTask);
        }

        // GET: TemplateTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateTask = await _context.TemplateTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateTask == null)
            {
                return NotFound();
            }

            return View(templateTask);
        }

        // POST: TemplateTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var templateTask = await _context.TemplateTasks.FindAsync(id);
            _context.TemplateTasks.Remove(templateTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TemplateTaskExists(int id)
        {
            return _context.TemplateTasks.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetDaily()
        {

            return Json(new { data = _context.TemplateTasks.Where(s => s.Schedule == "Daily").ToList() });

        }

        [HttpGet]
        public IActionResult GetWeekly()
        {

            return Json(new { data = _context.TemplateTasks.Where(s => s.Schedule == "Weekly").ToList() });

        }

        [HttpGet]
        public IActionResult GetMonthly()
        {

            return Json(new { data = _context.TemplateTasks.Where(s => s.Schedule == "Monthly").ToList() });

        }

        [HttpPost]
        public async Task<IActionResult> AddTask(string Desc, string Schedule)
        {

            var Task = new TemplateTask 
            { 
                Description = Desc,
                Schedule = Schedule,
                DateCreated = DateTime.Now,
                UserEmail = User.Identity.Name
            
            };

            _context.Add(Task);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task added!" });

        }

        [HttpPut]
        public async Task<IActionResult> EditTask(int Id, string Desc, string Schedule)
        {

            var templateTask = await _context.TemplateTasks.FindAsync(Id);

            templateTask.Description = Desc;
            templateTask.Schedule = Schedule;

            _context.Update(templateTask);
            
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task Updated!" });

        }

        [HttpGet]
        public async Task<IActionResult> GetTaskAsync(int id)
        {
            var templateTask = await _context.TemplateTasks.FindAsync(id);
            return Json(new { data = templateTask });

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var templateTask = await _context.TemplateTasks.FindAsync(id);
            _context.TemplateTasks.Remove(templateTask);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Task deleted!" });
        }

        #endregion
    }
}
