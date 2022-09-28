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

            var ReportsToday = _context.mreports.Where(d => d.DateCreated.Date == oDate.Date).ToList();

            if (ReportsToday.Count == 0)
            {
                var active_D = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).ToList();
               
                if (active_D.Any(s => s.Status.StartsWith("D")))
                {
                    var actives = active_D.Where(s => s.Status.StartsWith("D"));

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

                else if (active_D.Any(s => s.Status.StartsWith("P")))
                { 
                    var active = active_D.Where(s => s.Status.StartsWith("P"));

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

                else if (active_D.Any(s => s.Status.StartsWith("C")))
                {
                    var active = active_D.Where(s => s.Status.StartsWith("C"));

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

                if (hardware.Any(s => s.Status.StartsWith("D")))
                {
                    var hardwares = hardware.Where(s => s.Status.StartsWith("D"));
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

                else if (hardware.Any(s => s.Status.StartsWith("P")))
                {
                   var daily = hardware.Where(s => s.Status.StartsWith("P"));

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

                else if (hardware.Any(s => s.Status.StartsWith("C")))
                {
                    var hardwares = hardware.Where(s => s.Status.StartsWith("C")); ;
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



                var networking = _context.Networking.Where(d => d.DateCreated.Date == oDate.Date).ToList();
               
                
                if (networking.Any(s => s.Status.StartsWith("D")))
                {
                    var networks = networking.Where(s => s.Status.StartsWith("D")); 
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

                else if (networking.Any(s => s.Status.StartsWith("P")))
                {
                    var network = networking.Where(s => s.Status.StartsWith("P"));
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

                else if (networking.Any(s => s.Status.StartsWith("C")))
                {
                    var networks = networking.Where(s => s.Status.StartsWith("C"));
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


                var securities = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).ToList();
                if (securities.Any(s => s.Status.StartsWith("D")))
                {
                    var security = securities.Where(s => s.Status.StartsWith("D"));
                    foreach (var task in security)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Software",
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

                else if (securities.Any(s => s.Status.StartsWith("P")))
                {
                    DateTime latest3 = (DateTime)_context.Security.Where(d => d.DateCreated.Date == oDate.Date).Where(r => r.DateTaskCompleted.HasValue).Max(d => d.DateTaskCompleted);
                    var secure = securities.Where(s => s.Status.StartsWith("P"));

                    foreach (var tasks in secure)
                    {
                        var Task = new MReport
                        {
                            Checklist = "Software",
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

                else if(securities.Any(s => s.Status.StartsWith("C")))
                {

                    foreach (var task in securities)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Software",
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


                var sofwares = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).ToList();
            
                if (sofwares.Any(s => s.Status.StartsWith("D")))
                {
                    var sofware = sofwares.Where(s => s.Status.StartsWith("D"));
                    foreach (var task in sofware)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Security",
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

                else if (sofwares.Any(s => s.Status.StartsWith("P")))
                {
                    var soft = sofwares.Where(s => s.Status.StartsWith("P"));

                    foreach (var tasks in soft)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Security",
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
                
                else if(sofwares.Any(s => s.Status.StartsWith("C")))
                {
                    var sofware = sofwares.Where(s => s.Status.StartsWith("C"));
                    foreach (var task in sofware)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Security",
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

            }

            if (ReportsToday.Count > 0)
            {
                var active_D = _context.Active_D.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                if (active_D.Any(s => s.Status.StartsWith("D")))
                {
                    var actives = active_D.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("D"));

                    foreach (var task in actives)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Active Directory",
                            Status = task.Status,
                            DateCreated = task.DateCreated,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.User,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        await _context.SaveChangesAsync();
                    }
                }

                else if (active_D.Any(s => s.Status.StartsWith("P")))
                {var active = active_D.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("P"));

                    foreach (var tasks in active)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Active Directory",
                            Status = tasks.Status,
                            DateCreated = tasks.DateCreated,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.User,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else if (active_D.Any(s => s.Status.StartsWith("C")))
                {
                    var actives = active_D.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("C"));

                    foreach (var task in actives)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Active Directory",
                            Status = task.Status,
                            DateCreated = task.DateCreated,
                            DateCompleted = task.DateAllTaskCompleted,
                            UserName = task.User,
                            AssignedTo = "Operator",
                            TaskCompleted = task.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        await _context.SaveChangesAsync();
                    }
                }


                var hardware = _context.Hardware.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                if (hardware.Any(s => s.Status.StartsWith("D")))
                {
                    var hardwares = hardware.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("D"));
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

                else if (hardware.Any(s => s.Status.StartsWith("P")))
                {
                    var daily = hardware.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("P"));

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

                else if (hardware.Any(s => s.Status.StartsWith("C")))
                {
                    var hardwares = hardware.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("C"));
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


                var networking = _context.Networking.Where(d => d.DateCreated.Date == oDate.Date).ToList();
                
                if (networking.Any(s => s.Status.StartsWith("D")))
                {
                    var networks = networking.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("D"));
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

                else if (networking.Any(s => s.Status.StartsWith("P")))
                {
                    var network = networking.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("P"));
                    foreach (var tasks in network)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Networking",
                            Status = tasks.Status,
                            DateCreated = tasks.DateCreated,
                            DateCompleted = tasks.DateAllTaskCompleted,
                            UserName = tasks.User,
                            AssignedTo = "Operator",
                            TaskCompleted = tasks.DateTaskCompleted
                        };

                        _context.mreports.Add(Task);
                        // await _context.SaveChangesAsync();
                    }
                }

                else if (networking.Any(s => s.Status.StartsWith("C")))
                {
                    var networks = networking.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("C"));
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


                var securities = _context.Security.Where(d => d.DateCreated.Date == oDate.Date).ToList();

                if (securities.Any(s => s.Status.StartsWith("D")))
                {
                    var security = securities.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("D"));
                    foreach (var task in security)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Software",
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

                else if (securities.Any(s => s.Status.StartsWith("P")))
                {
                    var secure = securities.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("P"));

                    foreach (var tasks in secure)
                    {
                        var Task = new MReport
                        {
                            Checklist = "Software",
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

                else if (securities.Any(s => s.Status.StartsWith("C")))
                {
                    var security = securities.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("C"));
                    foreach (var task in security)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Software",
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


                var sofwares = _context.Software.Where(d => d.DateCreated.Date == oDate.Date).ToList();
                
                if (sofwares.Any(s => s.Status.StartsWith("D")))
                {
                    var sofware = sofwares.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("D"));
                    foreach (var task in sofware)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Security",
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

                else if (sofwares.Any(s => s.Status.StartsWith("P")))
                {
                    var soft = sofwares.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("P"));

                    foreach (var tasks in soft)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Security",
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

                else if (sofwares.Any(s => s.Status.StartsWith("C")))
                {
                    var sofware = sofwares.Where(p => ReportsToday.All(p2 => p2.Status != p.Status)).Where(s => s.Status.StartsWith("C"));
                    foreach (var task in sofware)
                    {

                        var Task = new MReport
                        {
                            Checklist = "Security",
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

            }

            await _context.SaveChangesAsync();
            return Json(new { data = _context.mreports.Where(d => d.DateCreated.Date == oDate.Date).ToList() });
        }
        #endregion
    }
}
