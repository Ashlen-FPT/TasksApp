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
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            return View(await _context.items.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TaskId,SubItem,Checks,Machine,UserName,PlantNumber,Hours,Date,InOrder,OutOfOrder,Schedule,IsDone")] Items items)
        {
            if (ModelState.IsValid)
            {
                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(items);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.items.FindAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            return View(items);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TaskId,SubItem,Checks,Machine,UserName,PlantNumber,Hours,Date,InOrder,OutOfOrder,Schedule,IsDone")] Items items)
        {
            if (id != items.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(items);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemsExists(items.Id))
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
            return View(items);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var items = await _context.items.FindAsync(id);
            _context.items.Remove(items);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemsExists(int id)
        {
            return _context.items.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public async Task<IActionResult> GetMonAsync(DateTime date, DayOfWeek dayOfWeek )
        {
            var dateValue = DateTime.Now.DayOfWeek;
            if (dateValue == DayOfWeek.Monday)
            {
                DateTime oDate = Convert.ToDateTime(date);


                var TasksToday = _context.items.Where(d => d.DateCreated.Date == oDate.Date & dateValue == DayOfWeek.Monday).ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Monday").ToList();
                    var last = Main_Task.LastOrDefault();

                    foreach (var task in Main_Task)
                    {

                        var Task = new Items
                        {
                            Checks = task.Main,
                            SubItem = task.Description,
                            DateCreated = date,
                            DateCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete"
                        };
                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Items - Monday";
                        }

                        _context.items.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Monday").ToList();
                    var last = Main_Task.LastOrDefault();
                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.SubItem != p.Description)).Where(p => TasksToday.All(p2 => p2.Schedule != p.Schedule));

                        foreach (var item in result)
                        {
                            var Task = new Items
                            {
                                Checks = item.Main,
                                SubItem = item.Description,
                                DateCreated = date,
                                Schedule = item.Schedule,
                                DateCompleted = new DateTime(),
                                Status = "Do-Checklist"
                            };

                            _context.items.Add(Task);
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
                    UpdatedTable = "Items",
                    OldData = "Read Items Checklist - Monday",
                    NewData = null
                };

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                return Json(new { data = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Monday") });
            }

            if (dateValue != DayOfWeek.Monday)
            {
                return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Wednesday").ToList() });
            }

            await _context.SaveChangesAsync();
            return Json(new { data = _context.items.Where(s => s.Schedule == "Monday") });
        }

        [HttpGet]
        public async Task<IActionResult> GetTueAsync(DateTime date/*, DayOfWeek dayOfWeek*/)
        {
            var dateValue = DateTime.Now.DayOfWeek;
            if (dateValue == DayOfWeek.Tuesday)
            {
                DateTime oDate = Convert.ToDateTime(date);

                //var dateValue = DateTime.Now.DayOfWeek;
                var TasksToday = _context.items.Where(d => d.DateCreated.Date == oDate.Date & dateValue == DayOfWeek.Tuesday).ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Tuesday").ToList();
                    var last = Main_Task.LastOrDefault();
                    foreach (var task in Main_Task)
                    {

                        var Task = new Items
                        {
                            Checks = task.Main,
                            SubItem = task.Description,
                            DateCreated = date,
                            DateCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete"
                        };
                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Items - Tuesday";
                        }

                        _context.items.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Tuesday").ToList();

                    if (Main_Task.Count >= TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.SubItem != p.Description)).Where(p => TasksToday.All(p2 => p2.Schedule != p.Schedule));

                        foreach (var item in result)
                        {
                            var Task = new Items
                            {
                                Checks = item.Main,
                                SubItem = item.Description,
                                DateCreated = date,
                                Schedule = item.Schedule,
                                DateCompleted = new DateTime(),
                                Status = "Do-Checklist"
                            };

                            _context.items.Add(Task);
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
                    UpdatedTable = "Items",
                    OldData = "Read Items Checklist - Tuesday",
                    NewData = null
                };

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                return Json(new { data = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Tuesday") });
            }

            if (dateValue != DayOfWeek.Tuesday)
            {
                return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Wednesday").ToList() });
            }

            await _context.SaveChangesAsync();
            return Json(new { data = _context.items.Where(s => s.Schedule == "Tuesday") });

        }

        [HttpGet]
        public async Task<IActionResult> GetWedAsync(DateTime date, DayOfWeek dayOfWeek)
        {
            var dateValue = DateTime.Now.DayOfWeek;
            if (dateValue == DayOfWeek.Wednesday)
            {
                DateTime oDate = Convert.ToDateTime(date);


                var TasksToday = _context.items.Where(d => d.DateCreated.Date == oDate.Date & dateValue == DayOfWeek.Wednesday).ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Wednesday").ToList();
                    var last = Main_Task.LastOrDefault();
                    foreach (var task in Main_Task)
                    {

                        var Task = new Items
                        {
                            Checks = task.Main,
                            SubItem = task.Description,
                            DateCreated = date,
                            DateCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete"
                        };
                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Items - Wednesday";
                        }

                        _context.items.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Wednesday").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.SubItem != p.Description)).Where(p => TasksToday.All(p2 => p2.Schedule != p.Schedule));

                        foreach (var item in result)
                        {
                            var Task = new Items
                            {
                                Checks = item.Main,
                                SubItem = item.Description,
                                DateCreated = date,
                                Schedule = item.Schedule,
                                DateCompleted = new DateTime(),
                                Status = "Do-Checklist"
                            };

                            _context.items.Add(Task);
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
                    UpdatedTable = "Items",
                    OldData = "Read Items Checklist - Wednesday",
                    NewData = null
                };

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                return Json(new { data = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Wednesday") });
            }

            if (dayOfWeek != DayOfWeek.Wednesday)
            {
                return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Wednesday").ToList() });
            }

            await _context.SaveChangesAsync();
            return Json(new { data = _context.items.Where(s => s.Schedule == "Wednesday") });

        }

        [HttpGet]
        public async Task<IActionResult> GetThuAsync(DateTime date, DayOfWeek dayOfWeek)
        {
            var dateValue = DateTime.Now.DayOfWeek;
            if (dateValue == DayOfWeek.Thursday)
            {
                DateTime oDate = Convert.ToDateTime(date);

                //var dateValue = DateTime.Now.DayOfWeek;
                var TasksToday = _context.items.Where(d => d.DateCreated.Date == oDate.Date & dateValue == DayOfWeek.Thursday).ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Thursday").ToList();
                    var last = Main_Task.LastOrDefault();
                    foreach (var task in Main_Task)
                    {

                        var Task = new Items
                        {
                            Checks = task.Main,
                            SubItem = task.Description,
                            DateCreated = date,
                            DateCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete"
                        };
                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Items - Thursday";
                        }

                        _context.items.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Thursday").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.SubItem != p.Description)).Where(p => TasksToday.All(p2 => p2.Schedule != p.Schedule));

                        foreach (var item in result)
                        {
                            var Task = new Items
                            {
                                Checks = item.Main,
                                SubItem = item.Description,
                                DateCreated = date,
                                Schedule = item.Schedule,
                                DateCompleted = new DateTime(),
                                Status = "Do-Checklist"
                            };

                            _context.items.Add(Task);
                        }

                    }
                }
                await _context.SaveChangesAsync();
                return Json(new { data = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Thursday") });
            }

            if (dateValue != DayOfWeek.Thursday)
            {
                return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Wednesday").ToList() });
            }

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Read,
                DateTime = DateTime.Now,
                UpdatedTable = "Items",
                OldData = "Read Items Checklist - Thursday",
                NewData = null
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
            return Json(new { data = _context.items.Where(s => s.Schedule == "Thursday") });

        }

        [HttpGet]
        public async Task<IActionResult> GetFriAsync(DateTime date, DayOfWeek dayOfWeek)
        {
            var dateValue = DateTime.Now.DayOfWeek;
            if (dateValue == DayOfWeek.Friday)
            {
                DateTime oDate = Convert.ToDateTime(date);

                //var dateValue = DateTime.Now.DayOfWeek;
                var TasksToday = _context.items.Where(d => d.DateCreated.Date == oDate.Date & dateValue == DayOfWeek.Friday).ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Friday").ToList();
                    var last = Main_Task.LastOrDefault();
                    foreach (var task in Main_Task)
                    {

                        var Task = new Items
                        {
                            Checks = task.Main,
                            SubItem = task.Description,
                            DateCreated = date,
                            DateCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete"
                        };
                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Items - Friday";
                        }

                        _context.items.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Friday").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.SubItem != p.Description)).Where(p => TasksToday.All(p2 => p2.Schedule != p.Schedule));

                        foreach (var item in result)
                        {
                            var Task = new Items
                            {
                                Checks = item.Main,
                                SubItem = item.Description,
                                DateCreated = date,
                                Schedule = item.Schedule,
                                DateCompleted = new DateTime(),
                                Status = "Do-Checklist"
                            };

                            _context.items.Add(Task);
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
                    UpdatedTable = "Items",
                    OldData = "Read Items Checklist - Friday",
                    NewData = null
                };

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                return Json(new { data = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Friday") });
            }

            if (dateValue != DayOfWeek.Friday)
            {
                return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Wednesday").ToList() });
            }

            await _context.SaveChangesAsync();
            return Json(new { data = _context.items.Where(s => s.Schedule == "Friday") });
        }

        [HttpGet]
        public async Task<IActionResult> GetSatAsync(DateTime date, DayOfWeek dayOfWeek)
        {
            var dateValue = DateTime.Now.DayOfWeek;
            if (dateValue == DayOfWeek.Saturday)
            {
                DateTime oDate = Convert.ToDateTime(date);

                //var dateValue = DateTime.Now.DayOfWeek;
                var TasksToday = _context.items.Where(d => d.DateCreated.Date == oDate.Date & dateValue == DayOfWeek.Saturday).ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Saturday").ToList();
                    var last = Main_Task.LastOrDefault();
                    foreach (var task in Main_Task)
                    {

                        var Task = new Items
                        {
                            Checks = task.Main,
                            SubItem = task.Description,
                            DateCreated = date,
                            DateCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete"
                        };
                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Items - Saturday";
                        }

                        _context.items.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Saturday").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.SubItem != p.Description)).Where(p => TasksToday.All(p2 => p2.Schedule != p.Schedule));

                        foreach (var item in result)
                        {
                            var Task = new Items
                            {
                                Checks = item.Main,
                                SubItem = item.Description,
                                DateCreated = date,
                                Schedule = item.Schedule,
                                DateCompleted = new DateTime(),
                                Status = "Do-Checklist"
                            };

                            _context.items.Add(Task);
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
                    UpdatedTable = "Items",
                    OldData = "Read Items Checklist - Saturday",
                    NewData = null
                };

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                return Json(new { data = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Saturday") });
            }

            if (dateValue != DayOfWeek.Saturday)
            {
                return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Wednesday").ToList() });
            }

            await _context.SaveChangesAsync();
            return Json(new { data = _context.items.Where(s => s.Schedule == "Saturday") });

        }

        [HttpGet]
        public async Task<IActionResult> GetSunAsync(DateTime date, DayOfWeek dayOfWeek)
        {
            var dateValue = DateTime.Now.DayOfWeek;
            if (dateValue == DayOfWeek.Sunday)
            {
                DateTime oDate = Convert.ToDateTime(date);

                //var dateValue = DateTime.Now.DayOfWeek;
                var TasksToday = _context.items.Where(d => d.DateCreated.Date == oDate.Date & dateValue == DayOfWeek.Sunday).ToList();

                if (TasksToday.Count == 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Sunday").ToList();
                    var last = Main_Task.LastOrDefault();
                    foreach (var task in Main_Task)
                    {

                        var Task = new Items
                        {
                            Checks = task.Main,
                            SubItem = task.Description,
                            DateCreated = date,
                            DateCompleted = new DateTime(),
                            Schedule = task.Schedule,
                            Status = "Task : Incomplete"
                        };
                        if (task == last)
                        {
                            Task.Status = "Do-Checklist : Items - Sunday";
                        }

                        _context.items.Add(Task);

                    }
                }

                if (TasksToday.Count > 0)
                {
                    var Main_Task = _context.TemplateItem.Where(s => s.Schedule == "Sunday").ToList();

                    if (Main_Task.Count > TasksToday.Count)
                    {
                        var result = Main_Task.Where(p => TasksToday.All(p2 => p2.SubItem != p.Description)).Where(p => TasksToday.All(p2 => p2.Schedule != p.Schedule));

                        foreach (var item in result)
                        {
                            var Task = new Items
                            {
                                Checks = item.Main,
                                SubItem = item.Description,
                                DateCreated = date,
                                Schedule = item.Schedule,
                                DateCompleted = new DateTime(),
                                Status = "Do-Checklist"
                            };

                            _context.items.Add(Task);
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
                    UpdatedTable = "Items",
                    OldData = "Read Items Checklist - Sunday",
                    NewData = null
                };

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                return Json(new { data = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Where(s => s.Schedule == "Sunday") });
            }

            if (dateValue != DayOfWeek.Sunday)
            {
                return Json(new { data = _context.TemplateItem.Where(s => s.Schedule == "Wednesday").ToList() });
            }

            await _context.SaveChangesAsync();
            return Json(new { data = _context.items.Where(s => s.Schedule == "Sunday") });

        }

        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.items.Where(d => d.DateCreated.Date == oDate.Date)

            });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var today = DateTime.Now.DayOfWeek.ToString();
            var Main_Task = _context.TemplateItem.Where(x=>x.Schedule==today).ToList();
            var Items = _context.items.ToList();
            var last = Main_Task.LastOrDefault();
            var count = Main_Task.Count();
            var DateCreation = new DateTime();
            var Ddate = _context.items.Find(id).DateCreated;
            var task = _context.items.Find(id);
            //Get Last Item & Change Status
            int digit;
            if(count < 4)
            {
                digit = count;
            }
            else
            {
                digit = 1;
            }
            var items = Items.Where((x, i) => i % count == count - digit);
            var ItemDate = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.DateCreated).FirstOrDefault();
            var ItemStatus = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Status).FirstOrDefault();
            var ItemId = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Id).FirstOrDefault();
            var ChangeStatus = _context.items.Find(ItemId);


            task.DateCompleted = DateTime.Now;
            task.Status = "Task : Completed";
            task.IsDone = true;
            task.UserName = User.Identity.Name;
            DateCreation = task.DateCreated;
            //var date = task.DateCreated;

            if (ItemDate == Ddate)
            {
                ChangeStatus.Status = "Partially Completed : Items - " + today;
            }

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Completed,
                DateTime = DateTime.Now,
                UpdatedTable = "Items",
                OldData = null,
                NewData = "Task Completed"
            };
            _context.Logs.Add(log);
            _context.SaveChanges();


            var tasks = _context.items.Where(d => d.DateCreated == DateCreation).ToList();

            if (tasks.All(c => c.IsDone == true))
            {

                if (ItemDate == Ddate)
                {
                    task.DateAllTaskCompleted = DateTime.Now;
                    ChangeStatus.Status = "Completed : Items - " + today;
                }
            }

            foreach (var item in tasks)
            {

                if (item.IsDone == false)
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
            var task = _context.items.Where(d => d.DateCreated == date).Where(t => t.IsDone == true).ToList();

            foreach (var item in task)
            {
                if (item.IsDone == true)
                {
                    //item.TasksCompleted = true;
                    item.DateAllTaskCompleted = DateTime.Now;
                }

            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "All Tasks Completed!" });
        }


        [HttpGet]
        public IActionResult GetTask(int id)
        {
            var task = _context.items.Find(id);
            return Json(task);

        }
        #endregion
    }
}
