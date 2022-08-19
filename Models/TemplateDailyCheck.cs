using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class TemplateDailyCheck
    {
        public int Id { get; set; }

        public int HeadNo { get; set; }

        public string Heading { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public bool SubItems { get; set; }

    }
}
