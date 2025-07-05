using Microsoft.EntityFrameworkCore;
using PartsTracker.WebApi.Models;

namespace PartsTracker.Server.Data;

public class InventoryDbContext : DbContext
{
    public DbSet<Part> Parts { get; set; }

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }
}
