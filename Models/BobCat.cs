using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TasksApp.Models
{
    public class BobCat
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Main { get; set; }

        public int Number { get; set; }

        [Display(Name = "Date")]
        public DateTime DateCreated { get; set; }

        public DateTime? DateTaskCompleted { get; set; }

        [Display(Name="Operator")]
        public string UserName1 { get; set; }

        [Display(Name = "Supervisor")]
        public string UserName2 { get; set; }

        [Display(Name = "Operator Sign off")]
        public bool Sign1 { get; set; }

        [Display(Name = "Supervisor Sign off")]
        public bool Sign2 { get; set; }

        public bool Yes { get; set; }

        public bool No { get; set; }

        public bool NA { get; set; }
    }
}
