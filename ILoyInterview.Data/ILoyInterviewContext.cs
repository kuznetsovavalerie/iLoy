using ILoyInterview.Data.Entities;
using InstaStore.Data.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ILoyInterview.Data
{
    public class ILoyInterviewContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }

        public ILoyInterviewContext(DbContextOptions<ILoyInterviewContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectTaskConfiguration());
        }
    }
}
