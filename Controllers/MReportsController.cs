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
    public class MReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MReports
        public async Task<IActionResult> Index()
        {
            return View(await _context.mreports.ToListAsync());
        }

        // GET: MReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mReport = await _context.mreports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mReport == null)
            {
                return NotFound();
            }

            return View(mReport);
        }

        // GET: MReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Checklist,DateCreated,DateCompleted,Status,AssignedTo,UserName,TaskCompleted")] MReport mReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mReport);
        }

        // GET: MReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mReport = await _context.mreports.FindAsync(id);
            if (mReport == null)
            {
                return NotFound();
            }
            return View(mReport);
        }

        // POST: MReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Checklist,DateCreated,DateCompleted,Status,AssignedTo,UserName,TaskCompleted")] MReport mReport)
        {
            if (id != mReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MReportExists(mReport.Id))
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
            return View(mReport);
        }

        // GET: MReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mReport = await _context.mreports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mReport == null)
            {
                return NotFound();
            }

            return View(mReport);
        }

        // POST: MReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mReport = await _context.mreports.FindAsync(id);
            _context.mreports.Remove(mReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MReportExists(int id)
        {
            return _context.mreports.Any(e => e.Id == id);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetReportAsync(DateTime date)
        {
            DateTime oDate = Convert.ToDateTime(date);

            var ReportsToday = _context.reports.Where(d => d.DateCreated.Date == oDate.Date).ToList();

            if (ReportsToday.Count == 0)
            {
                var active_D = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).ToList();
                DateTime latest = (DateTime)_context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(r => r.DateTaskCompleted.HasValue).Max(d => d.DateTaskCompleted);
                var active = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateTaskCompleted == latest).ToList();
                //var bob = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateTaskCompleted == DateTime.MaxValue).ToList();

                if (active_D.Any(s => s.Status.StartsWith("D")))
                {
                    var actives = active_D.Take(1);

                    foreach (var task in actives)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Active Directory",
                            Status = task.Status,
                            DateCreated = DateTime.Today,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.User,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        await _context.SaveChangesAsync();
                    }
                }

                else if (active_D.Any(s => !s.Status.StartsWith("D")))
                {
                    foreach (var tasks in active)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Active Directory",
                            Status = tasks.Status,
                            DateCreated = date,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.User,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                var hardware = _context.Hardware.Where(d => d.DateCreated.Date == oDate.Date).ToList();
                var hardwares = hardware.Take(1);
                DateTime latest1 = (DateTime)_context.Hardware.Where(d => d.DateCreated.Date == oDate.Date).Where(r => r.DateTaskCompleted.HasValue).Max(d => d.DateTaskCompleted);
                var daily = _context.Hardware.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateTaskCompleted == latest1).ToList();

                if (hardware.Any(s => s.Status.StartsWith("D")))
                {

                    foreach (var task in hardwares)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Hardware",
                            Status = task.Status,
                            DateCreated = date,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.User,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else if (hardware.Any(s => !s.Status.StartsWith("D")))
                {
                    foreach (var tasks in daily)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Hardware",
                            Status = tasks.Status,
                            DateCreated = date,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.User,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                var networking = _context.Networking.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                var networks = networking.Take(1);
                DateTime latest2 = (DateTime)_context.Networking.Where(d => d.DateCreated.Date == oDate.Date).Where(r => r.DateTaskCompleted.HasValue).Max(d => d.DateTaskCompleted);
                var network = _context.Networking.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateTaskCompleted == latest2).ToList();

                if (networking.Any(s => s.Status.StartsWith("D")))
                {

                    foreach (var task in networks)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Networking",
                            Status = task.Status,
                            DateCreated = date,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.User,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else if (networking.Any(s => !s.Status.StartsWith("D")))
                {
                    foreach (var tasks in network)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Networking",
                            Status = tasks.Status,
                            DateCreated = date,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.User,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                var securities = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                var security = securities.Take(1);
                DateTime latest3 = (DateTime)_context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(r => r.DateTaskCompleted.HasValue).Max(d => d.DateTaskCompleted);
                var secure = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateTaskCompleted == latest3).ToList();

                if (securities.Any(s => s.Status.StartsWith("D")))
                {

                    foreach (var task in security)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Items",
                            Status = task.Status,
                            DateCreated = date,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.User,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else if (securities.Any(s => !s.Status.StartsWith("D")))
                {
                    foreach (var tasks in secure)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Items",
                            Status = tasks.Status,
                            DateCreated = date,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.User,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                var sofwares = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                var sofware = sofwares.Take(1);
                DateTime latest4 = (DateTime)_context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(r => r.DateTaskCompleted.HasValue).Max(d => d.DateTaskCompleted);
                var soft = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).Where(d => d.DateTaskCompleted == latest4).ToList();

                if (sofwares.Any(s => s.Status.StartsWith("D")))
                {

                    foreach (var task in sofware)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Items",
                            Status = task.Status,
                            DateCreated = date,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.User,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else if (sofwares.Any(s => !s.Status.StartsWith("D")))
                {
                    foreach (var tasks in soft)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Items",
                            Status = tasks.Status,
                            DateCreated = date,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.User,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

            }
            await _context.SaveChangesAsync();

            return Json(new { data = _context.mreports.Where(d => d.DateCreated.Date == oDate.Date).ToList() });
        }
        #endregion
    }
}
