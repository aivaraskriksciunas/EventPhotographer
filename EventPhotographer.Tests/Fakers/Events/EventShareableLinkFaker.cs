using EventPhotographer.App.Events.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventPhotographer.Tests.Fakers.Events;

internal class EventShareableLinkFaker : Bogus.Faker<EventShareableLink>
{
    public EventShareableLinkFaker()
    {
        RuleFor(e => e.Code, (f, e) => e.Code = f.Random.String2(6, "123456"));
    }
}
