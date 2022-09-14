using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TasksApp.Data;
using TasksApp.ViewModels;
using TasksApp.Models;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        [HttpGet]
        public IActionResult GetAll(DateTime date)
        {

            DateTime oDate = Convert.ToDateTime(date);

            return Json(new { data = _context.DailyChecks.Where(d => d.DateCreated.Date == oDate.Date) });

        }

        public IActionResult OperatorCalendar()
        {
            //BobCats
            var BobCats = _context.BobCats.ToList();
            var queryBobCats = new List<BobCat>();
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            if (BobCats.Any(x => x.Status.StartsWith("D")))
            {
                queryBobCats = BobCats.Where(x => x.DateCreated == today).Where(x => x.Status.StartsWith("D")).Take(1).ToList();            
            }
            else if (BobCats.Any(x => x.Status.StartsWith("P")))
            {
                queryBobCats = BobCats.Where(x => x.DateCreated == today).Where(x => x.Status.StartsWith("P")).Take(1).ToList();
            }
            else if(BobCats.Any(x => x.Status.StartsWith("C")))
            {
                queryBobCats = BobCats.Where(x => x.DateCreated == today).Where(x => x.Status.StartsWith("C")).Take(1).ToList();
            }
            ViewData["BobCat"] = queryBobCats;

            //DailyChecks

            var DailyCheck = _context.DailyChecks.ToList();
            var queryDailyChecks = new List<DailyCheck>();
            if (DailyCheck.Any(x => x.Status.StartsWith("D")))
            {
                queryDailyChecks = DailyCheck.Where(x => x.Status.StartsWith("D")).Take(1).ToList();
            }

            if (DailyCheck.Any(x => x.Status.StartsWith("P")))
            {
                queryDailyChecks = DailyCheck.Where(x => x.Status.StartsWith("P")).Take(1).ToList();
            }

            if (DailyCheck.Any(x => x.Status.StartsWith("C")))
            {
                queryDailyChecks = DailyCheck.Where(x => x.Status.StartsWith("C")).Take(1).ToList();
            }

            ViewData["DailyCheck"] = queryDailyChecks;

            //DailyWeighs

            var DailyWeighs = _context.DailyWeighs.ToList();
            var queryDailyWeighs = new List<DailyWeigh>();
            if (DailyWeighs.Any(x => x.Status.StartsWith("D")))
            {
                queryDailyWeighs = DailyWeighs.Where(x => x.Status.StartsWith("D")).Take(1).ToList();
            }

            if (DailyWeighs.Any(x => x.Status.StartsWith("P")))
            {
                queryDailyWeighs = DailyWeighs.Where(x => x.Status.StartsWith("P")).Take(1).ToList();
            }

            if (DailyWeighs.Any(x => x.Status.StartsWith("C")))
            {
                queryDailyWeighs = DailyWeighs.Where(x => x.Status.StartsWith("C")).Take(1).ToList();
            }

            ViewData["DailyWeighs"] = queryDailyWeighs;

            //Items - TODO



            return View();
        }

        public IActionResult SupervisorCalendar()
        {
            //BobCats
            var BobCats = _context.BobCats.ToList();
            var queryBobCats = new List<BobCat>();
            if (BobCats.Any(x => x.Status.StartsWith("D")))
            {
                queryBobCats = BobCats.Where(x => x.Status.StartsWith("D")).Take(1).ToList();
            }

            if (BobCats.Any(x => x.Status.StartsWith("P")))
            {
                queryBobCats = BobCats.Where(x => x.Status.StartsWith("P")).Take(1).ToList();
            }

            if (BobCats.Any(x => x.Status.StartsWith("C")))
            {
                queryBobCats = BobCats.Where(x => x.Status.StartsWith("C")).Take(1).ToList();
            }

            ViewData["BobCat"] = queryBobCats;

            //DailyChecks

            var DailyCheck = _context.DailyChecks.ToList();
            var queryDailyChecks = new List<DailyCheck>();
            if (DailyCheck.Any(x => x.Status.StartsWith("D")))
            {
                queryDailyChecks = DailyCheck.Where(x => x.Status.StartsWith("D")).Take(1).ToList();
            }

            if (DailyCheck.Any(x => x.Status.StartsWith("P")))
            {
                queryDailyChecks = DailyCheck.Where(x => x.Status.StartsWith("P")).Take(1).ToList();
            }

            if (DailyCheck.Any(x => x.Status.StartsWith("C")))
            {
                queryDailyChecks = DailyCheck.Where(x => x.Status.StartsWith("C")).Take(1).ToList();
            }

            ViewData["DailyCheck"] = queryDailyChecks;

            //DailyWeighs

            var DailyWeighs = _context.DailyWeighs.ToList();
            var queryDailyWeighs = new List<DailyWeigh>();
            if (DailyWeighs.Any(x => x.Status.StartsWith("D")))
            {
                queryDailyWeighs = DailyWeighs.Where(x => x.Status.StartsWith("D")).Take(1).ToList();
            }

            if (DailyWeighs.Any(x => x.Status.StartsWith("P")))
            {
                queryDailyWeighs = DailyWeighs.Where(x => x.Status.StartsWith("P")).Take(1).ToList();
            }

            if (DailyWeighs.Any(x => x.Status.StartsWith("C")))
            {
                queryDailyWeighs = DailyWeighs.Where(x => x.Status.StartsWith("C")).Take(1).ToList();
            }

            ViewData["DailyWeighs"] = queryDailyWeighs;



            //Maintenances

            var Maintenances = _context.Maintenances.ToList();
            var queryMaintenances = new List<Maintenance>();
            if (Maintenances.Any(x => x.Status.StartsWith("D")))
            {
                queryMaintenances = Maintenances.Where(x => x.Status.StartsWith("D")).Take(1).ToList();
            }

            if (Maintenances.Any(x => x.Status.StartsWith("P")))
            {
                queryMaintenances = Maintenances.Where(x => x.Status.StartsWith("P")).Take(1).ToList();
            }

            if (Maintenances.Any(x => x.Status.StartsWith("C")))
            {
                queryMaintenances = Maintenances.Where(x => x.Status.StartsWith("C")).Take(1).ToList();
            }

            ViewData["Maintenances"] = queryMaintenances;


            return View();


        }

        public ActionResult Test()
        {
            return View();
        }

    }
}