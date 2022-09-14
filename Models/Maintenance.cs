using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class Maintenance
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Description")]
        public string Description { get; set; }

        public string Schedule { get; set; }

        public bool Ok { get; set; }

        public bool Not { get; set; }

        public string Comments { get; set; }

        public string Shift { get; set; }

        public bool TasksCompleted { get; set; }

        public string Status { get; set; }

        public DateTime DateAllCompleted { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateCompleted { get; set; }

        public string User { get; set; }
    }
}
