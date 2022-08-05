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

        public string username { get; set; }

        public int Gross { get; set; }

        public int Tare { get; set; }

        public int Net { get; set; }

        public string Observation { get; set; }
    }
}
