using Microsoft.EntityFrameworkCore;
using Pmp.Api.Models;

namespace Pmp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.ProjectId)
            .IsUnique();

        modelBuilder.Entity<ProjectIdCounter>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();

            entity.HasData(new ProjectIdCounter
            {
                Id = 1,
                LastNumber = 0
            });
        });
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectIdCounter> ProjectsIdCounters => Set<ProjectIdCounter>();
}