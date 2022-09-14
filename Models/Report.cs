using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class Report
    {
        public int Id { get; set; }

        public string  Checklist { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateCompleted { get; set; }

        public string Status { get; set; }

        public string AssignedTo { get; set; }

        public string UserName { get; set; }

        public DateTime? TaskCompleted { get; set; }
    }
}
