using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TasksApp.Data;
using TasksApp.ViewModels;
using TasksApp.Models;

namespace TasksApp.Controllers
{
    public class Dashboard : Controller
    {
        private readonly ApplicationDbContext _context;

        public Dashboard(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            DashboardViewModel dashboard = new DashboardViewModel();

            dashboard.templateTasks_count = _context.TemplateTasks.Count();
            dashboard.tasks_count = _context.Tasks.Count();
            dashboard.preTasks_count = _context.PreTasks.Count();

            return View(dashboard);
        }

        public IActionResult TLG_Index()
        {
            DashboardViewModel TLG_dashboard = new DashboardViewModel();

            TLG_dashboard.bobCats_count = _context.BobCats.Count();
            TLG_dashboard.dailyChecks_count = _context.DailyChecks.Count();
            TLG_dashboard.dailyWeighs_count = _context.DailyWeighs.Count();
            TLG_dashboard.items_count = _context.items.Count();
            TLG_dashboard.maintanance_count = _context.Maintenances.Count();

            return View(TLG_dashboard);
        }

        public IActionResult SupervisorCalendar()
        {


            var query = _context.Tasks.Where(x => x.Status != null).Select(t => new Tasks
            {
                DateCreated = t.DateCreated,
                Status = t.Status,
            }).ToList();

            ViewData["SupervisorEvents"] = query;

            return View();
        }

        public IActionResult OperatorCalendar()
        {
            var query = _context.PreTasks.Where(x => x.Status != null).Select(t => new PreTasks
            {
                DateCreated = t.DateCreated,
                Status = t.Status,
            }).ToList();

            ViewData["OperatorEvents"] = query;

            return View();
        }



        public ActionResult Test()
        {
            return View();
        }

        
    }
}