using Dota2DraftHelper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dota2DraftHelper.DataBase.Configurations;

public class OwnPickConfiguration : IEntityTypeConfiguration<OwnPick>
{
    public void Configure(EntityTypeBuilder<OwnPick> builder)
    {
        builder.HasOne<Hero>()
               .WithMany()
               .HasForeignKey(c => c.HeroId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Lane>()
               .WithMany()
               .HasForeignKey(c => c.LaneId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
