using Microsoft.EntityFrameworkCore;
using TasksApp.Models;
using TasksApp.Services;

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

        public DbSet<BobCat> BobCats { get; set; }

        public DbSet<TemplateBobCat> TemplateBobCat { get; set; }

        public DbSet<DailyWeigh> DailyWeighs { get; set; }

        public DbSet<DailyCheck> DailyChecks { get; set; }

        public DbSet<Maintenance> Maintenances { get; set; }

        public DbSet<Items> items { get; set; }

        public DbSet<BE> BEs { get; set; }

        public DbSet<TemplateDailyCheck> TemplateDailyChecks { get; set; }

        public DbSet<TemplateMain> TemplateMains { get; set; }

        public DbSet<DailyChecksSub> DailyChecksSubs { get; set; }

        public DbSet<TemplateItems> TemplateItem { get; set; }
        public DbSet<Logs> Logs { get; set; }
    }
}
