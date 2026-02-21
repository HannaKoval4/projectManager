using System.Data.Entity;
using ProjectManager.Models;

namespace ProjectManager.Data
{
    public class ProjectManagerDbContext : DbContext
    {
        public ProjectManagerDbContext() : base("ProjectManagerConnection")
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithRequired(t => t.Project)
                .HasForeignKey(t => t.ProjectID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Tasks)
                .WithOptional(t => t.Employee)
                .HasForeignKey(t => t.EmployeeID)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}






