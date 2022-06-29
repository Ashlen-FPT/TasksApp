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
    public class PreTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PreTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PreTasks
        public async Task<IActionResult> Index()
        {
            return View(await _context.PreTasks.ToListAsync());
        }

        // GET: PreTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preTasks = await _context.PreTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (preTasks == null)
            {
                return NotFound();
            }

            return View(preTasks);
        }

        // GET: PreTasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PreTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Comments,Description,IsDone,DateTaskCompleted,TasksCompleted,DateAllTaskCompleted,DateCreated,User")] PreTasks preTasks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(preTasks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(preTasks);
        }

        // GET: PreTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preTasks = await _context.PreTasks.FindAsync(id);
            if (preTasks == null)
            {
                return NotFound();
            }
            return View(preTasks);
        }

        // POST: PreTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comments,Description,IsDone,DateTaskCompleted,TasksCompleted,DateAllTaskCompleted,DateCreated,User")] PreTasks preTasks)
        {
            if (id != preTasks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(preTasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreTasksExists(preTasks.Id))
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
            return View(preTasks);
        }

        // GET: PreTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preTasks = await _context.PreTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (preTasks == null)
            {
                return NotFound();
            }

            return View(preTasks);
        }

        // POST: PreTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var preTasks = await _context.PreTasks.FindAsync(id);
            _context.PreTasks.Remove(preTasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PreTasksExists(int id)
        {
            return _context.PreTasks.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public async Task<IActionResult> GetTasks(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.PreTasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskType == "PreTasks").ToList();


            if (TasksToday.Count == 0)
            {
                var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "PreTasks").ToList();

                foreach (var task in TemplateTasks)
                {

                    var preTask = new PreTasks
                    {
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Schedule = task.Schedule,
                        TaskType = task.TaskType
                    };

                    _context.PreTasks.Add(preTask);

                }
            }

            if (TasksToday.Count > 0)
            {
                var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "PreTasks").ToList();

                if (TemplateTasks.Count > TasksToday.Count)
                {
                    var result = TemplateTasks.Where(p => TasksToday.All(p2 => p2.Description != p.Description)).Where(s => s.TaskType == "PreTasks");

                    foreach (var item in result)
                    {
                        var preTask = new PreTasks
                        {
                            Description = item.Description,
                            DateCreated = date,
                            Schedule = item.Schedule,
                            DateTaskCompleted = new DateTime(),
                            TaskType = item.TaskType
                        };

                        _context.PreTasks.Add(preTask);
                    }

                }
            }

            await _context.SaveChangesAsync();

            return Json(new { data = _context.PreTasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "PreTasks") });
        }

        public async Task<IActionResult> AdminGetTasks(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.PreTasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskType == "PreTasks").ToList();


            if (TasksToday.Count == 0)
            {
                var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "PreTasks").ToList();

                foreach (var task in TemplateTasks)
                {

                    var preTask = new PreTasks
                    {
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Schedule = task.Schedule,
                        TaskType = task.TaskType
                    };

                    _context.PreTasks.Add(preTask);

                }
            }

            if (TasksToday.Count > 0)
            {
                var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "PreTasks").ToList();

                if (TemplateTasks.Count > TasksToday.Count)
                {
                    var result = TemplateTasks.Where(p => TasksToday.All(p2 => p2.Description != p.Description)).Where(s => s.TaskType == "PreTasks");

                    foreach (var item in result)
                    {
                        var preTask = new PreTasks
                        {
                            Description = item.Description,
                            DateCreated = date,
                            Schedule = item.Schedule,
                            DateTaskCompleted = new DateTime(),
                            TaskType = item.TaskType
                        };

                        _context.PreTasks.Add(preTask);
                    }

                }
            }

            await _context.SaveChangesAsync();

            return Json(new { data = _context.PreTasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "PreTasks") });
        }

        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.PreTasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskType == "PreTasks") });

        }

        [HttpGet]
        public IActionResult AdminGetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.PreTasks.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskType == "PreTasks") });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {


            var task = _context.PreTasks.Find(id);
            task.IsDone = true;
            task.DateTaskCompleted = DateTime.Now;
            task.User = User.Identity.Name;

            var date = task.DateCreated;


            var tasks = _context.PreTasks.Where(d => d.DateCreated == date).ToList();

            foreach (var item in tasks)
            {

                if (item.IsDone == false)
                {
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Task inCompleted!" });
                }

                //else
                //{
                //    task.DateAllTaskCompleted = DateTime.Now;
                //    task.TasksCompleted = true;
                //}

            }



            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task Completed!" });

        }

        [HttpGet]
        public async Task<IActionResult> AddComment(int id, string comment)
        {


            var task = _context.PreTasks.Find(id);

            task.Comments = comment;


            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Comment added!" });

        }

        [HttpGet]
        public IActionResult GetTask(int id)
        {
            var preTask = _context.PreTasks.Find(id);
            return Json(preTask);

        }

        #endregion
    }
}
