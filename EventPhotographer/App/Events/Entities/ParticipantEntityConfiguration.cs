using EventPhotographer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPhotographer.App.Events.Entities;

internal class ParticipantEntityConfiguration : UUIDEntityConfiguration<Participant>
{
    public override void Configure(EntityTypeBuilder<Participant> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Token)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("uuidv4()");
    }
}
