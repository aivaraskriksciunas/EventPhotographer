import { MediaResponse } from '@/api/media';
import { useFileUploadState } from '@/state/fileUpload';
import { useCallback, useEffect } from 'react';
import { useShallow } from 'zustand/shallow';

const uploadConcurrency = 2;

export default function FileUploaderWrapper({
    children,
}: {
    children: React.ReactNode;
}) {
    const fileQueue = useFileUploadState(
        useShallow((state) => state.fileQueue),
    );
    const setFileState = useFileUploadState((state) => state.setFileState);
    const setFileProgress = useFileUploadState(
        (state) => state.setFileProgress,
    );

    const enqueueFileIfAvailable = useCallback(() => {
        const waitingToUpload = fileQueue.filter((f) => f.state === 'pending');
        const uploadingCount = fileQueue.filter(
            (f) => f.state === 'uploading',
        ).length;
        if (uploadingCount >= uploadConcurrency) {
            return;
        }

        const file = waitingToUpload[0] ?? null;
        if (file === null) {
            return;
        }

        setFileState(file, 'uploading');
        file.uploadHandler.onError((err) => {
            setFileState(file, 'failed', { message: err.message });
        });
        file.uploadHandler.onProgress((progress: number) => {
            setFileProgress(file, progress);
        });
        file.uploadHandler.onUploaded(() => {
            setFileState(file, 'validating');
            setFileProgress(file, 1);
        });
        file.uploadHandler.onValidated((media: MediaResponse) => {
            setFileState(file, 'complete', { mediaResponse: media });
        });

        file.uploadHandler.uploadFile();
    }, [fileQueue, setFileProgress, setFileState]);

    useEffect(enqueueFileIfAvailable, [fileQueue, enqueueFileIfAvailable]);

    return children;
}
