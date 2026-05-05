import { EventResponse } from '@/api/events';
import { formatLongDateTime } from '@/utils/date';
import { Calendar } from 'lucide-react';
import { Link, useLoaderData } from 'react-router-dom';
import { DateTime } from 'luxon';
import { useTranslation } from 'react-i18next';

export default function ListEventsPage() {
    const events = useLoaderData();

    if (!events) {
        return (
            <div>
                No current events. Please
                <Link to="/events/new">create one</Link>.
            </div>
        );
    }

    return (
        <div className="">
            {events.map((event: EventResponse) => (
                <div className="card mb-3" key={event.id}>
                    <div className="card-body">
                        <div className='d-flex align-items-center'>
                            <Link to={`/events/${event.id}`}>
                                <h4 className="card-title me-2">{event.name}</h4>
                            </Link>
                            <EventStatusBadge event={event} />
                        </div>
                        <div className='card-meta'>
                            <div className='d-flex align-items-center'>
                                <Calendar className='me-1' />
                                {formatLongDateTime(event.startDate)} - {formatLongDateTime(event.endDate)}
                            </div>
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
}

function EventStatusBadge({ event }: { event: EventResponse }) {
    const { t } = useTranslation();
    const now = DateTime.now();
    const start = DateTime.fromISO(event.startDate);
    const end = DateTime.fromISO(event.endDate);

    if (now < start) {
        return <span className="badge bg-secondary">{start.toRelative()}</span>;
    } else if (now <= end) {
        return <span className="badge bg-success">{t('Active')}</span>;
    }

    return <span className="badge bg-secondary">{t('Past')}</span>;
}