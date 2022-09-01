using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApp.Models
{
    public class Logs
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string LogType{ get; set; }

        public DateTime DateTime { get; set; }

        public string UpdatedTable { get; set; }

        public string OldData { get; set; }

        public string NewData { get; set; }

        public string Entity { get; set; }
    }
}
