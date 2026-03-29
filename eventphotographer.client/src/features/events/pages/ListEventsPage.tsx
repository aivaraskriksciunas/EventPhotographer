import { EventResponse } from '@/api/events';
import { Link, useLoaderData } from 'react-router-dom';

export default function ListEventsPage() {
    const events = useLoaderData();

    if (!events) {
        return (
            <div>
                No current events. Please{' '}
                <Link to="/events/new">create one</Link>.
            </div>
        );
    }

    return (
        <div className="">
            {events.map((event: EventResponse) => (
                <div className="card mb-3" key={event.id}>
                    <div className="card-body">
                        <Link to={`/events/${event.id}`}>
                            <h4 className="card-title">{event.name}</h4>
                        </Link>
                    </div>
                </div>
            ))}
        </div>
    );
}
