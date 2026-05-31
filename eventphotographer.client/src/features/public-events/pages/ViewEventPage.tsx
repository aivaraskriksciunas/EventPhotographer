import { useParticipant } from '@/state/participant';
import UploadEventFilesDropzone from '../components/UploadEventFilesDropzone';
import { WhatsAppLinkButton } from '../../../components/base/WhatsAppLinkButton';

export default function ViewEventPage() {
    const { participant } = useParticipant();

    return (
        <>
            <h1>{participant!.event.name}</h1>

            {participant?.eventShareableLink != null ? 
                <div className='mb-3'>
                    <WhatsAppLinkButton shareableLink={participant.eventShareableLink} className='w-100'></WhatsAppLinkButton>
                </div> 
                : null}
            <UploadEventFilesDropzone />
        </>
    );
}
