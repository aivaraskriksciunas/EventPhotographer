import { EventResponse, eventsApi, EventShareableLinkResponse } from '@/api/events';
import { useState } from 'react';
import {
    useLoaderData,
    useNavigate,
    useRouteLoaderData,
    generatePath,
} from 'react-router-dom';
import { Link as LinkIcon } from 'lucide-react'

export default function ShareEventPage() {
    const { event } = useRouteLoaderData('view-event');
    const shareableLinks = useLoaderData();
    console.log(shareableLinks);

    return (
        <>
            <h2>Share {event.name}</h2>
            <div>
                {shareableLinks && shareableLinks.length > 0 ? (
                    shareableLinks.map((link: EventShareableLinkResponse) => (
                        <ShareableLinkItem key={link.id} event={event} link={link} />
                    ))
                ) : (
                    <CreateShareableLinkAction event={event} />
                )}
            </div>
        </>
    );
}

function ShareableLinkItem({ event, link }: { event: EventResponse, link: EventShareableLinkResponse }) {
    const linkPath = `https://${window.location.host}` + generatePath('/join/:code', { code: link.code });

    return (
        <div className="card">
            <div className="card-body">
                <div className="d-flex shareablelink-item">
                    <div className="flex-grow-1">
                        <div className="shareablelink-code">
                            Code: <span className='badge badge-secondary'>{link.code}</span>
                        </div>
                        <div className='shareablelink-links'>
                            <div><LinkIcon className='me-1'/>{linkPath}</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

function CreateShareableLinkAction({
    event,
}: {
    event: EventShareableLinkResponse;
}) {
    const navigate = useNavigate();
    const [isLoading, setIsLoading] = useState(false);

    const handleCreateLink = async () => {
        setIsLoading(true);

        try {
            await eventsApi.createShareableLink(event.id);
            navigate(0); // Refresh the page
        } catch {
            // TODO: Error handling
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="card">
            <div className="card-body">
                <button
                    type="button"
                    className="btn btn-primary"
                    onClick={handleCreateLink}
                    disabled={isLoading}
                >
                    Create Shareable Link
                </button>
            </div>
        </div>
    );
}
