using System;
using System.ComponentModel.DataAnnotations;

namespace TasksApp.Models
{
    public class TemplateTask
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Schedule { get; set; }

        public string DayOfWeek { get; set; }

        public string Month { get; set; }

        public string Quarterly { get; set; }

        public string Bi_Annual { get; set; }

        public string Annual { get; set; }

        public DateTime DateCreated { get; set; }

        public string UserEmail { get; set; }

        public string TaskType { get; set; }
    }
}
