using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
        public IActionResult GetSignOffs(DateTime date)
        {
            DateTime oDate = Convert.ToDateTime(date);

            var SignsToday = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).ToList().Take(1);

            return Json(new { data = SignsToday });
        }

        [HttpGet]
        public async Task<IActionResult> SignOff(int id)
        {
            var Main_Task = _context.TemplateBobCat.ToList();
            var BobCats = _context.BobCats.ToList();
            var last = Main_Task.LastOrDefault();
            var count = Main_Task.Count();
            var DateCreation = new DateTime();
            var Ddate = _context.BobCats.Find(id).DateCreated;
            var ChangeAllSignatures = _context.BobCats.Where(x => x.DateCreated == Ddate).ToList();
            var Btasks = _context.BobCats.Find(id);
            //Get Last Item & Save Signature
            var items = BobCats.Where((x, i) => i % count == count - 1);
            var ItemDate = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.DateCreated).FirstOrDefault();
            var ItemStatus = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Status).FirstOrDefault();
            var ItemId = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Id).FirstOrDefault();
            var SaveSignature = _context.BobCats.Find(ItemId);


            if (User.IsInRole(SD.Role_Operator.ToString()))
            {
                //Btasks.Sign1 = true;
                //Btasks.Sign2 = Btasks.Sign2;
                foreach (var item in ChangeAllSignatures)
                {
                    bool completion = ChangeAllSignatures.All(c => c.isDone == true);
                    {
                        if (completion == false)
                        { 
                            item.Sign1 = false;

                            await _context.SaveChangesAsync();

                            return Json(new { warning = true, message = "Please Complete All Checklist Items!" });
                        }

                        if (completion == true)
                        {
                            item.Sign1 = true;
                            item.DateAllTaskCompleted = DateTime.Now;
                        }
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Operator Signed!" });
                    }
                    
                }

                var log = new Logs
                {
                    UserName = User.FindFirst("Username")?.Value,
                    UserEmail = User.Identity.Name,
                    Entity = User.FindFirst("Organization")?.Value,
                    LogType = LogTypes.Completed,
                    DateTime = DateTime.Now,
                    UpdatedTable = "BobCat",
                    OldData = null,
                    NewData = "Checklist Signed By Operator"
                };
                _context.Logs.Add(log);

            }
            else
            if (User.IsInRole(SD.Role_Supervisor.ToString()))
            {
                Btasks.Sign1 = Btasks.Sign1;
                foreach (var item in ChangeAllSignatures)
                {
                    item.Sign2 = true;
                    item.UserName2= User.FindFirst("Username")?.Value;
                }

                var log = new Logs
                {
                    UserName = User.FindFirst("Username")?.Value,
                    UserEmail = User.Identity.Name,
                    Entity = User.FindFirst("Organization")?.Value,
                    LogType = LogTypes.Completed,
                    DateTime = DateTime.Now,
                    UpdatedTable = "BobCat",
                    OldData = null,
                    NewData = "Checklist Signed By Supervisor"
                };
                _context.Logs.Add(log);
            }
            //Btasks.DateAllTaskCompleted = DateTime.Now;
            _context.SaveChanges();



            var tasks = _context.BobCats.Where(d => d.DateCreated == DateCreation).ToList();


            //    foreach (var item in tasks)
            //    {

            //        if (item.Sign1 == true)
            //    {

            //        return Json(new { success = true, message = "Operator Signed" });
            //    }

            //    if (item.Sign2 == true)
            //    {

            //        return Json(new { success = true, message = "Supervisor Signed" });
            //    }

            //}
            if (User.IsInRole(SD.Role_Operator.ToString()))
            {
                return Json(new { success = true, message = "Operator Signed" });
            }

            if (User.IsInRole(SD.Role_Supervisor.ToString()))
            {
                return Json(new { success = true, message = "Supervisor Signed" });
            }

                await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Checklist SignOffs Completed" });

        }

        [HttpGet]
        public async Task<IActionResult> GetTasksTodayAsync(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).ToList();


            if (TasksToday.Count == 0)
            {
                var Main_Task = _context.TemplateBobCat.ToList();
                var last = Main_Task.LastOrDefault();

                foreach (var task in Main_Task)
                {

                    var Task = new BobCat
                    {
                        Main = task.Heading,
                        Number = task.TaskNo,
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Status = "Task : Incomplete",
                        UserName1 = User.FindFirst("Username")?.Value,
                        DateAllTaskCompleted = new DateTime()
                    };
                    if (task == last)
                    {
                        Task.Status = "Do-Checklist : BobCat";
                    }
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
                            DateAllTaskCompleted = new DateTime()
                            //Status = "Do-Checklist"
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
            var Main_Task = _context.TemplateBobCat.ToList();
            var BobCats = _context.BobCats.ToList();
            var last = Main_Task.LastOrDefault();
            var count = Main_Task.Count();
            var DateCreation = new DateTime();
            var Ddate = _context.BobCats.Find(id).DateCreated;
            var Btasks = _context.BobCats.Find(id);
            //Get Last Item & Change Status
            var items = BobCats.Where((x, i) => i % count == count - 1);
            var ItemDate = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.DateCreated).FirstOrDefault();
            var ItemStatus = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Status).FirstOrDefault();
            var ItemId = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Id).FirstOrDefault();
            var ChangeStatus = _context.BobCats.Find(ItemId);

            var date = Btasks.DateCreated;
            var task = _context.BobCats.Where(d => d.DateCreated == date).ToList();


            Btasks.Yes = true;
            Btasks.No = false;
            Btasks.NA = false;
            Btasks.DateTaskCompleted = DateTime.Now;
            Btasks.isDone = true;
            DateCreation = Btasks.DateCreated;
            Btasks.Status = "Task : Completed";


            if (ItemDate == Ddate)
            {
                ChangeStatus.Status = "Partially Completed : BobCat";
            }

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



            var tasks = _context.BobCats.Where(d => d.DateCreated == DateCreation).ToList();

            if (tasks.All(c => c.isDone == true))
            {

                if (ItemDate == Ddate)
                {
                    Btasks.DateAllTaskCompleted = DateTime.Now;
                    ChangeStatus.Status = "Completed : BobCat";
                }
            }

            foreach (var item in tasks)
            {

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

                }

            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "All Tasks Completed!" });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTaskNo(int id)
        {
            var Main_Task = _context.TemplateBobCat.ToList();
            var BobCats = _context.BobCats.ToList();
            var last = Main_Task.LastOrDefault();
            var count = Main_Task.Count();
            var DateCreation = new DateTime();
            var Ddate = _context.BobCats.Find(id).DateCreated;
            var Btasks = _context.BobCats.Find(id);
            //Get Last Item & Change Status
            var items = BobCats.Where((x, i) => i % count == count - 1);
            var ItemDate = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.DateCreated).FirstOrDefault();
            var ItemStatus = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Status).FirstOrDefault();
            var ItemId = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Id).FirstOrDefault();
            var ChangeStatus = _context.BobCats.Find(ItemId);

            var date = Btasks.DateCreated;
            var task = _context.BobCats.Where(d => d.DateCreated == date).ToList();


            Btasks.Yes = false;
            Btasks.No = true;
            Btasks.NA = false;
            Btasks.DateTaskCompleted = DateTime.Now;
            Btasks.isDone = true;
            DateCreation = Btasks.DateCreated;
            Btasks.Status = "Task : Completed";


            if (ItemDate == Ddate)
            {
                ChangeStatus.Status = "Partially Completed : BobCat";
            }

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



            var tasks = _context.BobCats.Where(d => d.DateCreated == DateCreation).ToList();

            if (tasks.All(c => c.isDone == true))
            {

                if (ItemDate == Ddate)
                {
                    Btasks.DateAllTaskCompleted = DateTime.Now;
                    ChangeStatus.Status = "Completed : BobCat";
                }
            }

            foreach (var item in tasks)
            {

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

                }

            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "All Tasks Completed!" });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTaskNA(int id)
        {
            var Main_Task = _context.TemplateBobCat.ToList();
            var BobCats = _context.BobCats.ToList();
            var last = Main_Task.LastOrDefault();
            var count = Main_Task.Count();
            var DateCreation = new DateTime();
            var Ddate = _context.BobCats.Find(id).DateCreated;
            var Btasks = _context.BobCats.Find(id);
            //Get Last Item & Change Status
            var items = BobCats.Where((x, i) => i % count == count - 1);
            var ItemDate = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.DateCreated).FirstOrDefault();
            var ItemStatus = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Status).FirstOrDefault();
            var ItemId = items.Where(x => x.DateCreated == Ddate.Date).Select(x => x.Id).FirstOrDefault();
            var ChangeStatus = _context.BobCats.Find(ItemId);

            var date = Btasks.DateCreated;
            var task = _context.BobCats.Where(d => d.DateCreated == date).ToList();


            Btasks.Yes = false;
            Btasks.No = false;
            Btasks.NA = true;
            Btasks.DateTaskCompleted = DateTime.Now;
            Btasks.isDone = true;
            DateCreation = Btasks.DateCreated;
            Btasks.Status = "Task : Completed";


            if (ItemDate == Ddate)
            {
                ChangeStatus.Status = "Partially Completed : BobCat";
            }

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



            var tasks = _context.BobCats.Where(d => d.DateCreated == DateCreation).ToList();

            if (tasks.All(c => c.isDone == true))
            {

                if (ItemDate == Ddate)
                {
                    Btasks.DateAllTaskCompleted = DateTime.Now;
                    ChangeStatus.Status = "Completed : BobCat";
                }
            }

            foreach (var item in tasks)
            {

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


        [HttpGet]
        public async Task<IActionResult> AdminGetTasks(DateTime date, bool Yes)
        {

            DateTime oDate = Convert.ToDateTime(date);

            var TasksToday = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date).ToList();


            if (TasksToday.Count == 0)
            {
                var TemplateTasks = _context.TemplateBobCat.ToList();

                foreach (var task in TemplateTasks)
                {

                    var Task = new BobCat
                    {
                        Main = task.Heading,
                        Number = task.TaskNo,
                        Description = task.Description,
                        DateCreated = date,
                        DateTaskCompleted = new DateTime(),
                        Status = "Task : Incomplete",
                        UserName1 = User.FindFirst("Username")?.Value,
                        DateAllTaskCompleted = new DateTime(),
                        Yes = Yes
                    };

                    _context.BobCats.Add(Task);

                }
            }

            if (TasksToday.Count > 0)
            {
                var TemplateTasks = _context.TemplateBobCat.ToList();

                if (TemplateTasks.Count > TasksToday.Count)
                {
                    var result = TemplateTasks.Where(p => TasksToday.All(p2 => p2.Description != p.Description));

                    foreach (var task in result)
                    {
                        var Task = new BobCat
                        {
                            Main = task.Heading,
                            Number = task.TaskNo,
                            Description = task.Description,
                            DateCreated = date,
                            DateTaskCompleted = new DateTime(),
                            Status = "Task : Incomplete",
                            UserName1 = User.FindFirst("Username")?.Value,
                            DateAllTaskCompleted = new DateTime()
                        };

                        _context.BobCats.Add(Task);
                    }

                }
            }


            await _context.SaveChangesAsync();

            return Json(new { data = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date) });
        }

        [HttpGet]
        public IActionResult AdminGetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.BobCats.Where(d => d.DateCreated.Date == oDate.Date)});

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
