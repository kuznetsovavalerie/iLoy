using ILoyInterview.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstaStore.Data.Configuration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects").HasKey(x => x.Id);

            builder.HasOne(p => p.ParentProject).WithMany(p => p.ChildProjects).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(p => p.Tasks).WithOne(t => t.Project).OnDelete(DeleteBehavior.Cascade);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Code).IsRequired().HasMaxLength(4);
            builder.Property(p => p.State).HasComputedColumnSql("dbo.GetProjectState(Id)");
        }
    }
}
