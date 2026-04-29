using EventPhotographer.Data.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPhotographer.Data.Entities.Events;

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
