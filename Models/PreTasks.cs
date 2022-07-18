using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class PreTasks
    {
        public int Id { get; set; }

        public string Comments { get; set; }

        [Required(ErrorMessage = "Please enter Description")]
        public string Description { get; set; }

        public bool IsDone { get; set; }

        public DateTime? DateTaskCompleted { get; set; }

        public bool TasksCompleted { get; set; }

        public string Status { get; set; }

        public DateTime? DateAllTaskCompleted { get; set; }

        public DateTime DateCreated { get; set; }

        public string User { get; set; }

        public string Schedule { get; set; }

        public string TaskType { get; set; }
    }
}
