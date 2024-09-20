using Dota2DraftHelper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dota2DraftHelper.DataBase.Configurations;

class LaneConfiguration : IEntityTypeConfiguration<Lane>
{
    public void Configure(EntityTypeBuilder<Lane> builder)
    {
        builder.HasData(new Lane() { Id = 1, Name = "Safe Lane", AlternativeName = "Pos 1" },
                        new Lane() { Id = 2, Name = "Mid Lane", AlternativeName = "Pos 2" },
                        new Lane() { Id = 3, Name = "Hard Lane", AlternativeName = "Pos 3" },
                        new Lane() { Id = 4, Name = "Support", AlternativeName = "Pos 4" },
                        new Lane() { Id = 5, Name = "Hard Support", AlternativeName = "Pos 5" });
    }
}
