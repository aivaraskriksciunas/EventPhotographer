import { EventMediaResponse, EventResponse } from '@/api/events';
import { FolderArchive, User } from 'lucide-react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useRouteLoaderData } from 'react-router-dom';
import { eventsApi } from '@/api/events';

export default function ViewEventPage() {
    const { t } = useTranslation();
    const { event, media } = useRouteLoaderData<{
        event: EventResponse;
        media: EventMediaResponse[];
    }>('view-event')!;

    return (
        <>
            <div className="d-flex mb-3">
                <h2 className="flex-fill">{event.name}</h2>
                <Link to="share">
                    <div className="btn btn-primary">{t('Share')}</div>
                </Link>
            </div>
            <div className="mb-3">
                <EventArchiveLink eventId={event.id} />
            </div>
            <div className="row">
                {media.map((m) => (
                    <SingleEventFile key={m.id} media={m} />
                ))}
            </div>
        </>
    );
}

function EventArchiveLink({ eventId }: { eventId: string }) {
    const { t } = useTranslation();
    const [archive, setArchive] = useState<EventMediaResponse | null>(null);
    const url = import.meta.env.VITE_API_BASE_URL;

    useEffect(() => {
        async function fetchArchiveId() {
            try {
                const response = await eventsApi.getEventArchive(eventId);
                setArchive(response);
            } catch (error) {
                setArchive(null);
                console.error('Error fetching archive URL:', error);
            }
        }

        fetchArchiveId();
    }, [eventId]);

    if (!archive || !archive.files.length) {
        return null;
    }

    return (
        <div className="card">
            <div className="card-body">
                <a
                    href={`${url}/api/media/file/${archive.files[0].id}`}
                    download={archive.files[0].id}
                    className="d-flex align-items-center gap-2"
                >
                    <FolderArchive />
                    {t('Download all files')}
                </a>
            </div>
        </div>
    );
}

function SingleEventFile({ media }: { media: EventMediaResponse }) {
    const url = import.meta.env.VITE_API_BASE_URL;
    return (
        <div className="col-sm-2 col-md-4 col-lg-3 mb-4">
            <div className="card eventImageCard">
                <img
                    className="card-img-top"
                    loading="lazy"
                    src={`${url}/api/media/file/${media.files[0].id}`}
                />
                <div className="card-body">
                    <div className="card-text">
                        <User className="me-1" />
                        {media.participant ? media.participant.name : 'Unknown'}
                    </div>
                </div>
            </div>
        </div>
    );
}
