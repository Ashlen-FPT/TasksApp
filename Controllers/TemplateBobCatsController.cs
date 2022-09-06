using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TasksApp.Data;
using TasksApp.Enums;
using TasksApp.Models;

namespace TasksApp.Controllers
{
    public class TemplateBobCatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TemplateBobCatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TemplateBobCats
        public async Task<IActionResult> Index()
        {
            return View(await _context.TemplateBobCat.ToListAsync());
        }

        // GET: TemplateBobCats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateBobCat = await _context.TemplateBobCat
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateBobCat == null)
            {
                return NotFound();
            }

            return View(templateBobCat);
        }

        // GET: TemplateBobCats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TemplateBobCats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Heading,Description,DateCreated")] TemplateBobCat templateBobCat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(templateBobCat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(templateBobCat);
        }

        // GET: TemplateBobCats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateBobCat = await _context.TemplateBobCat.FindAsync(id);
            if (templateBobCat == null)
            {
                return NotFound();
            }
            return View(templateBobCat);
        }

        // POST: TemplateBobCats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Heading,Description,DateCreated")] TemplateBobCat templateBobCat)
        {
            if (id != templateBobCat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(templateBobCat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TemplateBobCatExists(templateBobCat.Id))
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
            return View(templateBobCat);
        }

        // GET: TemplateBobCats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateBobCat = await _context.TemplateBobCat
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateBobCat == null)
            {
                return NotFound();
            }

            return View(templateBobCat);
        }

        // POST: TemplateBobCats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var templateBobCat = await _context.TemplateBobCat.FindAsync(id);
            _context.TemplateBobCat.Remove(templateBobCat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TemplateBobCatExists(int id)
        {
            return _context.TemplateBobCat.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetBobCats()
        {
            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Read,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateBobCat",
                OldData = "Read TemplateBobCat",
                NewData = null
            };

            _context.Logs.Add(log);
            _context.SaveChanges();

            return Json(new { data = _context.TemplateBobCat.ToList() });

        }

        [HttpGet]
        public async Task<IActionResult> GetBobCatAsync(int id)
        {
            var templateTask = await _context.TemplateBobCat.FindAsync(id);
            return Json(new { data = templateTask });

        }

        [HttpPost]
        public async Task<IActionResult> AddBobCat(int T_No, string Desc, string Head)
        {

            var bobCat = new TemplateBobCat
            {
                TaskNo = T_No,
                Heading = Head,
                Description = Desc,
                DateCreated = DateTime.Now,

            };

            var log = new Logs
            {
                UserName= User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Created,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateBobCat",
                OldData = "New Task",
                NewData= $"{ "Task No: " + bobCat.TaskNo + "Description: " + bobCat.Description + "Heading: " + bobCat.Heading}"
            };

            _context.Add(bobCat);
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Sub-Task added!" });

        }

        [HttpPut]
        public async Task<IActionResult> EditBobCat(int Id, int TaskNo, string Desc, string Head)
        {
            var existingTaskNo = _context.TemplateBobCat.Find(Id).TaskNo;
            var existingDesc = _context.TemplateBobCat.Find(Id).Description;
            var existingHead = _context.TemplateBobCat.Find(Id).Heading;
            var templateBobcat = await _context.TemplateBobCat.FindAsync(Id);

            templateBobcat.TaskNo = TaskNo;
            templateBobcat.Description = Desc;
            templateBobcat.Heading = Head;


            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity= User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Updated,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateBobCat",
                OldData = $"{ "Task No: "+ existingTaskNo + " Description: "+ existingDesc + " Heading: " + existingHead}",
                NewData = $"{ "Task No: " + templateBobcat.TaskNo + "Description: " + templateBobcat.Description + "Heading: " + templateBobcat.Heading}"
            };
            
            _context.Update(templateBobcat);
            _context.Logs.Add(log);

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
        public async Task<IActionResult> DeleteBobCat(int id)
        {

            var templateBobcat = await _context.TemplateBobCat.FindAsync(id);

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Deleted,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateBobCat",
                OldData = $"{ "Task No: " + templateBobcat.TaskNo + "Description: " + templateBobcat.Description + "Heading: " + templateBobcat.Heading}",
                NewData = "Task Removed"
            };

            _context.TemplateBobCat.Remove(templateBobcat);
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Sub-Task deleted!" });
        }

        #endregion
    }
}
