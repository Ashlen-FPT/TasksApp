using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TasksApp.Models;

namespace TasksApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<TemplateTask> TemplateTasks { get; set; }

        public DbSet<Tasks> Tasks { get; set; }
    }
}
