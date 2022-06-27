using System;
using System.Linq;
using TasksApp.Data;
using TasksApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TasksApp.Controllers
{
    [Authorize]
    public class Main_TaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Main_TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Main_Task
        public async Task<IActionResult> Index()
        {
            return View(await _context.Main_Task.ToListAsync());
        }

        // GET: Main_Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var main_Task = await _context.Main_Task
                .FirstOrDefaultAsync(m => m.Id == id);
            if (main_Task == null)
            {
                return NotFound();
            }

            return View(main_Task);
        }

        // GET: Main_Task/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Main_Task/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Schedule,DateCreated,UserEmail,TaskCategory")] Main_Task main_Task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(main_Task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(main_Task);
        }

        // GET: Main_Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var main_Task = await _context.Main_Task.FindAsync(id);
            if (main_Task == null)
            {
                return NotFound();
            }
            return View(main_Task);
        }

        // POST: Main_Task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Schedule,DateCreated,UserEmail,TaskCategory")] Main_Task main_Task)
        {
            if (id != main_Task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(main_Task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Main_TaskExists(main_Task.Id))
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
            return View(main_Task);
        }

        // GET: Main_Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var main_Task = await _context.Main_Task
                .FirstOrDefaultAsync(m => m.Id == id);
            if (main_Task == null)
            {
                return NotFound();
            }

            return View(main_Task);
        }

        // POST: Main_Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var main_Task = await _context.Main_Task.FindAsync(id);
            _context.Main_Task.Remove(main_Task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Main_TaskExists(int id)
        {
            return _context.Main_Task.Any(e => e.Id == id);
        }
        #region API Calls

        [HttpGet]
        public IActionResult GetDaily()
        {

            return Json(new { data = _context.Main_Task.Where(s => s.Schedule == "Daily").ToList() });

        }

        [HttpGet]
        public IActionResult GetWeekly()
        {

            return Json(new { data = _context.Main_Task.Where(s => s.Schedule == "Weekly").ToList() });

        }

        [HttpGet]
        public IActionResult GetMonthly()
        {

            return Json(new { data = _context.Main_Task.Where(s => s.Schedule == "Monthly").ToList() });

        }

        [HttpGet]
        public IActionResult GetQuarterly()
        {
            return Json(new { data = _context.Main_Task.Where(s => s.Schedule == "Quarterly").ToList() });

        }

        [HttpGet]
        public IActionResult GetBi()
        {
            return Json(new { data = _context.Main_Task.Where(s => s.Schedule == "Bi_Annually").ToList() });

        }

        [HttpGet]
        public IActionResult GetAnually()
        {
            return Json(new { data = _context.Main_Task.Where(s => s.Schedule == "Annually").ToList() });

        }

        [HttpPost]
        public async Task<IActionResult> AddTask(string Desc, string Schedule, string Day, string Month, string CType, string Annual, string Quarter, string Bi_Annually)
        {

            var Task = new Main_Task
            {
                Description = Desc,
                Schedule = Schedule,
                DayOfWeek = Day,
                Month = Month,
                Quarterly = Quarter,
                Bi_Annual = Bi_Annually,
                Annual = Annual,
                TaskCategory = CType,
                DateCreated = DateTime.Now,
                UserEmail = User.Identity.Name
            };

            _context.Add(Task);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task added!" });

        }

        [HttpPut]
        public async Task<IActionResult> EditTask(int Id, string Desc, string Schedule)
        {

            var main_Task = await _context.Main_Task.FindAsync(Id);

            main_Task.Description = Desc;
            main_Task.Schedule = Schedule;

            _context.Update(main_Task);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Task Updated!" });

        }

        [HttpGet]
        public async Task<IActionResult> GetTaskAsync(int id)
        {
            var main_Task = await _context.Main_Task.FindAsync(id);
            return Json(new { data = main_Task });

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var main_Task = await _context.Main_Task.FindAsync(id);
            _context.Main_Task.Remove(main_Task);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Task deleted!" });
        }

        #endregion
    }
}
