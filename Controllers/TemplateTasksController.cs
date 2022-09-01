using System;
using System.Linq;
using TasksApp.Data;
using TasksApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TasksApp.Services;
using TasksApp.Enums;

namespace TasksApp.Controllers
{
    [Authorize]
    public class TemplateTasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        //private readonly UserManager<IdentityUser> _userManager;

        public TemplateTasksController(ApplicationDbContext context , UserService userService)
        {
            _context = context;
            _userService = userService;
            //_userManager = userManager;
        }

        // GET: TemplateTasks
        public async Task<IActionResult> Index()
        {
            return View(await _context.TemplateTasks.ToListAsync());
        }

        // GET: TemplateTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateTask = await _context.TemplateTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateTask == null)
            {
                return NotFound();
            }

            return View(templateTask);
        }

        // GET: TemplateTasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TemplateTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Schedule,DateCreated,UserEmail")] TemplateTask templateTask)
        {
            if (ModelState.IsValid)
            {
               
                _context.Add(templateTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(templateTask);
        }

        // GET: TemplateTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateTask = await _context.TemplateTasks.FindAsync(id);
            if (templateTask == null)
            {
                return NotFound();
            }
            return View(templateTask);
        }

        // POST: TemplateTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Schedule,DateCreated,UserEmail")] TemplateTask templateTask)
        {
            if (id != templateTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(templateTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TemplateTaskExists(templateTask.Id))
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
            return View(templateTask);
        }

        // GET: TemplateTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var templateTask = await _context.TemplateTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (templateTask == null)
            {
                return NotFound();
            }

            return View(templateTask);
        }

        // POST: TemplateTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var templateTask = await _context.TemplateTasks.FindAsync(id);
            _context.TemplateTasks.Remove(templateTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TemplateTaskExists(int id)
        {
            return _context.TemplateTasks.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetDaily()
        {
            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Read,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateTasks",
                OldData= "Read TemplateTasks",
                NewData=null
            };

            _context.Logs.Add(log);
            _context.SaveChanges();


            return Json(new { data = _context.TemplateTasks.Where(s => s.Schedule == "Daily").ToList()});

        }

        [HttpGet]
        public IActionResult GetWeekly()
        {

            return Json(new { data = _context.TemplateTasks.Where(s => s.Schedule == "Weekly").ToList() });

        }

        [HttpGet]
        public IActionResult GetMonthly()
        {

            return Json(new { data = _context.TemplateTasks.Where(s => s.Schedule == "Monthly").ToList() });

        }

        [HttpGet]
        public IActionResult GetPreTasks()
        {
            return Json(new { data = _context.TemplateTasks.Where(s => s.Schedule == "Daily").Where(t => t.TaskType == "PreTasks").ToList() });

        }

        [HttpGet]
        public IActionResult GetQuarterly()
        {
            return Json(new { data = _context.TemplateTasks.Where(s => s.Schedule == "Quarterly").ToList() });

        }

        [HttpGet]
        public IActionResult GetBi()
        {
            return Json(new { data = _context.TemplateTasks.Where(s => s.Schedule == "Bi-Annually").ToList() });

        }

        [HttpGet]
        public IActionResult GetAnually()
        {
            return Json(new { data = _context.TemplateTasks.Where(s => s.Schedule == "Annually").ToList() });

        }

        [HttpPost]
        public async Task<IActionResult> AddTask(string Desc, string Schedule, string Day, string Month, string TType, string Annual, string Quarter, string Bi_Annually, string ChekList)
        {

            var Task = new TemplateTask
            {
                Description = Desc,
                Schedule = Schedule,
                DayOfWeek = Day,
                Month = Month,
                Quarterly = Quarter,
                Bi_Annual = Bi_Annually,
                Annual = Annual,
                TaskType = TType,
                DateCreated = DateTime.Now,
                UserEmail = User.Identity.Name,
                ChekList = ChekList
            
            };

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Created,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateTasks",
                OldData = "New Task",
                NewData = $"{ "Description: " + Task.Description + "Schedule: " + Task.Schedule +"Day Of Week:"+Task.DayOfWeek+" Month :"+Task.Month+"Quarterly :"+Task.Quarterly+"Bi Annual :"+Task.Bi_Annual+"Annual :"+Task.Annual+"Task Type :"+Task.TaskType+ "Date Created: " + Task.DateCreated + "UserEmail: " + Task.UserEmail +"Checklist :"+Task.ChekList}"
            };

            _context.Add(Task);
            _context.Logs.Add(log);
            await _context.SaveChangesAsync(_userService.GetUser());

            return Json(new { success = true, message = "Task added!" });

        }

        [HttpPut]
        public async Task<IActionResult> EditTask(int Id, string Desc, string Schedule)
        {
            var existingDescription = _context.TemplateTasks.Find(Id).Description;
            var existingSchedule = _context.TemplateTasks.Find(Id).Schedule;

            var templateTask = await _context.TemplateTasks.FindAsync(Id);

            templateTask.Description = Desc;
            templateTask.Schedule = Schedule;


            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Created,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateTasks",
                OldData = $"{ "Description: " + existingDescription + "Schedule: " + existingSchedule}",
                NewData = $"{ "Description: " + templateTask.Description + "Schedule: " + templateTask.Schedule}"
            };

            _context.Update(templateTask);
            _context.Logs.Add(log);
            
            await _context.SaveChangesAsync(_userService.GetUser());

            return Json(new { success = true, message = "Task Updated!" });

        }

        [HttpGet]
        public async Task<IActionResult> GetTaskAsync(int id)
        {
            var templateTask = await _context.TemplateTasks.FindAsync(id);
            return Json(new { data = templateTask });

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var templateTask = await _context.TemplateTasks.FindAsync(id);

            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Created,
                DateTime = DateTime.Now,
                UpdatedTable = "TemplateTasks",
                OldData = $"{ "Description: " + templateTask.Description + "Schedule: " + templateTask.Schedule}",
                NewData = "Task Removed"
            };

            _context.TemplateTasks.Remove(templateTask);
            _context.Logs.Add(log);
            await _context.SaveChangesAsync(_userService.GetUser());
            return Json(new { success = true, message = "Task deleted!" });
        }

        #endregion
    }
}
