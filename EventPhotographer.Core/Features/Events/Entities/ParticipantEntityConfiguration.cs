using EventPhotographer.Core.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPhotographer.Core.Features.Events.Entities;

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
