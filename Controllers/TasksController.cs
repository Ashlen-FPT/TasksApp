using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksApp.Data;
using TasksApp.Models;

namespace TasksApp.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tasks.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Comments,Description,IsDone,DateTaskCompleted,TasksCompleted,DateAllTaskCompleted,DateCreated,User")] Tasks tasks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tasks);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comments,Description,IsDone,DateTaskCompleted,TasksCompleted,DateAllTaskCompleted,DateCreated,User")] Tasks tasks)
        {
            if (id != tasks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.Id))
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
            return View(tasks);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tasks = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        #region API Calls


        [HttpGet]
        public async Task<IActionResult> GetTasksTodayAsync(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.Tasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskType == "Tasks").ToList();


            if (TasksToday.Count == 0)
            {
                var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "Tasks").ToList();

                foreach (var task in TemplateTasks)
                {

                    var Task = new Tasks
                    { 
                       Description = task.Description,
                       DateCreated = date,
                       DateTaskCompleted = new DateTime(),
                       Schedule = task.Schedule,
                       TaskType = task.TaskType
                    };

                    _context.Tasks.Add(Task);

                }
            }

            if (TasksToday.Count > 0)
            {
                var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "Tasks").ToList();

                if (TemplateTasks.Count > TasksToday.Count)
                {
                    var result = TemplateTasks.Where(p => TasksToday.All(p2 => p2.Description != p.Description)).Where(s => s.TaskType == "Tasks");

                    foreach (var item in result)
                    {
                        var Task = new Tasks
                        {
                            Description = item.Description,
                            DateCreated = date,
                            Schedule = item.Schedule,
                            DateTaskCompleted = new DateTime(),
                            TaskType = item.TaskType
                        };

                        _context.Tasks.Add(Task);
                    }

                }
            }

            await _context.SaveChangesAsync();
            
            return Json(new { data = _context.Tasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "Tasks") });
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksWeekly(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var Day = oDate.DayOfWeek.ToString();

            var TasksToday = _context.Tasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule ==  "Weekly").Where(s => s.TaskType == "Tasks").ToList();


            if (TasksToday.Count == 0)
            {
                var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Weekly").Where(s => s.TaskType == "Tasks").Where(d => d.DayOfWeek == Day).ToList();

                foreach (var task in TemplateTasks)
                {

                    var Task = new Tasks
                    {
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Schedule = task.Schedule,
                        TaskType = task.TaskType

                    };

                    _context.Tasks.Add(Task);

                }

            }

            if (TasksToday.Count > 0)
            {
                var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Weekly").Where(s => s.TaskType == "Tasks").Where(d => d.DayOfWeek == Day).ToList();

                if (TemplateTasks.Count > TasksToday.Count)
                {
                    var result = TemplateTasks.Where(p => TasksToday.All(p2 => p2.Description != p.Description)).Where(s => s.TaskType == "Tasks");

                    foreach (var item in result)
                    {
                        var Task = new Tasks
                        {
                            Description = item.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = item.Schedule,
                            TaskType = item.TaskType
                        };

                        _context.Tasks.Add(Task);
                    }

                }

            }

            await _context.SaveChangesAsync();

            return Json(new { data = _context.Tasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Weekly").Where(s => s.TaskType == "Tasks") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksMonthly(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var firstDayOfMonth = new DateTime(oDate.Year, oDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            if (firstDayOfMonth == oDate)
            {
                var TasksToday = _context.Tasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskType == "Tasks").ToList();

                if(TasksToday.Count == 0)
                {
                    var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskType == "Tasks").ToList();

                    foreach (var task in TemplateTasks)
                    {

                        var Task = new Tasks
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            TaskType = task.TaskType
                        };

                        _context.Tasks.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskType == "Tasks").ToList();

                    if (TemplateTasks.Count > TasksToday.Count)
                    {
                        var result = TemplateTasks.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Tasks
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                TaskType = item.TaskType
                            };

                            _context.Tasks.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfMonth == oDate)
            {
                var TasksToday = _context.Tasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskType == "Tasks").ToList();

                if (TasksToday.Count == 0)
                {
                    var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskType == "Tasks").ToList();

                    foreach (var task in TemplateTasks)
                    {

                        var Task = new Tasks
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            TaskType = task.TaskType
                        };

                        _context.Tasks.Add(Task);

                    }

                }

                if (TasksToday.Count > 0)
                {
                    var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskType == "Tasks").ToList();

                    if (TemplateTasks.Count > TasksToday.Count)
                    {
                        var result = TemplateTasks.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Tasks
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                TaskType = item.TaskType
                            };

                            _context.Tasks.Add(Task);
                        }

                    }
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { data = _context.Tasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskType == "Tasks") });

        }

        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Tasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskType == "Tasks") });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {

            var task = _context.Tasks.Find(id);
            task.IsDone = true;
            task.DateTaskCompleted = DateTime.Now;
            task.User = User.Identity.Name;

            var date = task.DateCreated;


            var tasks = _context.Tasks.Where(d => d.DateCreated == date).ToList();

            foreach (var item in tasks)
            {

                if (item.IsDone == false)
                {
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Task Completed!" });
                }

                else
                {
                    
                    task.DateAllTaskCompleted = DateTime.Now;
                    task.TasksCompleted = true;
                }

            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task Completed!" });

        }

        [HttpGet]
        public async Task<IActionResult> AddComment(int id, string comment)
        {


            var task = _context.Tasks.Find(id);

            task.Comments = comment;


            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Comment added!" });

        }

        [HttpGet]
        public IActionResult GetTask(int id)
        {
            var task = _context.Tasks.Find(id);
            return Json(task);

        }

        #endregion
    }

}
