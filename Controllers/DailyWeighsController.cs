using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TasksApp.Data;
using TasksApp.Enums;
using TasksApp.Models;
using TasksApp.Services;

namespace TasksApp.Controllers
{
    [Authorize]
    public class DailyWeighsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyWeighsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DailyWeighs
        public async Task<IActionResult> Index()
        {

            return View(await _context.DailyWeighs.ToListAsync());
        }

        // GET: DailyWeighs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyWeigh = await _context.DailyWeighs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyWeigh == null)
            {
                return NotFound();
            }

            return View(dailyWeigh);
        }

        // GET: DailyWeighs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DailyWeighs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Time,username,Gross,Tare,Net,Observation")] DailyWeigh dailyWeigh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dailyWeigh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dailyWeigh);
        }

        // GET: DailyWeighs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyWeigh = await _context.DailyWeighs.FindAsync(id);
            if (dailyWeigh == null)
            {
                return NotFound();
            }
            return View(dailyWeigh);
        }

        // POST: DailyWeighs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Time,username,Gross,Tare,Net,Observation")] DailyWeigh dailyWeigh)
        {
            if (id != dailyWeigh.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dailyWeigh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DailyWeighExists(dailyWeigh.Id))
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
            return View(dailyWeigh);
        }

        // GET: DailyWeighs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyWeigh = await _context.DailyWeighs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyWeigh == null)
            {
                return NotFound();
            }

            return View(dailyWeigh);
        }

        // POST: DailyWeighs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dailyWeigh = await _context.DailyWeighs.FindAsync(id);
            _context.DailyWeighs.Remove(dailyWeigh);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DailyWeighExists(int id)
        {
            return _context.DailyWeighs.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public async Task<IActionResult> GetTasksTodayAsync(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.DailyWeighs.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskType == "Tasks").Where(t => t.ChekList == "Weighbridge Test").ToList();

            if (TasksToday.Count == 0)
            {
                var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "Tasks").Where(t => t.ChekList == "Weighbridge Test").ToList();
                foreach (var task in TemplateTasks)
                {

                    var Task = new DailyWeigh
                    {
                        Description = task.Description,
                        Date = date,
                        DateCompleted = new DateTime(),
                        Schedule = task.Schedule,
                        TaskType = task.TaskType,
                        ChekList = task.ChekList,
                    };

                    _context.DailyWeighs.Add(Task);

                }
            }

            if (TasksToday.Count > 0)
            {
                var TemplateTasks = _context.TemplateTasks.Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "Tasks").Where(t => t.ChekList == "Weighbridge Test").ToList();

                if (TemplateTasks.Count > TasksToday.Count)
                {
                    var result = TemplateTasks.Where(p => TasksToday.All(p2 => p2.Description != p.Description)).Where(s => s.TaskType == "Tasks").Where(t => t.ChekList == "Weighbridge Test");

                    foreach (var item in result)
                    {
                        var Task = new DailyWeigh
                        {
                            Description = item.Description,
                            DateCreated = date,
                            Schedule = item.Schedule,
                            DateCompleted = new DateTime(),
                            TaskType = item.TaskType,
                            ChekList = item.ChekList,
                            Status="Task : Incomplete"
                        };

                        _context.DailyWeighs.Add(Task);
                    }

                }
            }

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Read,
                DateTime = DateTime.Now,
                UpdatedTable = "DailyWeighs",
                OldData = "Read DailyWeighs",
                NewData = null
            };

            _context.Logs.Add(log);

            await _context.SaveChangesAsync();

            return Json(new { data = _context.DailyWeighs.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Daily").Where(s => s.TaskType == "Tasks").Where(c => c.ChekList == "Weighbridge Test") });
        }

        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Read,
                DateTime = DateTime.Now,
                UpdatedTable = "DailyWeighs",
                OldData = "Read DailyWeighs",
                NewData = null
            };

            _context.Logs.Add(log);

            _context.SaveChanges();

            return Json(new { data = _context.DailyWeighs.Where(d => d.DateCreated.Date == oDate.Date) });

        }

        [HttpGet]
        public IActionResult GetTask(int id)
        {
            var task = _context.DailyWeighs.Find(id);
            return Json(task);

        }

        [HttpGet]
        public async Task<IActionResult> AddTask(int Id,int Gross, int Tare, int Net, string Observation, DateTime date)
        {
            DateTime oDate = Convert.ToDateTime(date);
            
            var tasks = _context.DailyWeighs.Where(d => d.DateCreated.Date == oDate.Date).ToList();
            //var task = _context.DailyWeighs.Find(Id);

            //task.Date = Convert.ToDateTime(date);
            //task.Time = DateTime.Now;
            //task.Supervisor = User.Identity.Name;
            //task.Gross = Gross;
            //task.Tare = Tare;
            //task.Net = Net;
            //task.Observation = Observation;
            //task.DateCreated = date;
            //_context.Add(task);
            

            if (tasks.Count == 0)
            {

                var Task = new DailyWeigh
                {
                    Date = date,
                    Time = DateTime.Now,
                    Supervisor = User.Identity.Name,
                    Gross = Gross,
                    Tare = Tare,
                    Net = Net,
                    Observation = Observation,
                    DateCreated = date,
                    DateCompleted = DateTime.Now,
                    DateAllTaskCompleted = new DateTime()
                };

                _context.Add(Task);
            }

            if (tasks.Count != 0)
            {

                var Task = new DailyWeigh
                {
                    Date = date,
                    Time = DateTime.Now,
                    Supervisor = User.Identity.Name,
                    Gross = Gross,
                    Tare = Tare,
                    Net = Net,
                    Observation = Observation,
                    DateCreated = date,
                    DateCompleted = DateTime.Now,
                    DateAllTaskCompleted = new DateTime()
                };

                _context.Add(Task);
            }

            if (tasks.Count > 0)
            {

                var results = tasks.Where(p => tasks.All(p2 => p2.Date == p.Date)).Where(x => tasks.All(x2 => x2.Time == x.Time));
                foreach (var item in results)
                {
                    var Task = new DailyWeigh
                    {
                        Date = date,
                        Time = DateTime.Now,
                        Supervisor = User.Identity.Name,
                        Gross = Gross,
                        Tare = Tare,
                        Net = Net,
                        Observation = Observation,
                        DateCreated = date,
                        DateCompleted  = DateTime.Now,
                        DateAllTaskCompleted = new DateTime()

                    };
                    var log = new Logs
                    {
                        UserName = User.FindFirst("Username")?.Value,
                        UserEmail = User.Identity.Name,
                        Entity = User.FindFirst("Organization")?.Value,
                        LogType = LogTypes.Created,
                        DateTime = DateTime.Now,
                        UpdatedTable = "DailyWeighs",
                        OldData = "New Daily Weigh",
                        NewData = $"{ "Gross: " + Task.Gross + "Tare: " + Task.Tare + "Net: " + Task.Net + "Date Created:" + Task.DateCreated}"
                    };
                    _context.Logs.Add(log);

                    _context.Add(Task);
                }

                var result = tasks.Where(p => tasks.All(p2 => p2.Date == p.Date)).Where(x => tasks.All(x2 => x2.Time == x.Time)).Where(r => tasks.All(r2 => r2.Gross == 0));
                foreach (var item in result)
                {
                    var Task = new DailyWeigh
                    {
                        Date = date,
                        Time = DateTime.Now,
                        Supervisor = User.Identity.Name,
                        Gross = Gross + Gross,
                        Tare = Tare,
                        Net = Net,
                        Observation = Observation,
                        DateCreated = date,
                        DateCompleted = DateTime.Now,
                        DateAllTaskCompleted = new DateTime()


                    };
                    var log = new Logs
                    {
                        UserName = User.FindFirst("Username")?.Value,
                        UserEmail = User.Identity.Name,
                        Entity = User.FindFirst("Organization")?.Value,
                        LogType = LogTypes.Updated,
                        DateTime = DateTime.Now,
                        UpdatedTable = "DailyWeighs",
                        OldData = "Updated Daily Weigh",
                        NewData = $"{ "Gross: " + Task.Gross + "Tare: " + Task.Tare + "Net: " + Task.Net + "Date Created:" + Task.DateCreated}"
                    };
                    _context.Logs.Add(log);
                    _context.Update(Task);
                }

            }



            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Weighbridge Test added!"});
        }

        [HttpGet]
        public IActionResult GetSignOff(DateTime date)
        {
            DateTime oDate = Convert.ToDateTime(date);

            var SignsToday = _context.DailyWeighs.Where(d => d.DateCreated.Date == oDate.Date).ToList().Take(1);

            return Json(new { data = SignsToday });
        }

        [HttpGet]

        public async Task<IActionResult> Sign(int id)
        {

            DateTime oDate = _context.DailyWeighs.Find(id).DateCreated;
            var Signature = _context.DailyWeighs.Find(id);
            var sign = _context.DailyWeighs.Where(x => x.DateCreated == oDate).ToList();

            if (User.IsInRole(SD.Role_Supervisor.ToString()))
            {
                foreach (var item in sign)
                {
                        item.IsDone = true;
                        item.Supervisor = User.Identity.Name;
                }
            }

            //var date = Signature.DateCreated;


            //var tasks = _context.Tasks.Where(d => d.DateCreated == date).ToList();

            //foreach (var item in tasks)
            //{

            //    if (item.IsDone == false)
            //    {
            //        await _context.SaveChangesAsync();

            //        return Json(new { success = true, message = "Task Completed!" });
            //    }

            //}
            Signature.DateAllTaskCompleted = DateTime.Now;

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Completed,
                DateTime = DateTime.Now,
                UpdatedTable = "DailyWeighs",
                OldData = null,
                NewData = $"{"Daily Weigh Signed Off" + User.FindFirst("Username")?.Value}"
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();


            return Json(new { success = true, message = "Supervisor Signed Off" });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var DateCreation = new DateTime();
            var task = _context.DailyWeighs.Find(id);

            
            task.IsDone = true;
            task.DateCompleted = DateTime.Now;
            task.Supervisor = User.Identity.Name;
            task.Status = "Task : Completed";
            DateCreation = task.Date;

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Completed,
                DateTime = DateTime.Now,
                UpdatedTable = "DailyWeighs",
                OldData = null,
                NewData = "Task Completed"
            };

            _context.Logs.Add(log);
            _context.SaveChanges();

            var tasks = _context.Tasks.Where(d => d.DateCreated == DateCreation).ToList();
            //bool status = tasks.All(c => c.IsDone == false);
            foreach (var item in tasks)
            {
                //if (item.Status == null)
                //{
                //    task.Status = "Do-CheckList";
                //    await _context.SaveChangesAsync();
                //}

                if (item.IsDone == false)
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

                    bool completeTasks = tasks.All(c => c.IsDone == true);
                    {
                        if (completeTasks == false)
                        {
                            //task.Status = "Partially Completed";
                            await _context.SaveChangesAsync();

                            return Json(new { success = true, message = "Task Completed!" });
                        }
                        else if (completeTasks == true)
                        {
                            //task.TasksCompleted = true;
                            task.DateCompleted = DateTime.Now;
                            //task.Status = "Completed";
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
        public async Task<IActionResult> AddComment(int id, string comment)
        {


            var task = _context.DailyWeighs.Find(id);

            task.Comments = comment;


            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Comment added!" });

        }
        #endregion
    }
}
