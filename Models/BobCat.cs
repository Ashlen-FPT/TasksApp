using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class BobCat
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Main { get; set; }

        public int Number { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateTaskCompleted { get; set; }

        public string UserName1 { get; set; }

        public string UserName2 { get; set; }

        public bool Sign1 { get; set; }

        public bool Sign2 { get; set; }

        public bool Yes { get; set; }

        public bool No { get; set; }

        public bool NA { get; set; }
    }
}
