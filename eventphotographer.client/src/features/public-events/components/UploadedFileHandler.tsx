import { AxiosProgressEvent } from 'axios';
import { mediaApi } from '@/api/media';
import { useEffect, useRef, useState } from 'react';
import clsx from 'clsx';
import { CircleCheck } from 'lucide-react';
import { truncate } from '@/utils/helpers';

type FileUploadStatus = 'Pending' | 'Uploading' | 'Success' | 'Error';

interface UploadedFile {
    rawFile: File;
    uploadId?: string;
    error?: string;
    status: FileUploadStatus;
}

export default function UploadedFileHandler({ rawFile }: { rawFile: File }) {
    const isUploaded = useRef(false);
    const [file, setFile] = useState<UploadedFile>({
        rawFile: rawFile,
        status: 'Pending' as FileUploadStatus,
    });
    const [progress, setProgress] = useState<number>(0);

    const onFileUploadProgress = (ev: AxiosProgressEvent) => {
        setProgress(Math.round((ev.progress ?? 0) * 100));
    };

    const uploadFile = async (file: UploadedFile) => {
        try {
            const response = await mediaApi.createMedia();
            setFile({
                ...file,
                status: 'Uploading' as FileUploadStatus,
            });
            await mediaApi.uploadFile(
                response.uploadToken,
                file.rawFile,
                onFileUploadProgress,
            );
        } catch {
            setFile({
                ...file,
                status: 'Error',
                error: 'Could not upload the file due to server error',
            });
            return;
        }

        setFile({
            ...file,
            status: 'Success',
        });
    };

    useEffect(() => {
        if (isUploaded.current === true) {
            return;
        }

        uploadFile(file);
        isUploaded.current = true;
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [rawFile]);

    return (
        <div className={clsx("card mb-3", {'card-success': file.status === 'Success', 'card-error': file.status === 'Error'})}>
            <div className="card-body">
                <div className="card-title d-flex align-items-center">
                    {progress === 100 ? <CircleCheck className='me-1' /> : null}
                    {truncate(file.rawFile.name, 20)}
                </div>
                {progress < 100 ? (
                    <div
                        className="progress"
                        role="progressbar"
                        aria-label="Basic example"
                        aria-valuenow={progress}
                        aria-valuemin={0}
                        aria-valuemax={100}
                    >
                        <div
                            className="progress-bar"
                            style={{ width: progress + '%' }}
                        ></div>
                    </div>
                ) : null}

                {file?.error ? (
                    <p className="card-text text-danger">{file.error}</p>
                ) : null}
            </div>
        </div>
    );
}
