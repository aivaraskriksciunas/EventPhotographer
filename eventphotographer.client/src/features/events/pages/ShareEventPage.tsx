import { EventResponse, eventsApi, EventShareableLinkResponse, WhatsAppMessageLinkResponse } from '@/api/events';
import { useEffect, useState } from 'react';
import {
    useLoaderData,
    useNavigate,
    useRouteLoaderData,
    generatePath,
    Link,
    useRevalidator,
} from 'react-router-dom';
import { Link as LinkIcon } from 'lucide-react'
import { WhatsAppIcon } from '@/components/icons/WhatsAppIcon';
import { useTranslation } from 'react-i18next';

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
                            <ShareableLinkWhatsAppLink event={event} shareableLink={link}></ShareableLinkWhatsAppLink>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}


function ShareableLinkWhatsAppLink({ event, shareableLink }: { event: EventResponse, shareableLink: EventShareableLinkResponse }) {
    const { t } = useTranslation();
    const [linkState, setLinkState] = useState('not-created')
    const { revalidate } = useRevalidator();

    const STATE_LOADING = 'loading';
    const STATE_CREATING = 'creating';
    const STATE_NOT_CREATED = 'not-created'
    const STATE_CREATED = 'created';

    useEffect(() => {
        let link = shareableLink.whatsAppMessageLink;
        if (link === null || link.status === 'failed') {
            setLinkState(STATE_NOT_CREATED);
        } else if (link.status === 'pending') {
            setLinkState(STATE_CREATING);
        } else if (link.status === 'created') {
            setLinkState(STATE_CREATED);
        }
    }, [shareableLink])

    const createWhatsApp = async () => {
        setLinkState(STATE_LOADING);
        try {
            await eventsApi.createWhatsAppLinkForShareableLink(event.id, shareableLink.id);
        } finally {
            revalidate();
        }
    }

    if (linkState === STATE_NOT_CREATED) {
        return (
            <div>
                <span className="me-1">
                    <WhatsAppIcon size={14}/>
                </span>
                <a href='#' onClick={createWhatsApp}>{t("Create WhatsApp Link")}</a>
            </div>
        );
    }
    else if (linkState === STATE_CREATING) {
        return (
            <div>
                <span className="me-1">
                    <WhatsAppIcon size={14}/>
                </span>
                <span>{t("Link is being created and will show up here once its done.")}</span>
            </div>
        );
    }
    else if (linkState === STATE_LOADING) {
        return (
            <div>
                <span className="me-1">
                    <WhatsAppIcon size={14}/>
                </span>
                <span>{t("Loading...")}</span>
            </div>
        );
    }

    return (
        <div>
            <span className="me-1">
                <WhatsAppIcon size={14}/>
            </span>
            <span>{shareableLink.whatsAppMessageLink?.url}</span>
        </div>
    )
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
