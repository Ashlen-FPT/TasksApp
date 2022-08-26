using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class DailyCheck
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public string ReportHeading { get; set; }

        public string ReportDesc { get; set; }

        public bool IsDone { get; set; }

        public string Remarks { get; set; }

        public DateTime DateCompleted { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
