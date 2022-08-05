using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TasksApp.Data;
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
        public IActionResult GetAll()
        {
            return Json(new { data = _context.BobCats.ToList() });

        }

        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {

            var task = _context.BobCats.Find(id);
            task.Yes = true;
            task.No = false;
            task.NA = false;
            //task.Status = "Partialy Complete";

            ///ar date = task.DateCreated;

            var tasks = _context.BobCats.ToList();

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

            //foreach (var item in task)
            //{
            //    if (item.IsDone == true)
            //    {
            //        item.TasksCompleted = true;
            //        item.DateAllTaskCompleted = DateTime.Now;
            //    }

            //}
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "All Tasks Completed!" });
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
