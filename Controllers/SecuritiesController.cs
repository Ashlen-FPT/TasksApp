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
using TasksApp.Services;

namespace TasksApp.Controllers
{
    public class SecuritiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userServices;

        public SecuritiesController(ApplicationDbContext context , UserService userService)
        {
            _context = context;
            _userServices = userService;
        }

        // GET: Securities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Security.ToListAsync());
        }

        // GET: Securities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var security = await _context.Security
                .FirstOrDefaultAsync(m => m.Id == id);
            if (security == null)
            {
                return NotFound();
            }

            return View(security);
        }

        // GET: Securities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Securities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Comments,Description,IsDone,DateTaskCompleted,DateCreated,User,Schedule,TaskCategory")] Security security)
        {
            if (ModelState.IsValid)
            {
                _context.Add(security);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(security);
        }

        // GET: Securities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var security = await _context.Security.FindAsync(id);
            if (security == null)
            {
                return NotFound();
            }
            return View(security);
        }

        // POST: Securities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comments,Description,IsDone,DateTaskCompleted,DateCreated,User,Schedule,TaskCategory")] Security security)
        {
            if (id != security.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(security);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecurityExists(security.Id))
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
            return View(security);
        }

        // GET: Securities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var security = await _context.Security
                .FirstOrDefaultAsync(m => m.Id == id);
            if (security == null)
            {
                return NotFound();
            }

            return View(security);
        }

        // POST: Securities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var security = await _context.Security.FindAsync(id);
            _context.Security.Remove(security);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SecurityExists(int id)
        {
            return _context.Security.Any(e => e.Id == id);
        }

        #region API Calls


        [HttpGet]
        public async Task<IActionResult> GetTasksTodayAsync(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskCategory == "Security").ToList();


            if (TasksToday.Count == 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "Security").ToList();
                var last = Main_Task.LastOrDefault();

                foreach (var task in Main_Task)
                {

                    var Task = new Security
                    {
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Schedule = task.Schedule,
                        TaskCategory = task.TaskCategory,
                        Status = "Task : Incomplete",
                        User = User.FindFirst("Username")?.Value
                    };

                    if (task == last)
                    {
                        Task.Status = "Do-Checklist : Security";
                    }
                    _context.Security.Add(Task);

                }
            }

            if (TasksToday.Count > 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "Security").ToList();
                var last = Main_Task.LastOrDefault();

                if (Main_Task.Count > TasksToday.Count)
                {
                    var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description)).Where(s => s.TaskCategory == "Security");

                    foreach (var item in result)
                    {
                        var Task = new Security
                        {
                            Description = item.Description,
                            DateCreated = date,
                            Schedule = item.Schedule,
                            DateTaskCompleted = new DateTime(),
                            TaskCategory = item.TaskCategory,
                            Status = "Task : Incomplete",
                            User = User.FindFirst("Username")?.Value
                        };

                        if (item == last)
                        {
                            Task.Status = "Do-Checklist : Security";
                        }

                        _context.Security.Add(Task);
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
                UpdatedTable = "Security",
                OldData = "Read Security Daily Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "Security") });
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksWeekly(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var Day = oDate.DayOfWeek.ToString();

            var TasksToday = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Weekly").ToList();


            if (TasksToday.Count == 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Weekly").Where(d => d.DayOfWeek == Day).ToList();
                var last = Main_Task.LastOrDefault();

                foreach (var task in Main_Task)
                {

                    var Task = new Security
                    {
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Schedule = task.Schedule,
                        Status = "Task : Incomplete",
                        User = User.FindFirst("Username")?.Value
                    };

                    if (task == last)
                    {
                        Task.Status = "Do-Checklist : Security";
                    }

                    _context.Security.Add(Task);

                }
            }

            if (TasksToday.Count > 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Weekly").Where(d => d.DayOfWeek == Day).ToList();
                var last = Main_Task.LastOrDefault();

                if (Main_Task.Count > TasksToday.Count)
                {
                    var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                    foreach (var item in result)
                    {
                        var Task = new Security
                        {
                            Description = item.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = item.Schedule,
                            Status = "Task : Incomplete",
                            User = User.FindFirst("Username")?.Value
                        };

                        if (item == last)
                        {
                            Task.Status = "Do-Checklist : Security";
                        }

                        _context.Security.Add(Task);
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
                UpdatedTable = "Security",
                OldData = "Read Security Weekly Checklist",
                NewData = null
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Weekly") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksMonthly(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var firstDayOfMonth = new DateTime(oDate.Year, oDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            if (firstDayOfMonth == oDate)
            {
                var TasksToday = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Security").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Security
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete",
                            User = User.FindFirst("Username")?.Value
                        };

                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Security";
                        }

                        _context.Security.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Security
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                Status = "Task : Incomplete",
                                User = User.FindFirst("Username")?.Value
                            };

                            if (item == last)
                            {
                                Task.Status = "Do-Checklist : Security";
                            }

                            _context.Security.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfMonth == oDate)
            {
                var TasksToday = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Security").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Security
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete",
                            User = User.FindFirst("Username")?.Value
                        };

                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Security";
                        }

                        _context.Security.Add(Task);

                    }



                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Security
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                Status = "Task : Incomplete",
                                User = User.FindFirst("Username")?.Value
                            };

                            if (item == last)
                            {
                                Task.Status = "Do-Checklist : Security";
                            }

                            _context.Security.Add(Task);
                        }

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
                UpdatedTable = "Security",
                OldData = "Read Security Monthly Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Security") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksQuarterly(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            //var firstDayOfMonth = new DateTime(oDate.Year, oDate.Month, 1);
            //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            int quarterNumber = (oDate.Month - 1) / 3 + 1;
            DateTime firstDayOfQuarter = new DateTime(oDate.Year, (quarterNumber - 1) * 3 + 1, 1);
            DateTime lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);


            if (firstDayOfQuarter == oDate)
            {
                var TasksToday = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Quarterly").Where(s => s.TaskCategory == "Security").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Quarterly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Security
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete",
                            User = User.FindFirst("Username")?.Value
                        };

                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Security";
                        }

                        _context.Security.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Quarterly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Security
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                Status = "Task : Incomplete",
                                User = User.FindFirst("Username")?.Value
                            };

                            if (item == last)
                            {
                                Task.Status = "Do-Checklist : Security";
                            }

                            _context.Security.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfQuarter == oDate)
            {
                var TasksToday = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Quarterly").Where(s => s.TaskCategory == "Security").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Quarterly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Security
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete",
                            User = User.FindFirst("Username")?.Value
                        };

                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Security";
                        }
                        _context.Security.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Quarterly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Security
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                Status = "Task : Incomplete",
                                User = User.FindFirst("Username")?.Value
                            };

                            if (item == last)
                            {
                                Task.Status = "Do-Checklist : Security";
                            }

                            _context.Security.Add(Task);
                        }

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
                UpdatedTable = "Security",
                OldData = "Read Security Quarterly Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Quarterly").Where(s => s.TaskCategory == "Security") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksBi(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var firstDayOfMonth = new DateTime(oDate.Year, oDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            if (firstDayOfMonth == oDate)
            {
                var TasksToday = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Bi-Annually").Where(s => s.TaskCategory == "Security").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Bi-Annually").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Security
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete",
                            User = User.FindFirst("Username")?.Value
                        };

                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Security";
                        }

                        _context.Security.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Bi-Annually").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Security
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                Status = "Task : Incomplete",
                                User = User.FindFirst("Username")?.Value
                            };

                            if (item == last)
                            {
                                Task.Status = "Do-Checklist : Security";
                            }

                            _context.Security.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfMonth == oDate)
            {
                var TasksToday = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Bi-Annually").Where(s => s.TaskCategory == "Security").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Bi-Annually").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Security
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete",
                            User = User.FindFirst("Username")?.Value
                        };

                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Security";
                        }
                        _context.Security.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Bi-Annually").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Security
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                Status = "Task : Incomplete",
                                User = User.FindFirst("Username")?.Value
                            };

                            if (item == last)
                            {
                                Task.Status = "Do-Checklist : Security";
                            }

                            _context.Security.Add(Task);
                        }

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
                UpdatedTable = "Security",
                OldData = "Read Security Bi-Annual Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Bi-Annually").Where(s => s.TaskCategory == "Security") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksAnnually(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var firstDayOfMonth = new DateTime(oDate.Year, oDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            if (firstDayOfMonth == oDate)
            {
                var TasksToday = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Annually").Where(s => s.TaskCategory == "Security").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Annually").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Security
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete",
                            User = User.FindFirst("Username")?.Value
                        };

                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Security";
                        }

                        _context.Security.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Annually").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Security
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                Status = "Task : Incomplete",
                                User = User.FindFirst("Username")?.Value
                            };

                            if (item == last)
                            {
                                Task.Status = "Do-Checklist : Security";
                            }

                            _context.Security.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfMonth == oDate)
            {
                var TasksToday = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Annually").Where(s => s.TaskCategory == "Security").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Annually").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Security
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete",
                            User = User.FindFirst("Username")?.Value
                        };

                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Security";
                        }
                        _context.Security.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Annually").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Security").ToList();
                    var last = Main_Task.LastOrDefault();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Security
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule,
                                Status = "Task : Incomplete",
                                User = User.FindFirst("Username")?.Value
                            };

                            if (item == last)
                            {
                                Task.Status = "Do-Checklist : Security";
                            }

                            _context.Security.Add(Task);
                        }

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
                UpdatedTable = "Security",
                OldData = "Read Security Annual Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Annually").Where(s => s.TaskCategory == "Security") });

        }

        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskCategory == "Security") });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "Security").ToList();
            var Security = _context.Security.ToList();
            var last = Main_Task.LastOrDefault();
            var count = Main_Task.Count();
            var DateCreation = new DateTime();
            var Ddate = _context.Security.Find(id).DateCreated;
            var task = _context.Security.Find(id);

            //ChangeToPartialStatus
            var getPartial = Security.Where(x => x.DateCreated == Ddate && x.Status.StartsWith("D"));
            var getPDate = getPartial.Select(i => i.DateCreated).FirstOrDefault();
            var getPId = getPartial.Select(i => i.Id).FirstOrDefault();
            var ChangeToPartialStatus = _context.Security.Find(getPId);

            //ChangeToCompleteStatus
            var getComplete = Security.Where(x => x.DateCreated == Ddate && x.Status.StartsWith("P"));
            var getCDate = getComplete.Select(i => i.DateCreated).FirstOrDefault();
            var getCId = getComplete.Select(i => i.Id).FirstOrDefault();
            var ChangeToCompleteStatus = _context.Security.Find(getCId);


            task.IsDone = true;
            task.DateTaskCompleted = DateTime.Now;
            task.User = User.FindFirst("Username")?.Value;
            DateCreation = task.DateCreated;
            task.Status = "Task : Completed";

            if (getPDate == Ddate)
            {
                ChangeToPartialStatus.Status = "Partially Completed : Security";
            }


            var tasks = _context.Security.Where(d => d.DateCreated == DateCreation).ToList();

            if (tasks.All(c => c.IsDone == true))
            {

                if (getCDate == Ddate)
                {
                    task.DateAllTaskCompleted = DateTime.Now;
                    ChangeToCompleteStatus.Status = "Completed : Security";
                }
            }

            foreach (var item in tasks)
            {

                if (item.IsDone == false)
                {
                    await _context.SaveChangesAsync(_userServices.GetUser());

                    return Json(new { success = true, message = "Task Completed!" });
                }

                //else
                //{

                //    task.DateAllTaskCompleted = DateTime.Now;
                //    task.TasksCompleted = true;
                //}

            }


            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Completed,
                DateTime = DateTime.Now,
                UpdatedTable = "Security",
                OldData = null,
                NewData = "Task Completed"
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { success = true, message = "Task Completed!" });

        }

        [HttpGet]
        public async Task<IActionResult> AddComment(int id, string comment)
        {


            var task = _context.Security.Find(id);

            task.Comments = comment;

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Updated,
                DateTime = DateTime.Now,
                UpdatedTable = "Security",
                OldData = null,
                NewData = "Added Comment : " + comment
            };
            _context.Logs.Add(log);


            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { success = true, message = "Comment added!" });

        }

        [HttpGet]
        public IActionResult GetTask(int id)
        {
            var task = _context.Security.Find(id);
            return Json(task);

        }

        [HttpGet]
        public IActionResult AdminGetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date) });

        }

        [HttpGet]
        public IActionResult AdminGetAllW(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Weekly") });

        }

        [HttpGet]
        public IActionResult AdminGetAllM(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly") });

        }

        [HttpGet]
        public IActionResult AdminGetAllQ(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Quarterly") });

        }

        [HttpGet]
        public IActionResult AdminGetAllB(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Bi-Annually") });

        }

        [HttpGet]
        public IActionResult AdminGetAllA(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Anually") });

        }
        #endregion
    }

}
