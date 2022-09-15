using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TasksApp.Data;
using TasksApp.Models;

namespace TasksApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            return View(await _context.reports.ToListAsync());
        }

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.reports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetReportAsync(DateTime date)
        {
            DateTime oDate = Convert.ToDateTime(date);

            var ReportsToday = _context.reports.Where(d => d.DateCreated.Date == oDate.Date).ToList();
            
            if (ReportsToday.Count == 0)
            {
                var bobcat = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).ToList();
                DateTime latest = (DateTime)_context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).Where(r => r.DateTaskCompleted.HasValue).Max(d => d.DateTaskCompleted);
                var bob = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateTaskCompleted == latest).ToList();
                //var bob = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateTaskCompleted == DateTime.MaxValue).ToList();

                if (bobcat.Any(s => s.Status.StartsWith("D")))
                {
                    var bobcats = bobcat.Take(1);

                    foreach (var task in bobcats)
                    {

                        var Task = new Report
                        {
                            Checklist = "BobCat",
                            Status = task.Status,
                            DateCreated = DateTime.Today,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.UserName1,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateTaskCompleted
                        };

                        _context.reports.Add(Task);
                        await _context.SaveChangesAsync();
                    }
                }

                else if (bobcat.Any(s => !s.Status.StartsWith("D")))
                {
                    foreach (var tasks in bob)
                    {

                        var Task = new Report
                        {
                            Checklist = "BobCat",
                            Status = tasks.Status,
                            DateCreated = date,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.UserName1,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateTaskCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }


                var dailyCheck = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).ToList();
                var dailyChecks = dailyCheck.Take(1);
                DateTime latest1 = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).Max(d => d.DateCompleted);
                var daily = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateCompleted == latest1).ToList();

                if (dailyCheck.Any(s => s.Status.StartsWith("D")))
                {

                    foreach (var task in dailyChecks)
                    {

                        var Task = new Report
                        {
                            Checklist = "Daily Checks",
                            Status = task.Status,
                            DateCreated = date,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else if (dailyCheck.Any(s => !s.Status.StartsWith("D")))
                {
                    foreach (var tasks in daily)
                    {

                        var Task = new Report
                        {
                            Checklist = "Daily Checks",
                            Status = tasks.Status,
                            DateCreated = date,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                var maintenance = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                var maintenances = maintenance.Take(1);
                DateTime latest2 = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).Max(d => d.DateCompleted);
                var main = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateCompleted == latest2).ToList();

                if (maintenance.Any(s => s.Status.StartsWith("D")))
                {

                    foreach (var task in maintenances)
                    {

                        var Task = new Report
                        {
                            Checklist = "Maintenance",
                            Status = task.Status,
                            DateCreated = date,
                            DateCompleted = task.DateAllCompleted,
                            UserName = task.User,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else if (maintenance.Any(s => !s.Status.StartsWith("D")))
                {
                    foreach (var tasks in main)
                    {

                        var Task = new Report
                        {
                            Checklist = "Maintenance",
                            Status = tasks.Status,
                            DateCreated = date,
                            DateCompleted = tasks.DateAllCompleted,
                            UserName = tasks.User,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                var item = _context.items.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                var items = item.Take(1);
                DateTime latest3 = (DateTime)_context.items.Where(d => d.DateCreated.Date == oDate.Date).Max(d => d.DateCompleted);
                var item1 = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateCompleted == latest3).ToList();

                if (item.Any(s => s.Status.StartsWith("D")))
                {

                    foreach (var task in items)
                    {

                        var Task = new Report
                        {
                            Checklist = "Items",
                            Status = task.Status,
                            DateCreated = date,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else if (item.Any(s => !s.Status.StartsWith("D")))
                {
                    foreach (var tasks in item1)
                    {

                        var Task = new Report
                        {
                            Checklist = "Items",
                            Status = tasks.Status,
                            DateCreated = date,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

            }

            if (ReportsToday.Count > 0)
            {
                var bobcat = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).ToList();
                DateTime latest = (DateTime)_context.BobCats.Where(r => r.DateTaskCompleted.HasValue).Max(d => d.DateTaskCompleted);
                var bob = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateTaskCompleted == latest).ToList();
                
                //if (bobcat.Count >= ReportsToday.Count)
                //{
                    if (bobcat.Any(s => s.Status.StartsWith("D")))
                    {
                        //var bobcats = bobcat.Take(1);
                        var result = bobcat.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Take(1);
                        foreach (var task in result)
                        {

                            var Task = new Report
                            {
                                Checklist = "BobCat",
                                Status = task.Status,
                                DateCreated = task.DateCreated,
                                DateCompleted = task.DateAllTaskCompleted,
                                UserName = task.UserName1,
                                AssignedTo = "Operator",
                                TaskCompleted = task.DateTaskCompleted
                            };

                            _context.reports.Add(Task);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else if (bobcat.Any(s => !s.Status.StartsWith("D")))
                    {
                        var result = bobcat.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(d => d.DateTaskCompleted == latest).ToList();
                        foreach (var tasks in result)
                        {

                            var Task = new Report
                            {
                                Checklist = "BobCat",
                                Status = tasks.Status,
                                DateCreated = tasks.DateCreated,
                                DateCompleted = tasks.DateAllTaskCompleted,
                                UserName = tasks.UserName1,
                                AssignedTo = "Operator",
                                TaskCompleted = tasks.DateTaskCompleted
                            };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                        }
                    }
                //}


                var dailyCheck = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).ToList();
                var dailyChecks = dailyCheck.Take(1);
                

                //if (bobcat.Count >= ReportsToday.Count)
                //{
                if (dailyCheck.Any(s => s.Status.StartsWith("D")))
                {
                    //var bobcats = bobcat.Take(1);
                    var result = dailyCheck.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Take(1);
                    foreach (var task in result)
                    {

                        var Task = new Report
                        {
                            Checklist = "Daily Checks",
                            Status = task.Status,
                            DateCreated = task.DateCreated,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateCompleted
                        };

                        _context.reports.Add(Task);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (dailyCheck.Any(s => !s.Status.StartsWith("D")))
                {
                    DateTime latest1 = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).Max(d => d.DateCompleted);
                    var daily = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateCompleted == latest1).ToList();
                    var result = dailyCheck.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(d => d.DateCompleted == latest).ToList();
                    foreach (var tasks in result)
                    {

                        var Task = new Report
                        {
                            Checklist = "Daily Checks",
                            Status = tasks.Status,
                            DateCreated = tasks.DateCreated,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }
                //}

                var maintenance = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).ToList();
                var maintenances = maintenance.Take(1);
                DateTime latest2 = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).Max(d => d.DateCompleted);
                var main = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateCompleted == latest2).ToList();

                //if (bobcat.Count >= ReportsToday.Count)
                //{
                if (maintenance.Any(s => s.Status.StartsWith("D")))
                {
                    //var bobcats = bobcat.Take(1);
                    var result = maintenance.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Take(1);
                    foreach (var task in result)
                    {

                        var Task = new Report
                        {
                            Checklist = "Maintenance",
                            Status = task.Status,
                            DateCreated = task.DateCreated,
                            DateCompleted = task.DateAllCompleted,
                            UserName = task.User,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateCompleted
                        };

                        _context.reports.Add(Task);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (maintenance.Any(s => !s.Status.StartsWith("D")))
                {
                    var result = maintenance.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(d => d.DateCompleted == latest).ToList();
                    foreach (var tasks in result)
                    {

                        var Task = new Report
                        {
                            Checklist = "Maintenance",
                            Status = tasks.Status,
                            DateCreated = tasks.DateCreated,
                            DateCompleted = tasks.DateAllCompleted,
                            UserName = tasks.User,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }
                //}


                var item = _context.items.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                var items = item.Take(1);
                DateTime latest3 = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Max(d => d.DateCompleted);
                var item1 = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateCompleted == latest3).ToList();

                //if (bobcat.Count >= ReportsToday.Count)
                //{
                if (item.Any(s => s.Status.StartsWith("D")))
                {
                    //var bobcats = bobcat.Take(1);
                    var result = item.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Take(1);
                    foreach (var task in result)
                    {

                        var Task = new Report
                        {
                            Checklist = "Items",
                            Status = task.Status,
                            DateCreated = task.DateCreated,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateCompleted
                        };

                        _context.reports.Add(Task);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (item.Any(s => !s.Status.StartsWith("D")))
                {
                    var result = item.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(d => d.DateCompleted == latest).ToList();
                    foreach (var tasks in result)
                    {

                        var Task = new Report
                        {
                            Checklist = "Items",
                            Status = tasks.Status,
                            DateCreated = tasks.DateCreated,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }
                //}


            }

            await _context.SaveChangesAsync();

            return Json(new { data = _context.reports.Where(d => d.DateCreated.Date == oDate.Date).ToList()});
        }

        [HttpGet]
        public async Task<IActionResult> GetWeeklyReportAsync(DateTime date)
        {
            DateTime oDate = Convert.ToDateTime(date);
            var Day = oDate.DayOfWeek.ToString();
            var week = DateTime.Now.AddDays(-6);

            var ReportsToday = _context.reports.Where(d => d.DateCreated >= DateTime.Now.AddDays(-7) && d.DateCreated <= oDate.Date).ToList();

            if (ReportsToday.Count == 0)
            {
                var bobcat = _context.BobCats.Where(d => d.DateCreated == DateTime.Now.AddDays(-7) && d.DateCreated <= oDate.Date).ToList();
                var bob = _context.BobCats.Where(d => d.DateCreated.Date == DateTime.Now.AddDays(-7) && d.DateCreated <= oDate.Date).Where(d => d.DateTaskCompleted == DateTime.MaxValue).ToList();
                if (bobcat.Any(s => s.Status.StartsWith("D")))
                {
                    var bobcats = bobcat.Take(1);

                    foreach (var task in bobcats)
                    {

                        var Task = new Report
                        {
                            Checklist = "BobCat",
                            Status = task.Status,
                            DateCreated = DateTime.Today,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.UserName1,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateTaskCompleted
                        };

                        _context.reports.Add(Task);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    foreach (var tasks in bob)
                    {

                        var Task = new Report
                        {
                            Checklist = "BobCat",
                            Status = tasks.Status,
                            DateCreated = DateTime.Today,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.UserName1,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateTaskCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }


                var dailyCheck = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                var dailyChecks = dailyCheck.Take(1);
                var daily = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateCompleted == DateTime.MaxValue).ToList();

                if (dailyCheck.Any(s => s.Status.StartsWith("D")))
                {

                    foreach (var task in dailyChecks)
                    {

                        var Task = new Report
                        {
                            Checklist = "Daily Checks",
                            Status = task.Status,
                            DateCreated = DateTime.Today,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else
                {
                    foreach (var tasks in daily)
                    {

                        var Task = new Report
                        {
                            Checklist = "Daily Checks",
                            Status = tasks.Status,
                            DateCreated = DateTime.Today,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                var maintenance = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                var maintenances = maintenance.Take(1);
                var main = _context.Maintenances.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateCompleted == DateTime.MaxValue).ToList();

                if (maintenance.Any(s => s.Status.StartsWith("D")))
                {

                    foreach (var task in maintenances)
                    {

                        var Task = new Report
                        {
                            Checklist = "Maintenance",
                            Status = task.Status,
                            DateCreated = DateTime.Today,
                            DateCompleted = task.DateAllCompleted,
                            UserName = task.User,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else
                {
                    foreach (var tasks in main)
                    {

                        var Task = new Report
                        {
                            Checklist = "Maintenance",
                            Status = tasks.Status,
                            DateCreated = DateTime.Today,
                            DateCompleted = tasks.DateAllCompleted,
                            UserName = tasks.User,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                var item = _context.items.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                var items = item.Take(1);
                var items1 = _context.items.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateCompleted == DateTime.MaxValue).ToList();

                if (item.Any(s => s.Status.StartsWith("D")))
                {

                    foreach (var task in items)
                    {

                        var Task = new Report
                        {
                            Checklist = "Items",
                            Status = task.Status,
                            DateCreated = DateTime.Today,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else
                {
                    foreach (var tasks in items)
                    {

                        var Task = new Report
                        {
                            Checklist = "Items",
                            Status = tasks.Status,
                            DateCreated = DateTime.Today,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.UserName,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateCompleted
                        };

                        _context.reports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

            }

            await _context.SaveChangesAsync();

            return Json(new { data = _context.reports.Where(d => d.DateCreated.Date == oDate.Date).ToList() });
        }
        #endregion
    }
}
