using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TasksApp.Models;
using TasksApp.Services;
using TasksApp.ViewModels;

namespace TasksApp.Data
{
    public class AuditableIdentityContext : IdentityDbContext
    {
        public AuditableIdentityContext()
        {
        }
       

        public AuditableIdentityContext(DbContextOptions options ) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder builder)
        //{
        //    if (!builder.IsConfigured)
        //    {
        //        string conn = "Server=(LocalDb)\\MSSQLLocalDB;Database=aspnet-TasksApp;Trusted_Connection=True;MultipleActiveResultSets=true";
        //        builder.UseSqlServer(conn);
        //    }

        //    base.OnConfiguring(builder);
        //}

        public DbSet<Audit> AuditLogs { get; set; }
        //public DbSet<Sample> samples { get; set; }
        public virtual async Task<int> SaveChangesAsync(string userEmail = null)
        {
            OnBeforeSaveChanges(userEmail);
            var result = await base.SaveChangesAsync();
            return result;
        }

        private void OnBeforeSaveChanges(string userEmail)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserEmail = userEmail;
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = Enums.AuditType.AddTask;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = Enums.AuditType.DeleteTask;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = Enums.AuditType.EditTask;
                                auditEntry.AuditType = Enums.AuditType.CompleteTask;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }
            //foreach (var auditEntry in auditEntries)
            //{
            //    AuditLogs.Add(auditEntry.ToAudit());
            //}
        }
    }
}
