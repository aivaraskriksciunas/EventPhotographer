using EventPhotographer.App.Events.Entities;
using Bogus;

namespace EventPhotographer.Tests.Fakes.Events;

internal class EventFaker : Bogus.Faker<Event>
{
    public EventFaker()
    {
        RuleFor(e => e.Id, f => f.Random.Guid());
        RuleFor(e => e.Name, f => f.Lorem.Sentence(3));
        RuleFor(e => e.StartDate, f => f.Date.Future());
        RuleFor(e => e.EndDate, (f, e) => e.StartDate.AddHours(f.Random.Int(1, 5)));
        RuleFor(e => e.CreatedAt, f => f.Date.Recent(5));
    }
}
