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
            //var query = _context.PreTasks.Where(x => x.Status != null).Select(t => new PreTasks
            //{
            //    DateCreated = t.DateCreated,
            //    Status = t.Status,
            //}).ToList();

            //ViewData["OperatorEvents"] = query;

            var BobCats = _context.BobCats.ToList();
            //.Select(t => new BobCat
            // {
            //     DateCreated = t.DateCreated,
            //     Status = t.Status,
            // }).ToList();
           var query = new List<BobCat>();
            if (BobCats.Any(x => x.Status == "Do-Checklist"))
            {
                query= BobCats.Where(x => x.Status == "Do-Checklist").Take(1).ToList();
            }
            
                if (BobCats.Any(x => x.Status == "Partially Completed"))
            {
                query = BobCats.Where(x => x.Status == "Partially Completed").Take(1).ToList();
            }

            if (BobCats.Any(x => x.Status == "Completed"))
            {
                query = BobCats.Where(x => x.Status == "Completed").Take(1).ToList();
            }


            ViewData["BobCat"] = query;

            return View();
        }

        public IActionResult SupervisorCalendar()
        {
            //var query = _context.Tasks.Where(x => x.Status != null).Select(t => new Tasks
            //{
            //    DateCreated = t.DateCreated,
            //    Status = t.Status,
            //}).ToList();

            //ViewData["SupervisorEvents"] = query;

            var BobCats = _context.BobCats.Where(x => x.Status != null).Select(t => new BobCat
            {
                DateCreated = t.DateCreated,
                Status = t.Status,
            }).ToList();



            ViewData["BobCat"] = BobCats;

            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

    }
}