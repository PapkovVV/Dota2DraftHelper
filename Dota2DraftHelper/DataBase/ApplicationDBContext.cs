using Dota2DraftHelper.DataBase.Configurations;
using Dota2DraftHelper.Models;
using Microsoft.EntityFrameworkCore;

namespace Dota2DraftHelper.DataBase;

public class ApplicationDBContext:DbContext
{
    public DbSet<Lane> Lanes { get; set; } = null!; // Table "Lanes"
    public DbSet<Hero> Heroes { get; set; } = null!; // Table "Heroes"
    public DbSet<CounterPickInfo> CounterPickInfos { get; set; } = null!; // Table "CounterPickInfos"
    public DbSet<OwnPick> OwnPicks { get; set; } = null!; // Table "OwnPicks"

    public ApplicationDBContext()
    {
        //Database.EnsureDeleted();
        Database.EnsureCreatedAsync();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Dota2DraftHelper.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CounterPickInfoConfiguration());
        modelBuilder.ApplyConfiguration(new OwnPickConfiguration());
        modelBuilder.ApplyConfiguration(new LaneConfiguration());
    }
}
