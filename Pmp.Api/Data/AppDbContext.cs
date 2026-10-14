using Microsoft.EntityFrameworkCore;
using Pmp.Api.Models;

namespace Pmp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Project> Projects => Set<Project>();
}