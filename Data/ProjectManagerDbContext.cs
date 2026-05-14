using System.Data.Entity;
using ProjectManager.Models;

namespace ProjectManager.Data
{
    public class ProjectManagerDbContext : DbContext
    {
        static ProjectManagerDbContext()
        {
            Database.SetInitializer<ProjectManagerDbContext>(null);
        }

        public ProjectManagerDbContext() : base("ProjectManagerConnection")
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithRequired(t => t.Project)
                .HasForeignKey(t => t.ProjectID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Project>()
                .HasOptional(p => p.ResponsibleEmployee)
                .WithMany()
                .HasForeignKey(p => p.ResponsibleEmployeeID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Tasks)
                .WithOptional(t => t.Employee)
                .HasForeignKey(t => t.EmployeeID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Task>()
                .HasMany(t => t.Comments)
                .WithRequired(c => c.Task)
                .HasForeignKey(c => c.TaskID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<User>()
                .HasOptional(u => u.Employee)
                .WithMany()
                .HasForeignKey(u => u.EmployeeID)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
