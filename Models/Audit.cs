﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class Audit
    {
        public int Id { get; set; }
        public int KeyFieldId { get; set; }
        public string UserEmail { get; set; }
        public string Type { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string AffectedColumns { get; set; }
        public string PrimaryKey { get; set; }
        public string Changes { get; set; }
    }
}
