import { useFileUploadState } from '@/state/fileUpload';
import { useCallback, useEffect } from 'react';
import { useShallow } from 'zustand/shallow';
import {
    GeneratedThumbnailNotification,
    UploadFileNotification,
    uploadNotificationsApi,
} from '@/api/upload-notifications';
import { useTranslation } from 'react-i18next';

const uploadConcurrency = 2;

export default function FileUploaderWrapper({
    children,
}: {
    children: React.ReactNode;
}) {
    const { t } = useTranslation();
    const fileQueue = useFileUploadState(
        useShallow((state) => state.fileQueue),
    );
    const setFileState = useFileUploadState((state) => state.setFileState);
    const setFileProgress = useFileUploadState(
        (state) => state.setFileProgress,
    );
    const setExternalId = useFileUploadState((state) => state.setExternalId);

    useEffect(() => {
        const handleUpload = (notification: UploadFileNotification) => {
            const file = useFileUploadState
                .getState()
                .fileQueue.find((f) => f.externalId === notification.mediaId);

            if (!file) {
                return;
            }

            if (notification.status === 'Validated') {
                setFileState(file, 'complete');
            } else if (notification.status === 'Invalid') {
                setFileState(file, 'failed', {
                    message: t('File did not pass validation'),
                });
            }
        };

        const handleThumbnailGenerated = (
            notification: GeneratedThumbnailNotification,
        ) => {
            const file = useFileUploadState
                .getState()
                .fileQueue.find((f) => f.externalId === notification.mediaId);

            if (!file) {
                return;
            }

            setFileState(file, file.state, {
                thumbnailFileId: notification.thumbnailFileId,
            });
        };

        uploadNotificationsApi.onUploadNotification(handleUpload);
        uploadNotificationsApi.onThumbnailGeneratedNotification(
            handleThumbnailGenerated,
        );

        return () => {
            uploadNotificationsApi.offUploadNotification(handleUpload);
            uploadNotificationsApi.offThumbnailGeneratedNotification(
                handleThumbnailGenerated,
            );
        };
    }, [setFileState, t]);

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

        file.uploadHandler.uploadFile().then((externalId) => {
            if (externalId !== null) {
                setExternalId(file, externalId);
            }
        });
    }, [fileQueue, setFileProgress, setFileState, setExternalId]);

    useEffect(enqueueFileIfAvailable, [fileQueue, enqueueFileIfAvailable]);

    return children;
}
