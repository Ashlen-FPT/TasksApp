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
    public class DailyChecksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyChecksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DailyChecks
        public async Task<IActionResult> Index()
        {
            return View(await _context.DailyChecks.ToListAsync());
        }

        // GET: DailyChecks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyCheck = await _context.DailyChecks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyCheck == null)
            {
                return NotFound();
            }

            return View(dailyCheck);
        }

        // GET: DailyChecks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DailyChecks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReportHeading,ReportDesc,IsDone,Remarks")] DailyCheck dailyCheck)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dailyCheck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dailyCheck);
        }

        // GET: DailyChecks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyCheck = await _context.DailyChecks.FindAsync(id);
            if (dailyCheck == null)
            {
                return NotFound();
            }
            return View(dailyCheck);
        }

        // POST: DailyChecks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReportHeading,ReportDesc,IsDone,Remarks")] DailyCheck dailyCheck)
        {
            if (id != dailyCheck.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dailyCheck);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DailyCheckExists(dailyCheck.Id))
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
            return View(dailyCheck);
        }

        // GET: DailyChecks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dailyCheck = await _context.DailyChecks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dailyCheck == null)
            {
                return NotFound();
            }

            return View(dailyCheck);
        }

        // POST: DailyChecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dailyCheck = await _context.DailyChecks.FindAsync(id);
            _context.DailyChecks.Remove(dailyCheck);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DailyCheckExists(int id)
        {
            return _context.DailyChecks.Any(e => e.Id == id);
        }

        #region API Calls


        [HttpGet]
        public async Task<IActionResult> GetTasksTodayAsync(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).ToList();


            if (TasksToday.Count == 0)
            {
                var Main_Task = _context.TemplateDailyChecks.ToList();

                foreach (var task in Main_Task)
                {

                    var Task = new DailyCheck
                    {
                        ReportDesc = task.Description,
                        ReportHeading = task.Heading,
                        DateCreated = date,
                        DateCompleted = new DateTime(),
                        Number = task.HeadNo,
                    };

                    _context.DailyChecks.Add(Task);

                }
            }

            if (TasksToday.Count > 0)
            {
                var Main_Task = _context.TemplateDailyChecks.ToList();

                if (Main_Task.Count > TasksToday.Count)
                {
                    var result = Main_Task.Where(p => TasksToday.All(p2 => p2.ReportDesc != p.Description)).Where(q => TasksToday.All(q2 => q2.Number != q.HeadNo)).Where(x => TasksToday.All(x2 => x2.ReportHeading != x.Heading));

                    foreach (var item in result)
                    {
                        var Task = new DailyCheck
                        {
                            ReportDesc = item.Description,
                            ReportHeading = item.Heading,
                            DateCreated = date,
                            DateCompleted = new DateTime(),
                            Number = item.HeadNo,
                        };

                        _context.DailyChecks.Add(Task);
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
                UpdatedTable = "DailyChecks",
                OldData = "Read Daily Checks Checklist",
                NewData = null
            };

            _context.Logs.Add(log);

            await _context.SaveChangesAsync();

            return Json(new { data = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date) });
        }


        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date)});

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {


            var task = _context.DailyChecks.Find(id);
            task.IsDone = true;
            task.DateCompleted = DateTime.Now;
            //task.User = User.Identity.Name;

            var date = task.DateCreated;

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Completed,
                DateTime = DateTime.Now,
                UpdatedTable = "DailyChecks",
                OldData = null,
                NewData = "Task Completed"
            };

            _context.Logs.Add(log);
            _context.SaveChanges();


            var tasks = _context.DailyChecks.Where(d => d.DateCreated == date).ToList();

            foreach (var item in tasks)
            {

                if (item.IsDone == false)
                {
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Task Completed!" });
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


            var task = _context.DailyChecks.Find(id);

            task.Remarks = comment;


            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Comment added!" });

        }

        [HttpGet]
        public IActionResult GetTask(int id)
        {
            var task = _context.DailyChecks.Find(id);
            return Json(task);

        }

        #endregion
    }
}
