import { mediaApi } from "@/api/media";
import { AxiosProgressEvent } from "axios";
import clsx from "clsx"
import axios from "axios";
import { useEffect, useRef, useState } from "react";
import { CheckCircle, TriangleAlert } from "lucide-react";

type FileUploadStatus = 'Pending' | 'Uploading' | 'Success' | 'Error';

interface UploadedFile {
    rawFile: File;
    error?: string;
    status: FileUploadStatus;
}

export default function UploadedBatchHandler({ batch, onSuccess }: { batch: File[], onSuccess?: () => void }) {
    const [hasError, setHasError] = useState(false);
    const [isComplete, setIsComplete] = useState(false);
    const [uploadedFileCount, setUploadedFileCount] = useState(0);
    const [progressList, setProgressList] = useState<number[]>(batch.map(() => 0));
    const [progress, setProgress] = useState<number>(0);

    useEffect(() => {
        const totalProgress = progressList.reduce((acc, curr) => acc + curr, 0);
        const percentage = Math.round((totalProgress / batch.length) * 100);
        setProgress(percentage);
        setIsComplete(percentage === 100);

        if (percentage === 100 && !hasError) {
            onSuccess?.();
        }
    }, [progressList])

    const updateFileProgress = (index: number, fileProgress: number) => {
        setProgressList(prev => {
            const newProgressList = [...prev];
            newProgressList[index] = fileProgress;
            return newProgressList;
        });
    }

    return (
        <div
            className={clsx('card mb-3', {
                'card-success': isComplete && !hasError,
                'card-error': hasError,
            })}
        >
            <div className="card-body">
                <div className="mb-1">
                    {batch.map((file, i) => (
                        <SingleFileUploader 
                            key={i} 
                            rawFile={file} 
                            onProgress={(progress) => updateFileProgress(i, progress)}
                            onComplete={() => {
                                updateFileProgress(i, 1);
                                setUploadedFileCount(prev => prev + 1);
                            }}
                            onError={() => {
                                updateFileProgress(i, 1);
                                setHasError(true);
                            }}
                        />
                    ))}
                    <div>
                        {isComplete && <CheckCircle className='me-2' />}
                        {uploadedFileCount} / {batch.length} files uploaded.
                    </div>
                </div>
                <div
                    className="progress"
                    role="progressbar"
                    aria-label="Basic example"
                    aria-valuenow={progress}
                    aria-valuemin={0}
                    aria-valuemax={100}
                >
                    <div
                        className={clsx('progress-bar', {
                            'bg-success': isComplete && !hasError,
                            'bg-danger': hasError,
                            'progress-bar-striped progress-bar-animated': !isComplete,
                        })}
                        style={{ width: progress + '%' }}
                    ></div>
                </div>
            </div>
        </div>
    )
}

function SingleFileUploader({ rawFile, onProgress, onComplete, onError }: { rawFile: File, onProgress: (progress: number) => void, onComplete: () => void, onError: (error: string) => void }) {
    const isUploaded = useRef(false);
    const [file, setFile] = useState<UploadedFile>({
        rawFile: rawFile,
        status: 'Pending' as FileUploadStatus,
    });

    const updateFile = (update: Partial<UploadedFile>) => {
        setFile(prev => ({...prev, ...update}));
    }

    const setFileError = (error: string) => {
        onError(error);
        updateFile({ error, status: 'Error' as FileUploadStatus });
    };

    const updateFileUploadProgress = (ev: AxiosProgressEvent) => {
        onProgress(ev.progress ?? 0);
    };

    const uploadFile = async () => { 
        let response = null;   
        try {
            response = await mediaApi.createMedia();
            updateFile({ status: 'Uploading' as FileUploadStatus });
        } catch (e) {
            setFileError('Failed to upload file.');
            return;
        }   

        try {
            await mediaApi.uploadFile(
                response.uploadToken,
                file.rawFile,
                (ev: AxiosProgressEvent) => updateFileUploadProgress(ev),
            );
        } catch (e) {
            if (axios.isAxiosError(e)) {
                if (e.response?.status === 405) {
                    setFileError('File size is too large. Maximum allowed size is 200MB.');
                }

                return;
            }
            
            setFileError('Unexpected error occurred during file upload. Please try again.');
            return;
        }

        updateFile({ status: 'Success' as FileUploadStatus });
        onComplete();
    };

    useEffect(() => {
        if (isUploaded.current === true) {
            return;
        }
        
        uploadFile();
        isUploaded.current = true;
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [rawFile])

    if (file.status === 'Error') {
        return (
            <div className="text-danger d-flex align-items-center gap-1">
                <TriangleAlert />
                {rawFile.name} - <span className="text-danger">{file.error}</span>
            </div>
        )
    }

    return null;
}