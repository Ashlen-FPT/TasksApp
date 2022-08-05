using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class Items
    {
        public int Id { get; set; }

        public int TaskId { get; set; }

        public string SubItem { get; set; }

        public string Checks { get; set; }

        public string Machine { get; set; }

        public string UserName { get; set; }

        public int PlantNumber { get; set; }

        public DateTime Hours { get; set; }

        public DateTime Date { get; set; }

        public bool InOrder { get; set; }

        public bool OutOfOrder { get; set; }

        public string Schedule { get; set; }

        public bool IsDone { get; set; }

    }
}
