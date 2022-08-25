using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class TemplateItems
    {
        public int Id { get; set; }

        [Required]
        public string Main { get; set; }

        public string Description { get; set; }

        [Required]
        public string Schedule { get; set; }

        public DateTime DateCreated { get; set; }

        public string UserEmail { get; set; }
    }
}
