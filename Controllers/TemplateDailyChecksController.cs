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
            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Read,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateDailyCheck",
                OldData = "Read TemplateDailyCheck",
                NewData = null
            };

            _context.Logs.Add(log);
            _context.SaveChanges();

            return Json(new { data = _context.TemplateDailyChecks.ToList() });

        }

        [HttpPost]
        public async Task<IActionResult> AddDailyCheck(int H_No, string Check, string Head, string Desc)
        {

            var dailyCheck = new TemplateDailyCheck
            {
                HeadNo = H_No,
                Heading = Head,
                Checklist = Check,
                Description = Desc,
                UserEmail = User.Identity.Name
            };

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Created,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateDailyCheck",
                OldData = "New Task",
                NewData = $"{ "Heading No: " + dailyCheck.HeadNo + "Heading: " + dailyCheck.Heading + "Checklist: " + dailyCheck.Checklist + "Description: " + dailyCheck.Description + "UserEmail: " + dailyCheck.UserEmail}"
            };

            _context.Add(dailyCheck);
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Sub-Task added!" });
        }


        //GET: DailyChecksSubs/Create
        //public IActionResult AddSubItem()
        //{
        //    ViewData["HeadingId"] = new SelectList(_context.TemplateDailyChecks, "Id", "Heading");
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> AddSubItem(int Id, string Desc, string Head)
        {
            var dailyCheck = new TemplateDailyCheck
            {
                Heading = Head,
                Description = Desc,
                DateCreated = DateTime.Now,
                UserEmail = User.Identity.Name
            };

            //ViewData["HeadingId"] = new SelectList(_context.TemplateDailyChecks, "Id", "Heading", dailyCheck.HeadingId);
            _context.Add(dailyCheck);
            await _context.SaveChangesAsync();

            return View();

        }

        [HttpPut]
        public async Task<IActionResult> EditDailyCheck(int Id, int H_No, string Desc, string Head, bool Sub)
        {
            var existingHeadNo = _context.TemplateDailyChecks.Find(Id).HeadNo;
            var existingDescription = _context.TemplateDailyChecks.Find(Id).HeadNo;
            var existingHeading = _context.TemplateDailyChecks.Find(Id).HeadNo;
            var existingSubItems = _context.TemplateDailyChecks.Find(Id).SubItems;

            var templateDailyCheck = await _context.TemplateDailyChecks.FindAsync(Id);

            templateDailyCheck.HeadNo = H_No;
            templateDailyCheck.Description = Desc;
            templateDailyCheck.Heading = Head;
            templateDailyCheck.SubItems = Sub;

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Updated,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateDailyCheck",
                OldData = $"{ "Heading No: " + existingHeadNo + "Heading: " + existingHeading + "Description: " + existingDescription + "Sub Items: " + existingSubItems}",
                NewData = $"{ "Heading No: " + templateDailyCheck.HeadNo + "Heading: " + templateDailyCheck.Heading +"Description: " + templateDailyCheck.Description + "Sub Items: " + templateDailyCheck.SubItems}"
            };

            _context.Update(templateDailyCheck);
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
        public async Task<IActionResult> DeleteDailyCheck(int id)
        {
            var templateDailyCheck = await _context.TemplateDailyChecks.FindAsync(id);

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Deleted,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateBobCat",
                OldData = $"{ "Heading No: " + templateDailyCheck.HeadNo + "Heading: " + templateDailyCheck.Heading + "Description: " + templateDailyCheck.Description + "Sub Items: " + templateDailyCheck.SubItems}",
                NewData = "Task Removed"
            };
            _context.TemplateDailyChecks.Remove(templateDailyCheck);
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Sub-Task deleted!" });
        }

        #endregion
    }
}
