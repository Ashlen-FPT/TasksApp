using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class TemplateBobCat
    {
        public int Id { get; set; }

        public int TaskNo { get; set; }

        public string Heading { get; set; }

        public string General { get; set; }

        public string Brake_Test { get; set; }

        public string Test_Brake { get; set; }

        public string Results { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
