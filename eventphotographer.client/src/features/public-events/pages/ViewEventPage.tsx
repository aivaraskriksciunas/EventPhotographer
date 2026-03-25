import { useParticipant } from '@/state/participant';
import UploadEventFilesDropzone from '../components/UploadEventFilesDropzone';

export default function ViewEventPage() {
    const { participant } = useParticipant();

    return (
        <>
            <h1>{participant!.event.name}</h1>
            <UploadEventFilesDropzone />
        </>
    );
}
