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
    public class MaintenancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaintenancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Maintenances
        public async Task<IActionResult> Index()
        {
            return View(await _context.Maintenances.ToListAsync());
        }

        // GET: Maintenances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenance = await _context.Maintenances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenance == null)
            {
                return NotFound();
            }

            return View(maintenance);
        }

        // GET: Maintenances/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Maintenances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Ok,Not,Comments,Shift,UserName,Date")] Maintenance maintenance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maintenance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(maintenance);
        }

        // GET: Maintenances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenance = await _context.Maintenances.FindAsync(id);
            if (maintenance == null)
            {
                return NotFound();
            }
            return View(maintenance);
        }

        // POST: Maintenances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Ok,Not,Comments,Shift,UserName,Date")] Maintenance maintenance)
        {
            if (id != maintenance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maintenance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceExists(maintenance.Id))
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
            return View(maintenance);
        }

        // GET: Maintenances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenance = await _context.Maintenances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenance == null)
            {
                return NotFound();
            }

            return View(maintenance);
        }

        // POST: Maintenances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maintenance = await _context.Maintenances.FindAsync(id);
            _context.Maintenances.Remove(maintenance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceExists(int id)
        {
            return _context.Maintenances.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public async Task<IActionResult> GetTasksTodayAsync(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).ToList();


            if (TasksToday.Count == 0)
            {
                var TemplateTasks = _context.TemplateMains.Where(s => s.Schedule == "Daily").ToList();
                var last = TemplateTasks.LastOrDefault();

                foreach (var task in TemplateTasks)
                {

                    var Task = new Maintenance
                    {
                        Description = task.Description,
                        DateCreated = date,
                        DateAllCompleted = new DateTime(),
                        Schedule = task.Schedule,
                        Status = "Task : Incomplete"
                    };
                    if (task == last)
                    {
                        Task.Status = "Do-Checklist : Maintenances";
                    }

                    _context.Maintenances.Add(Task);

                }
            }

            if (TasksToday.Count > 0)
            {
                var TemplateTasks = _context.TemplateMains.Where(s => s.Schedule == "Daily").ToList();

                if (TemplateTasks.Count > TasksToday.Count)
                {
                    var result = TemplateTasks.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                    foreach (var item in result)
                    {
                        var Task = new Maintenance
                        {
                            Description = item.Description,
                            DateCreated = date,
                            Schedule = item.Schedule,
                            DateAllCompleted = new DateTime(),
                            Status = "Do-Checklist"
                        };

                        _context.Maintenances.Add(Task);
                    }

                }
            }

            await _context.SaveChangesAsync();

            return Json(new { data = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Daily")});
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var tasks = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).ToList();
            var task = _context.Maintenances.FirstOrDefault();

            //foreach (var item in tasks)
            //{
            //    var status = tasks.All(c => c.Ok == false);
            //    {
            //        task.Status = "Do-Checklist";
            //        await _context.SaveChangesAsync();
            //    }
            //}
            await _context.SaveChangesAsync();
            return Json(new { data = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date)});

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var Main_Task = _context.TemplateMains.ToList();
            var Maintenances = _context.Maintenances.ToList();
            var last = Main_Task.LastOrDefault();
            var count = Main_Task.Count();
            var DateCreation = new DateTime();
            var Ddate = _context.Maintenances.Find(id).DateCreated;
            var task = _context.Maintenances.Find(id);
            //Get Last Item & Change Status
            var items = Maintenances.Where((x, i) => i % count == count - 1);
            var ItemDate = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.DateCreated).FirstOrDefault();
            var ItemStatus = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Status).FirstOrDefault();
            var ItemId = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Id).FirstOrDefault();
            var ChangeStatus = _context.Maintenances.Find(ItemId);


            task.Ok = true;
            task.Not = false;
            task.DateCompleted = DateTime.Now;
            //task.DateAllCompleted = DateTime.Now;
            task.User = User.Identity.Name;
            //task.Status = "Partially Completed";
            task.Status = "Task : Completed";
            task.IsDone = true;
            DateCreation = task.DateCreated;
            //var date = task.DateCreated;

            if (ItemDate == Ddate)
            {
                ChangeStatus.Status = "Partially Completed : Maintenances";
            }

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Completed,
                DateTime = DateTime.Now,
                UpdatedTable = "Maintenance",
                OldData = null,
                NewData = "Task Completed"
            };

            _context.Logs.Add(log);
            _context.SaveChanges();

            var tasks = _context.Maintenances.Where(d => d.DateCreated == DateCreation).ToList();
            //bool status = tasks.All(c => c.IsDone == false);

            if (tasks.All(c => c.Ok == true))
            {

                if (ItemDate == Ddate)
                {
                    task.DateAllTaskCompleted = DateTime.Now;
                    ChangeStatus.Status = "Completed : Maintenances";
                }
            }

            foreach (var item in tasks)
            {

                if (item.Ok == false)
                {
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Task Completed!" });
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "All Tasks Completed!" });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteAllTasks(DateTime date)
        {
            var task = _context.Maintenances.Where(d => d.DateCreated == date).Where(t => t.Ok == true).ToList();

            foreach (var item in task)
            {
                if (item.Ok == true)
                {
                    item.TasksCompleted = true;
                    item.DateAllCompleted = DateTime.Now;
                }

            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "All Tasks Completed!" });
        }

        [HttpGet]
        public async Task<IActionResult> AddComment(int id, string comment)
        {


            var task = _context.Maintenances.Find(id);

            task.Comments = comment;


            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Comment added!" });

        }

        [HttpGet]
        public IActionResult GetTask(int id)
        {
            var task = _context.Maintenances.Find(id);
            return Json(task);

        }
        #endregion
    }
}
