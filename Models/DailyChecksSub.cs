using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class DailyChecksSub
    {
        public int Id { get; set; }

        public string Heading { get; set; }

        public string Description { get; set; }

        public virtual TemplateDailyCheck TemplateDailyChecks { get; set; }
    }
}
