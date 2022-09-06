using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TasksApp.Data;
using TasksApp.Enums;
using TasksApp.Models;

namespace TasksApp.Controllers
{
    public class BobCatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BobCatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BobCats
        public async Task<IActionResult> Index()
        {

            return View(await _context.BobCats.ToListAsync());
        }



        public async Task<IActionResult> Capture(BobCat b)
        {
            var date = DateTime.Now.ToShortDateString();
            var bobCat = new BobCat
            {
                DateCreated = Convert.ToDateTime(date),
                UserName1 = User.FindFirst("Username")?.Value,
                UserName2 = User.FindFirst("Username")?.Value,
                Sign1 = b.Sign1,
                Sign2 = b.Sign2
            };
            _context.BobCats.Add(bobCat);
            await _context.SaveChangesAsync();

            return View();

        }

        // GET: BobCats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bobCat = await _context.BobCats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bobCat == null)
            {
                return NotFound();
            }

            return View(bobCat);
        }

        // GET: BobCats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BobCats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Yes,No,NA")] BobCat bobCat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bobCat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bobCat);
        }

        // GET: BobCats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bobCat = await _context.BobCats.FindAsync(id);
            if (bobCat == null)
            {
                return NotFound();
            }
            return View(bobCat);
        }

        // POST: BobCats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Yes,No,NA")] BobCat bobCat)
        {
            if (id != bobCat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bobCat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BobCatExists(bobCat.Id))
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
            return View(bobCat);
        }

        // GET: BobCats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bobCat = await _context.BobCats
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bobCat == null)
            {
                return NotFound();
            }

            return View(bobCat);
        }

        // POST: BobCats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bobCat = await _context.BobCats.FindAsync(id);
            _context.BobCats.Remove(bobCat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BobCatExists(int id)
        {
            return _context.BobCats.Any(e => e.Id == id);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetTasksTodayAsync(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).ToList();


            if (TasksToday.Count == 0)
            {
                var Main_Task = _context.TemplateBobCat.ToList();

                foreach (var task in Main_Task)
                {

                    var Task = new BobCat
                    {
                        Main = task.Heading,
                        Number = task.TaskNo,
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Status = "Do-Checklist"
                    };

                    _context.BobCats.Add(Task);

                }
            }

            if (TasksToday.Count > 0)
            {
                var TemplateBobCat = _context.TemplateBobCat.ToList();

                if (TemplateBobCat.Count > TasksToday.Count)
                {
                    var result = TemplateBobCat.Where(p => TasksToday.All(p2 => p2.Description != p.Description)).Where(q => TasksToday.All(q2 => q2.Number != q.TaskNo)).Where(x => TasksToday.All(x2 => x2.Main != x.Heading));

                    foreach (var item in result)
                    {
                        var Task = new BobCat
                        {
                            Main = item.Heading,
                            Number = item.TaskNo,
                            Description = item.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Status = "Do-Checklist"
                        };

                        _context.BobCats.Add(Task);
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
                UpdatedTable = "BobCat",
                OldData = "Read BobCat Checklist",
                NewData = null
            };


            _context.Logs.Add(log);
            

            await _context.SaveChangesAsync();

            return Json(new { data = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date) });
        }

        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {
            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date) });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {

            var task = _context.BobCats.Find(id);
            task.Yes = true;
            task.No = false;
            task.NA = false;
            task.DateTaskCompleted = DateTime.Now;
            task.Status = "Partially Completed";
            task.isDone = true;
            //task.User = User.Identity.Name;
            var date = task.DateCreated;


            

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Completed,
                DateTime = DateTime.Now,
                UpdatedTable = "BobCat",
                OldData = null,
                NewData = "Task Completed"
            };
            _context.Logs.Add(log);
            _context.SaveChanges();


            var tasks = _context.BobCats.Where(d => d.DateCreated == date).ToList();

            if(tasks.All(c => c.isDone == true))
            {
                task.DateAllTaskCompleted = DateTime.Now;
                task.Status = "Completed";
                _context.SaveChanges();
            }

            foreach (var item in tasks)
            {
                //if (item.Status == null)
                //{
                //    task.Status = "Do-CheckList";
                //    await _context.SaveChangesAsync();
                //}

                if (item.Yes == false)
                {
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Task Completed!" });
                }

                else if (item.No == false)
                {
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Task Completed!" });
                }

                else if (item.NA == false)
                {
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Task Completed!" });


                    //else
                    //{
                    //    bool completeTasks = tasks.All(c => c.IsDone == true);
                    //    {
                    //        if (completeTasks == false)
                    //        {

                    //            await _context.SaveChangesAsync();

                    //            return Json(new { success = true, message = "Task Completed!" });
                    //        }
                    //        else if (completeTasks == true)
                    //        {
                    //            task.TasksCompleted = true;
                    //            task.DateAllTaskCompleted = DateTime.Now;
                    //            task.Status = "Completed";
                    //        }
                    //    }
                }

            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "All Tasks Completed!" });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteAllTasks()
        {
            var task = _context.BobCats.ToList();

            foreach (var item in task)
            {
                if (item.Yes == true)
                {
                    //item.TasksCompleted = true;
                    //item.DateAllTaskCompleted = DateTime.Now;
                }

            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "All Tasks Completed!" });
        }

        //public async Task<IActionResult> AddComment(int id, string comment)
        //{


        //    var task = _context.BobCats.Find(id);

        //    task.Comments = comment;


        //    await _context.SaveChangesAsync();

        //    return Json(new { success = true, message = "Comment added!" });

        //}

        [HttpGet]
        public IActionResult GetTask(int id)
        {
            var task = _context.BobCats.Find(id);
            return Json(task);

        }

        //[HttpPost]
        //public async Task<IActionResult> AddBobCat(string Desc, string Head)
        //{

        //    var bobCat = new TemplateBobCat
        //    {
        //        Heading = Head,
        //        Description = Desc,
        //        DateCreated = DateTime.Now,

        //    };

        //    _context.Add(bobCat);
        //    await _context.SaveChangesAsync();

        //    return Json(new { success = true, message = "Sub-Task added!" });

        //}
        #endregion
    }
}
