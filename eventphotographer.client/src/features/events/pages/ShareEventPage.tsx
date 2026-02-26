import { eventsApi, EventShareableLinkResponse } from '@/api/events';
import { useState } from 'react';
import {
    useLoaderData,
    useNavigate,
    useRouteLoaderData,
} from 'react-router-dom';

export default function ShareEventPage() {
    const event = useRouteLoaderData('view-event');
    const shareableLinks = useLoaderData();

    return (
        <>
            <h2>Share Event: {event.name}</h2>
            <div>
                {shareableLinks && shareableLinks.length > 0 ? (
                    shareableLinks.map((link: EventShareableLinkResponse) => (
                        <ShareableLinkItem key={link.id} link={link} />
                    ))
                ) : (
                    <CreateShareableLinkAction event={event} />
                )}
            </div>
        </>
    );
}

function ShareableLinkItem({ link }: { link: EventShareableLinkResponse }) {
    return (
        <div className="card">
            <div className="card-body">
                <p>Code: {link.code}</p>
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
        } catch (error) {
            // TODO: Error handling
        } finally {
            setIsLoading(false);
            navigate(0); // Refresh the page
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
