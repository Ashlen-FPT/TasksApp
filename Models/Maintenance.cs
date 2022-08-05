using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class Maintenance
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public bool Ok { get; set; }

        public bool Not { get; set; }

        public string Comments { get; set; }

        public string Shift { get; set; }

        public string UserName { get; set; }

        public DateTime Date { get; set; }
    }
}
