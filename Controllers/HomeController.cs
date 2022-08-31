using System.Linq;
using TasksApp.Data;
using TasksApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TasksApp.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
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

        public IActionResult TG_Index()
        {
            return View();
        }
    }
}
