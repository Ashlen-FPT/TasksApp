using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class DailyChecksSub
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Heading ")]
        public int HeadingId { get; set; }

        [ForeignKey("HeadingId")]
        public TemplateDailyCheck TemplateDailyCheck { get; set; }

        [Required]
        [Display(Name = "Main")]
        public int MainId { get; set; }

        [ForeignKey("MainId")]
        public TemplateItems TemplateItems { get; set; }

        public string Description { get; set; }

        public string Heading { get; set; }

        public DateTime DateCreated { get; set; }

        public string UserEmail { get; set; }

        public string Main { get; set; }

        public string Schedule { get; set; }

        //public virtual TemplateDailyCheck TemplateDailyChecks { get; set; }
    }
}
