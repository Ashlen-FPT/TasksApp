using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.ViewModels
{
    public class DashboardViewModel
    {
        public int templateTasks_count { get; set; }

        public int tasks_count { get; set; }

        public int bobCats_count { get; set; }

        public int dailyChecks_count { get; set; }

        public int dailyWeighs_count { get; set; }

        public int items_count { get; set; }

        public int preTasks_count { get; set; }

        public int maintanance_count { get; set; }

        public int dailyChecks_progress { get; set; }

        public int active_count { get; set; }

        public int hardware_count { get; set; }

        public int networking_count { get; set; }

        public int security_count { get; set; }

        public int software_count { get; set; }
    }
}
