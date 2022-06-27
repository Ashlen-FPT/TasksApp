using Microsoft.EntityFrameworkCore;
using TasksApp.Models;

namespace TasksApp.Data
{
    public class ApplicationDbContext : AuditableIdentityContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<TemplateTask> TemplateTasks { get; set; }

        public DbSet<Tasks> Tasks { get; set; }

        public DbSet<PreTasks> PreTasks { get; set; }

        public DbSet<Main_Task> Main_Task { get; set; }

        public DbSet<Active_D> Active_D { get; set; }

        public DbSet<Hardware> Hardware { get; set; }

        public DbSet<Networking> Networking { get; set; }

        public DbSet<Security> Security { get; set; }

        public DbSet<Software> Software { get; set; }

        public DbSet<Audit> Audits { get; set; }

    }
}
