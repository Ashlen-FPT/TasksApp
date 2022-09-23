using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TasksApp.Data;
using TasksApp.ViewModels;
using TasksApp.Models;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TasksApp.Enums;

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

        public IActionResult TLG_Index(DateTime date)
        {
            DateTime oDate = Convert.ToDateTime(date);
            DashboardViewModel TLG_dashboard = new DashboardViewModel();

            TLG_dashboard.bobCats_count = _context.BobCats.Where(d => d.DateCreated.Date == DateTime.Now.Date).Count();
            TLG_dashboard.dailyChecks_count = _context.DailyChecks.Where(d => d.DateCreated.Date == DateTime.Now.Date).Count();
            TLG_dashboard.dailyWeighs_count = _context.DailyWeighs.Where(d => d.DateCreated.Date == DateTime.Now.Date).Count();
            TLG_dashboard.items_count = _context.items.Where(d => d.DateCreated.Date == DateTime.Now.Date).Count();
            TLG_dashboard.maintanance_count = _context.Maintenances.Where(d => d.DateCreated.Date == DateTime.Now.Date).Count();

            var dailyChecks = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).ToList();
            //TLG_dashboard.dailyChecks_progress = dailyCheck;
            return View(TLG_dashboard);
        }

        public IActionResult MCT_IT_Index(DateTime date)
        {
            DateTime oDate = Convert.ToDateTime(date);
            DashboardViewModel MCT_IT_dashboard = new DashboardViewModel();

            MCT_IT_dashboard.active_count = _context.Active_D.Where(d => d.DateCreated.Date == DateTime.Now.Date).Count();
            MCT_IT_dashboard.hardware_count = _context.Hardware.Where(d => d.DateCreated.Date == DateTime.Now.Date).Count();
            MCT_IT_dashboard.networking_count = _context.Networking.Where(d => d.DateCreated.Date == DateTime.Now.Date).Count();
            MCT_IT_dashboard.security_count = _context.Security.Where(d => d.DateCreated.Date == DateTime.Now.Date).Count();
            MCT_IT_dashboard.software_count = _context.Software.Where(d => d.DateCreated.Date == DateTime.Now.Date).Count();

            var dailyChecks = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date).ToList();
            //TLG_dashboard.dailyChecks_progress = dailyCheck;
            return View(MCT_IT_dashboard);
        }

        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date) });

        }

        public IActionResult Calendar()
        {
            //BobCats
            var BobCats = _context.BobCats.ToList();
            var queryBobCats = new List<BobCat>();

            queryBobCats = BobCats.Where(x => x.Status.StartsWith("D") || x.Status.StartsWith("P") || x.Status.StartsWith("C")).ToList();



            ViewData["BobCat"] = queryBobCats;

            //DailyChecks

            var DailyCheck = _context.DailyChecks.ToList();
            var queryDailyChecks = new List<DailyCheck>();

            queryDailyChecks = DailyCheck.Where(x => x.Status.StartsWith("D") || x.Status.StartsWith("P") || x.Status.StartsWith("C")).ToList();

            ViewData["DailyChecks"] = queryDailyChecks;

            //DailyWeighs

            //var DailyWeighs = _context.DailyWeighs.ToList();
            //var queryDailyWeighs = new List<DailyWeigh>();

            //queryDailyWeighs = DailyWeighs.Where(x => x.Status.StartsWith("D") || x.Status.StartsWith("P") || x.Status.StartsWith("C")).ToList();

            //ViewData["DailyWeighs"] = queryDailyWeighs;


            //Items

            var Items = _context.items.ToList();
            var queryItems = new List<Items>();

            queryItems = Items.Where(x => x.Status.StartsWith("D") || x.Status.StartsWith("P") || x.Status.StartsWith("C")).ToList();

            ViewData["Items"] = queryItems;

            if (User.IsInRole(SD.Role_Supervisor.ToString())||User.IsInRole(SD.Role_Admin.ToString()))
            {
                //Maintenances

                var Maintenances = _context.Maintenances.ToList();
                var queryMaintenances = new List<Maintenance>();

                queryMaintenances = Maintenances.Where(x => x.Status.StartsWith("D") || x.Status.StartsWith("P") || x.Status.StartsWith("C")).ToList();

                ViewData["Maintenances"] = queryMaintenances;
            }




            var log = new Logs
            {
                UserName = User.FindFirst("Username")?.Value,
                UserEmail = User.Identity.Name,
                Entity = User.FindFirst("Organization")?.Value,
                LogType = LogTypes.Read,
                DateTime = DateTime.Now,
                UpdatedTable = null,
                OldData = "User Read Calendar",
                NewData = null
            };

            _context.Logs.Add(log);
            _context.SaveChanges();

            return View();
        }

        public IActionResult ITCalendar()
        {
            //Active_D
            var Active_D = _context.Active_D.ToList();
            var queryActive_D = new List<Active_D>();

            queryActive_D = Active_D.Where(x => x.Status.StartsWith("D") || x.Status.StartsWith("P") || x.Status.StartsWith("C")).ToList();



            ViewData["ActiveD"] = queryActive_D;

            return View();
        }

    }
}