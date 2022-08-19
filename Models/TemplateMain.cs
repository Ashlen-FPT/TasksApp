using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class TemplateMain
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Schedule { get; set; }

        public DateTime DateCreated { get; set; }

        public string UserEmail { get; set; }
    }
}
