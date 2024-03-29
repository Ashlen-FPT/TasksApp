﻿using System;
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
    public class TemplateMainsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TemplateMainsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TemplateMains
        public async Task<IActionResult> Index()
        {
            return View(await _context.TemplateMains.ToListAsync());
        }

        // GET: TemplateMains/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateMain = await _context.TemplateMains
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateMain == null)
            {
                return NotFound();
            }

            return View(templateMain);
        }

        // GET: TemplateMains/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TemplateMains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Schedule,DateCreated,UserEmail")] TemplateMain templateMain)
        {
            if (ModelState.IsValid)
            {
                _context.Add(templateMain);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(templateMain);
        }

        // GET: TemplateMains/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateMain = await _context.TemplateMains.FindAsync(id);
            if (templateMain == null)
            {
                return NotFound();
            }
            return View(templateMain);
        }

        // POST: TemplateMains/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Schedule,DateCreated,UserEmail")] TemplateMain templateMain)
        {
            if (id != templateMain.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(templateMain);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TemplateMainExists(templateMain.Id))
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
            return View(templateMain);
        }

        // GET: TemplateMains/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateMain = await _context.TemplateMains
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateMain == null)
            {
                return NotFound();
            }

            return View(templateMain);
        }

        // POST: TemplateMains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var templateMain = await _context.TemplateMains.FindAsync(id);
            _context.TemplateMains.Remove(templateMain);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TemplateMainExists(int id)
        {
            return _context.TemplateMains.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetDaily()
        {
            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Read,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateMain",
                OldData = "Read TemplateMain",
                NewData = null
            };

            _context.Logs.Add(log);
            _context.SaveChanges();

            return Json(new { data = _context.TemplateMains.Where(s => s.Schedule == "Daily").ToList() });

        }

        [HttpPost]
        public async Task<IActionResult> AddTask(string Desc, string Schedule, string Day, string Month, string TType, string Annual, string Quarter, string Bi_Annually, string ChekList)
        {

            var Task = new TemplateMain
            {
                Description = Desc,
                Schedule = Schedule,
                DateCreated = DateTime.Now,
                UserEmail = User.Identity.Name,
            };
            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Created,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateMains",
                OldData = "New Task",
                NewData = $"{ "Description: " + Task.Description + "Schedule: " + Task.Schedule + "Date Created: " + Task.DateCreated + "UserEmail: " + Task.UserEmail}"
            };

            _context.Add(Task);
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task added!" });

        }

        [HttpPut]
        public async Task<IActionResult> EditTask(int Id, string Desc, string Schedule)
        {
            var existingDescription = _context.TemplateMains.Find(Id).Description;
            var existingSchedule = _context.TemplateMains.Find(Id).Schedule;

            var templateMain = await _context.TemplateMains.FindAsync(Id);

            templateMain.Description = Desc;
            templateMain.Schedule = Schedule;

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Updated,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateItem",
                OldData = $"{  " Description: " + existingDescription + " Schedule: " + existingSchedule}",
                NewData = $"{  " Description: " + templateMain.Description + " Schedule: " + templateMain.Schedule}"
            };

            _context.Update(templateMain);
            _context.Logs.Add(log);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task Updated!" });

        }

        [HttpGet]
        public async Task<IActionResult> GetTaskAsync(int id)
        {
            var templateMains = await _context.TemplateMains.FindAsync(id);
            return Json(new { data = templateMains });

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var templateMain = await _context.TemplateMains.FindAsync(id);

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Deleted,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateItem",
                OldData = $"{ "Description: " + templateMain.Description + "Schedule: " + templateMain.Schedule + "Date Created: " + templateMain.DateCreated + "UserEmail: " + templateMain.UserEmail}",
                NewData = "Task Removed"
            };
            _context.TemplateMains.Remove(templateMain);
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Task deleted!" });
        }

        #endregion
    }
}
