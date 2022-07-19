using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TasksApp.Data;
using TasksApp.ViewModels;
using TasksApp.Models;
using System.Text.Json;

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

        //    #region API Calls
        //    public int GetCust()
        //    {
        //        var userList = _context.ApplicationUsers.ToList();
        //        var userRole = _context.UserRoles.ToList();
        //        var roles = _context.Roles.ToList();
        //        foreach (var user in userList)
        //        {
        //            var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
        //            user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

        //        }
        //        var list = userList.Where(l => l.Role == SD.Role_Operator);
        //        return list.Count();
        //    }
        //    public IActionResult GetDashBoxes(int? id)
        //    {
        //        DateTime now = DateTime.Now.Date;

        //        var tasks = _context.TemplateTasks
        //            .Where(b => b.TaskType == "Tasks").Where(c => c.TaskType == "PreTasks").ToList();
        //            //.Include(b => b.Schedule).Where(i => i.Id == id).Where(u => u.DateCreated.Month == DateTime.Now.Month).ToList();

        //        var TodayBookings = tasks.Where(d => d.DateCreated.Date == DateTime.Now.Date);

        //        decimal totalTasksIncomplete = 0;
        //        decimal totalTasksComplete = 0;

        //        decimal rate = 0;

        //        if (tasks.Count() != 0)
        //        {
        //            //totalTasksIncomplete = (TemplateTasks.First().Description.NumOfTasks) * 24;
        //            totalTasksComplete = TodayBookings.Count();

        //            rate = totalTasksComplete / totalTasksIncomplete;


        //            DashFirstRow dashFirstRow = new DashFirstRow
        //            {
        //                NewTask = tasks.Where(d => d.DateCreated.Date > now).Count(),

        //                TasksForToday = tasks.Where(u => u.DateCreated == now).Count(),

        //                Users = GetCust(),

        //                //Rate = Math.Round(rate, 2) + " %"


        //            };

        //            return Json(new { data = dashFirstRow });
        //        }

        //        else
        //        {
        //            DashFirstRow dashFirstRow = new DashFirstRow
        //            {
        //                NewTask = 0,

        //                TasksForToday = 0,

        //                Users = GetCust(),

        //                //Rate = "0 %"

        //            };

        //            return Json(new { data = dashFirstRow });
        //        }
        //    }
        //    public IActionResult GetPastSixMonths(int? id)
        //    {
        //        var applicationDbContext = _context.Tasks
        //            .Include(b => b.TaskType)
        //            .Include(b => b.Schedule)
        //            .Where(u => u.DateCreated > DateTime.Now.AddMonths(-7)).Where(i => i.Id == id).ToList();

        //        var lastSixMonths = Enumerable.Range(0, 6).Select(i => DateTime.Now.AddMonths(i - 6).ToString("MM/yyyy"));

        //        Dictionary<string, decimal> monthTasks = new Dictionary<string, decimal>();

        //        foreach (var monthAndYear in lastSixMonths)
        //        {
        //            var temp = Convert.ToDateTime(monthAndYear);

        //            monthTasks[temp.ToString("MMMM yyyy")] = 0;
        //        }
        //        foreach (var monthAndYear in lastSixMonths)
        //        {
        //            var temp = Convert.ToDateTime(monthAndYear);

        //            foreach (var booking in applicationDbContext)
        //            {
        //                if (temp.Month == booking.DateCreated.Month)
        //                {
        //                    if (monthTasks.ContainsKey(temp.ToString("MMMM yyyy")))
        //                    {
        //                        monthTasks[temp.ToString("MMMM yyyy")] += 1;
        //                    }
        //                    else
        //                    {
        //                        monthTasks[temp.ToString("MMMM yyyy")] = 1;

        //                    }
        //                }
        //            }

        //        }


        //        return Json(new { data = monthTasks });

        //    }
        //    public IActionResult GetLatePastSixMonths(int? id)
        //    {
        //        var applicationDbContext = _context.Tasks
        //            .Include(b => b.TaskType)
        //            .Include(b => b.Schedule)
        //            .Where(u => u.DateCreated > DateTime.Now.AddMonths(-7))
        //            .Where(i => i.Id == id).Where(l => l.IsNotDone == true).ToList();

        //        var lastSixMonths = Enumerable.Range(0, 6).Select(i => DateTime.Now.AddMonths(i - 6).ToString("MM/yyyy"));

        //        Dictionary<string, decimal> monthTasks = new Dictionary<string, decimal>();

        //        foreach (var monthAndYear in lastSixMonths)
        //        {
        //            var temp = Convert.ToDateTime(monthAndYear);

        //            monthTasks[temp.ToString("MMMM yyyy")] = 0;
        //        }
        //        foreach (var monthAndYear in lastSixMonths)
        //        {
        //            var temp = Convert.ToDateTime(monthAndYear);

        //            foreach (var tasks in applicationDbContext)
        //            {
        //                if (temp.Month == tasks.DateCreated.Month)
        //                {
        //                    if (monthTasks.ContainsKey(temp.ToString("MMMM yyyy")))
        //                    {
        //                        monthTasks[temp.ToString("MMMM yyyy")] += 1;
        //                    }
        //                    else
        //                    {
        //                        monthTasks[temp.ToString("MMMM yyyy")] = 1;

        //                    }
        //                }

        //            }

        //        }

        //        return Json(new { data = monthTasks });

        //    }
        //    public IActionResult GetEarlyPastSixMonths(int? id)
        //    {
        //        var applicationDbContext = _context.Tasks
        //            .Include(b => b.TaskType)
        //            .Include(b => b.Schedule)
        //            .Where(u => u.DateCreated > DateTime.Now.AddMonths(-7))
        //            .Where(i => i.Id == id).Where(e => e.IsDone == true).ToList();

        //        var lastSixMonths = Enumerable.Range(0, 6).Select(i => DateTime.Now.AddMonths(i - 6).ToString("MM/yyyy"));

        //        Dictionary<string, decimal> monthTasks = new Dictionary<string, decimal>();

        //        foreach (var monthAndYear in lastSixMonths)
        //        {
        //            var temp = Convert.ToDateTime(monthAndYear);

        //            monthTasks[temp.ToString("MMMM yyyy")] = 0;
        //        }
        //        foreach (var monthAndYear in lastSixMonths)
        //        {
        //            var temp = Convert.ToDateTime(monthAndYear);

        //            foreach (var tasks in applicationDbContext)
        //            {
        //                if (temp.Month == tasks.DateCreated.Month)
        //                {
        //                    if (monthTasks.ContainsKey(temp.ToString("MMMM yyyy")))
        //                    {
        //                        monthTasks[temp.ToString("MMMM yyyy")] += 1;
        //                    }
        //                    else
        //                    {
        //                        monthTasks[temp.ToString("MMMM yyyy")] = 1;

        //                    }
        //                }
        //            }

        //        }


        //        return Json(new { data = monthTasks });

        //    }
        //    public IActionResult GetPastSixMonthsFilters(int? id)
        //    {
        //        var tasks = _context.Tasks
        //            .Include(b => b.TaskType)
        //            .Include(b => b.Schedule)
        //            .Where(u => u.DateCreated > DateTime.Now.AddMonths(-7) && u.DateCreated.Month < DateTime.Now.Month)
        //            .Where(i => i.Id == id).ToList();


        //        if (tasks.Count() == 0)
        //        {
        //            DashFilters dashFilters = new DashFilters
        //            {

        //                Late = "0 %",
        //                Early = "0 %",
        //                Completed = "0 %",
        //                InCompleted = "0 %",
        //                Total = "0"
        //            };

        //            return Json(new { data = dashFilters });
        //        }
        //        else
        //        {
        //            var total = tasks.Count();
        //            var Late = tasks.Where(l => l.IsNotDone == true).Count();
        //            var Early = tasks.Where(e => e.IsDone == true).Count();
        //            var Completed = tasks.Where(a => a.DateCreated.Date == a.DateCreated.Date).Count();
        //            var Incomplete = tasks.Where(a => a.DateCreated.Date != a.DateCreated.Date).Where(s => s.TaskType == " ").Count();


        //            string LatePercent = ((int)Math.Round((double)(100 * Late) / total)).ToString();
        //            string EarlyPercent = ((int)Math.Round((double)(100 * Early) / total)).ToString();
        //            string CompletedPercent = ((int)Math.Round((double)(100 * Completed) / total)).ToString();
        //            string InCompletedPercent = ((int)Math.Round((double)(100 * Incomplete) / total)).ToString();

        //            DashFilters dashFilters = new DashFilters
        //            {

        //                Late = LatePercent + "%",
        //                Early = EarlyPercent + "%",
        //                Completed = CompletedPercent + "%",
        //                InCompleted = InCompletedPercent + "%",
        //                Total = total.ToString()
        //            };

        //            return Json(new { data = dashFilters });
        //        }
        //    }
        //    public IActionResult GetLastMonthsFilters(int? id)
        //    {
        //       var tasks = _context.Tasks
        //             .Include(b => b.TaskType)
        //             .Include(b => b.Schedule)
        //             .Where(u => u.DateCreated.Month == DateTime.Now.AddMonths(-1).Month)
        //             .Where(i => i.Id == id).ToList();

        //        var total = tasks.Count();
        //        var Late = tasks.Where(l => l.IsNotDone == true).Count();
        //        var Early = tasks.Where(e => e.IsDone == true).Count();
        //        var Completed = tasks.Where(a => a.DateCreated.Date == a.DateCreated.Date).Count();
        //        var NotCompleted = tasks.Where(a => a.DateCreated.Date != a.DateCreated.Date).Where(s => s.TaskType == " ").Count();

        //        DashFilters dashFilters = new DashFilters
        //        {

        //            Late = Late.ToString(),
        //            Early = Early.ToString(),
        //            Completed = Completed.ToString(),
        //            InCompleted = NotCompleted.ToString()


        //        };

        //        return Json(new { data = dashFilters });

        //    }

        //    public IActionResult GetTaskToday(int? id)
        //    {
        //        DateTime now = DateTime.Now.Date;

        //        var applicationDbContext = _context.Tasks
        //            .Include(b => b.TaskType)
        //            .Include(b => b.Schedule)
        //            .Where(u => u.DateCreated == now).Where(s => s.TaskType != " ").Where(i => i.Id == id).ToList();

        //        return Json(new { data = applicationDbContext });

        //    }
        //    #endregion
        //}

        //class DashFilters
        //{
        //    public string Total { get; set; }

        //    public string Late { get; set; }

        //    public string Early { get; set; }

        //    public string Completed { get; set; }

        //    public string InCompleted { get; set; }

        //}

        //class DashFirstRow
        //{

        //    public int NewTask { get; set; }

        //    //public string Rate { get; set; }

        //    public int Users { get; set; }

        //    public int TasksForToday { get; set; }


        //}
    }
}
