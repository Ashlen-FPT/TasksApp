using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class DailyWeigh
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime Time { get; set; }

        public string Supervisor { get; set; }

        public int Gross { get; set; }

        public int Tare { get; set; }

        public int Net { get; set; }

        public string Observation { get; set; }

        public string Description { get; set; }

        public string TaskType { get; set; }

        public string Schedule { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDone { get; set; }

        public DateTime DateCompleted { get; set; }

        public string Comments { get; set; }

        public string ChekList { get; set; }

        public string Status { get; set; }

        public DateTime? DateAllTaskCompleted { get; set; }
    }
}
