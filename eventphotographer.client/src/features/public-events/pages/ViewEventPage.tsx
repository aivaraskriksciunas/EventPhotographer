import { useParticipant } from '@/state/participant';
import UploadEventFilesDropzone from '../components/UploadEventFilesDropzone';
import { WhatsAppLinkButton } from '../../../components/base/WhatsAppLinkButton';

export default function ViewEventPage() {
    const { participant } = useParticipant();

    return (
        <>
            <div className="header">
                <h1>{participant!.event.name}</h1>
                <div className="header-action">
                    {participant?.eventShareableLink != null ? (
                        <WhatsAppLinkButton
                            shareableLink={participant.eventShareableLink}
                            className="w-100"
                        ></WhatsAppLinkButton>
                    ) : null}
                </div>
            </div>

            <UploadEventFilesDropzone />
        </>
    );
}
