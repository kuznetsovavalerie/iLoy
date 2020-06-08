using ILoyInterview.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstaStore.Data.Configuration
{
    public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
    {
        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.ToTable("ProjectTasks").HasKey(x => x.Id);

            builder.HasOne(p => p.ParentTask).WithMany(p => p.ChildTasks).OnDelete(DeleteBehavior.NoAction);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(255);
        }
    }
}
