import { EventMediaResponse, EventResponse } from '@/api/events';
import { User } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { Link, useRouteLoaderData } from 'react-router-dom';

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
            <div className="row">
                {media.map((m) => (
                    <SingleEventFile media={m} />
                ))}
            </div>
        </>
    );
}

function SingleEventFile({ media }: { media: EventMediaResponse }) {
    return (
        <div className="col-sm-2 col-md-4 col-lg-3 mb-4">
            <div className="card eventImageCard">
                <img
                    className="card-img-top"
                    loading='lazy'
                    src={`http://localhost:5252/api/media/file/${media.files[0].id}`}
                />
                <div className="card-body">
                    <div className="card-text">
                        <User className='me-1'/>
                        {media.participant
                            ? media.participant.name
                            : 'Unknown'}
                    </div>
                </div>
            </div>
        </div>
    );
}
