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

                foreach (var task in TemplateTasks)
                {

                    var Task = new Maintenance
                    {
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Schedule = task.Schedule,
                    };

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
                            DateTaskCompleted = new DateTime(),
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

            foreach (var item in tasks)
            {
                var status = tasks.All(c => c.Ok == false);
                {
                    task.Status = "Do-Checklist";
                    await _context.SaveChangesAsync();
                }
            }
            await _context.SaveChangesAsync();
            return Json(new { data = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date)});

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {

            var task = _context.Maintenances.Find(id);
            task.Ok = true;
            //task.Not = true;
            task.DateTaskCompleted = DateTime.Now;
            task.User = User.Identity.Name;
            //task.Status = "Partially Completed";

            var date = task.DateCreated;

            var tasks = _context.Maintenances.Where(d => d.DateCreated == date).ToList();
            //bool status = tasks.All(c => c.IsDone == false);
            foreach (var item in tasks)
            {
                //if (item.Status == null)
                //{
                //    task.Status = "Do-CheckList";
                //    await _context.SaveChangesAsync();
                //}

               if (item.Ok == false)
                {
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Task Completed!" });
                }

                else
                {

                    //bool statuses = tasks.All(c => c.IsDone == false);
                    //{
                    //    task.Status = "Do-Checklist";
                    //    await _context.SaveChangesAsync();
                    //}

                    bool completeTasks = tasks.All(c => c.Ok == true);
                    {
                        if (completeTasks == false)
                        {
                           // task.Status = "Partially Completed";
                            await _context.SaveChangesAsync();

                            return Json(new { success = true, message = "Task Completed!" });
                        }
                        else if (completeTasks == true)
                        {
                            task.TasksCompleted = true;
                            task.DateAllTaskCompleted = DateTime.Now;
                           // task.Status = "Completed";
                        }
                    }

                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "All Tasks Completed!" });
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
                    item.DateAllTaskCompleted = DateTime.Now;
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
