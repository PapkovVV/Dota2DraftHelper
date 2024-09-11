using Dota2DraftHelper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dota2DraftHelper.DataBase.Configurations;

public class CounterPickInfoConfiguration : IEntityTypeConfiguration<CounterPickInfo>
{
    public void Configure(EntityTypeBuilder<CounterPickInfo> builder)
    {
        builder.HasOne<Hero>()
               .WithMany()
               .HasForeignKey(c => c.PickId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Hero>()
               .WithMany()
               .HasForeignKey(c => c.CounterPickId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
