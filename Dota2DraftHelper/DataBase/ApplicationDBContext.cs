using Dota2DraftHelper.Models;
using Microsoft.EntityFrameworkCore;

namespace Dota2DraftHelper.DataBase;

public class ApplicationDBContext:DbContext
{
    public DbSet<Lane> Lanes { get; set; } = null!; // Table "Lanes"

    public ApplicationDBContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Dota2DraftHelper.db");
    }
}
