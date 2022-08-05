using System;
using System.ComponentModel.DataAnnotations;

namespace TasksApp.Models
{
    public class Tasks
    {

        public int Id { get; set; }

        public string Comments { get; set; }

        [Required(ErrorMessage = "Please enter Description")]
        public string Description { get; set; }

        //public string SubDescription { get; set; }

        public bool IsDone { get; set; }

        //public bool IsNotDone { get; set; }

        public DateTime? DateTaskCompleted { get; set; }

        public bool TasksCompleted { get; set; }

        public string Status { get; set; }

        public DateTime? DateAllTaskCompleted { get; set; }

        public DateTime DateCreated { get; set; }

        public string User { get; set; }

        [Required]
        public string Schedule { get; set; }

        public string TaskType { get; set; }
    }
}
