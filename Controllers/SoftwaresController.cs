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
    public class SoftwaresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userServices;

        public SoftwaresController(ApplicationDbContext context , UserService userService)
        {
            _context = context;
            _userServices = userService;
        }

        // GET: Softwares
        public async Task<IActionResult> Index()
        {
            return View(await _context.Software.ToListAsync());
        }

        // GET: Softwares/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var software = await _context.Software
                .FirstOrDefaultAsync(m => m.Id == id);
            if (software == null)
            {
                return NotFound();
            }

            return View(software);
        }

        // GET: Softwares/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Softwares/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Comments,Description,IsDone,DateTaskCompleted,DateCreated,User,Schedule,TaskCategory")] Software software)
        {
            if (ModelState.IsValid)
            {
                _context.Add(software);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(software);
        }

        // GET: Softwares/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var software = await _context.Software.FindAsync(id);
            if (software == null)
            {
                return NotFound();
            }
            return View(software);
        }

        // POST: Softwares/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comments,Description,IsDone,DateTaskCompleted,DateCreated,User,Schedule,TaskCategory")] Software software)
        {
            if (id != software.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(software);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoftwareExists(software.Id))
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
            return View(software);
        }

        // GET: Softwares/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var software = await _context.Software
                .FirstOrDefaultAsync(m => m.Id == id);
            if (software == null)
            {
                return NotFound();
            }

            return View(software);
        }

        // POST: Softwares/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var software = await _context.Software.FindAsync(id);
            _context.Software.Remove(software);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SoftwareExists(int id)
        {
            return _context.Software.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public async Task<IActionResult> GetTasksTodayAsync(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskCategory == "OS/Software").ToList();


            if (TasksToday.Count == 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "OS/Software").ToList();
                var last = Main_Task.LastOrDefault();

                foreach (var task in Main_Task)
                {

                    var Task = new Software
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
                        Task.Status = "Do-Checklist : OS/Software";
                    }

                    _context.Software.Add(Task);

                }
            }

            if (TasksToday.Count > 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "OS/Software").ToList();

                if (Main_Task.Count >= TasksToday.Count)
                {
                    var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description)).Where(s => s.TaskCategory == "OS/Software");

                    foreach (var item in result)
                    {
                        var Task = new Software
                        {
                            Description = item.Description,
                            DateCreated = date,
                            Schedule = item.Schedule,
                            DateTaskCompleted = new DateTime(),
                            TaskCategory = item.TaskCategory
                        };

                        _context.Software.Add(Task);
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
                UpdatedTable = "Software",
                OldData = "Read Software Daily Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "OS/Software") });
        }

        [HttpGet]
        public async Task<IActionResult> GetTasksWeekly(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var Day = oDate.DayOfWeek.ToString();

            var TasksToday = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Weekly").ToList();


            if (TasksToday.Count == 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Weekly").Where(d => d.DayOfWeek == Day).ToList();

                foreach (var task in Main_Task)
                {

                    var Task = new Software
                    {
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Schedule = task.Schedule

                    };

                    _context.Software.Add(Task);

                }



            }

            if (TasksToday.Count > 0)
            {
                var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Weekly").Where(d => d.DayOfWeek == Day).ToList();

                if (Main_Task.Count > TasksToday.Count)
                {
                    var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                    foreach (var item in result)
                    {
                        var Task = new Software
                        {
                            Description = item.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = item.Schedule

                        };

                        _context.Software.Add(Task);
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
                UpdatedTable = "Software",
                OldData = "Read Software Weekly Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Weekly") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksMonthly(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var firstDayOfMonth = new DateTime(oDate.Year, oDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            if (firstDayOfMonth == oDate)
            {
                var TasksToday = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "OS/Software").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "OS/Software").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Software
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,

                        };

                        _context.Software.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "OS/Software").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Software
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule

                            };

                            _context.Software.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfMonth == oDate)
            {
                var TasksToday = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "OS/Software").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "OS/Software").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Software
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,

                        };

                        _context.Software.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "OS/Software").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Software
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule

                            };

                            _context.Software.Add(Task);
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
                UpdatedTable = "Software",
                OldData = "Read Software Monthly Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "OS/Software") });

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
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,

                        };

                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule

                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfQuarter == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,

                        };
                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule

                            };

                            _context.Active_D.Add(Task);
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
                UpdatedTable = "Software",
                OldData = "Read Software Quarterly Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksBi(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var firstDayOfMonth = new DateTime(oDate.Year, oDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            if (firstDayOfMonth == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,

                        };

                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule

                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfMonth == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,

                        };
                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule

                            };

                            _context.Active_D.Add(Task);
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
                UpdatedTable = "Software",
                OldData = "Read Software Bi-Annual Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D") });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksAnnually(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var firstDayOfMonth = new DateTime(oDate.Year, oDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            if (firstDayOfMonth == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,

                        };

                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "Beginning").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule

                            };

                            _context.Active_D.Add(Task);
                        }

                    }
                }
            }

            if (lastDayOfMonth == oDate)
            {
                var TasksToday = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D").ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Active_D
                        {
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Schedule = task.Schedule,

                        };
                        _context.Active_D.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Monthly").Where(d => d.Month == "End").Where(s => s.TaskCategory == "Active_D").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                        foreach (var item in result)
                        {
                            var Task = new Active_D
                            {
                                Description = item.Description,
                                DateCreated = date,
                                DateTaskCompleted = new DateTime(),
                                Schedule = item.Schedule

                            };

                            _context.Active_D.Add(Task);
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
                UpdatedTable = "Software",
                OldData = "Read Software Annual Checklist",
                NewData = null
            };
            _context.Logs.Add(log);

            await _context.SaveChangesAsync(_userServices.GetUser());

            return Json(new { data = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly").Where(s => s.TaskCategory == "Active_D") });

        }

        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.TaskCategory == "OS/Software") });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var Main_Task = _context.Main_Task.Where(s => s.Schedule == "Daily").Where(s => s.TaskCategory == "OS/Software").ToList();
            var Software = _context.Software.ToList();
            var last = Main_Task.LastOrDefault();
            var count = Main_Task.Count();
            var DateCreation = new DateTime();
            var Ddate = _context.Software.Find(id).DateCreated;
            var task = _context.Software.Find(id);
            //Get Last Item & Change Status
            var items = Software.Where((x, i) => i % count == count - 1);
            var ItemDate = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.DateCreated).FirstOrDefault();
            var ItemStatus = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Status).FirstOrDefault();
            var ItemId = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Id).FirstOrDefault();
            var ChangeStatus = _context.Software.Find(ItemId);

            task.IsDone = true;
            task.DateTaskCompleted = DateTime.Now;
            task.User = User.FindFirst("Username")?.Value;
            DateCreation = task.DateCreated;
            task.Status = "Task : Completed";

            if (ItemDate == Ddate)
            {
                ChangeStatus.Status = "Partially Completed : OS/Software";
            }


            var tasks = _context.Software.Where(d => d.DateCreated == DateCreation).ToList();

            if (tasks.All(c => c.IsDone == true))
            {

                if (ItemDate == Ddate)
                {
                    task.DateAllTaskCompleted = DateTime.Now;
                    ChangeStatus.Status = "Completed : OS/Software";
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
                UpdatedTable = "Software",
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
            var task = _context.Software.Find(id);

            task.Comments = comment;

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Updated,
                DateTime = DateTime.Now,
                UpdatedTable = "Software",
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
            var task = _context.Software.Find(id);
            return Json(task);
        }

        [HttpGet]
        public IActionResult AdminGetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Software.Where(d => d.DateCreated.Date == oDate.Date) });

        }

        [HttpGet]
        public IActionResult AdminGetAllW(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Weekly") });

        }

        [HttpGet]
        public IActionResult AdminGetAllM(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monthly") });

        }

        [HttpGet]
        public IActionResult AdminGetAllQ(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Quarterly") });

        }

        [HttpGet]
        public IActionResult AdminGetAllB(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Bi-Annually") });

        }

        [HttpGet]
        public IActionResult AdminGetAllA(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Anually") });

        }

        #endregion
    }

}

