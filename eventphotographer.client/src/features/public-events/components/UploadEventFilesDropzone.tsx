import { useCallback, useEffect, useState } from 'react';
import { useDropzone } from 'react-dropzone';
import { useTranslation } from 'react-i18next';
import {
    PENDING_STATES,
    TERMINAL_STATES,
    useFileUploadState,
    UserUploadedFile,
} from '@/state/fileUpload';
import { mediaApi } from '@/api/media';
import { useShallow } from 'zustand/shallow';
import clsx from 'clsx';
import { AnimatePresence, motion } from 'motion/react';
import { FileImage, Upload } from 'lucide-react';

export default function UploadEventFilesDropzone() {
    const addFilesToQueue = useFileUploadState((state) => state.addFiles);
    const uploadedFiles = useFileUploadState(
        useShallow((state) =>
            state.currentBatchFileQueue().sort((a, b) => {
                let bp = b.progress;
                let ba = a.progress;
                if (PENDING_STATES.includes(b.state)) {
                    bp += 10;
                }
                if (PENDING_STATES.includes(a.state)) {
                    ba += 10;
                }

                return bp - ba;
            }),
        ),
    );
    const completedFiles = useFileUploadState(
        useShallow((state) =>
            state.fileQueue.filter((f) => TERMINAL_STATES.includes(f.state)),
        ),
    );
    const progress = useFileUploadState((state) => state.progress);

    const { t } = useTranslation();

    const onDrop = useCallback(
        (acceptedFiles: File[]) => {
            addFilesToQueue(acceptedFiles, mediaApi.createMediaUploadHandler);
        },
        [addFilesToQueue],
    );

    const { getRootProps, getInputProps, isDragActive } = useDropzone({
        onDrop,
        multiple: true,
    });

    return (
        <>
            <div
                className={clsx('dropzone mb-4', {
                    'dropzone-active': isDragActive,
                })}
                {...getRootProps()}
            >
                <input {...getInputProps()}></input>
                {isDragActive ? (
                    <div className="dropzone__help">
                        <div className="dropzone__file-icon">
                            <Upload size={48} />
                        </div>
                        <div className="dropzone__help-text">
                            <div className="dropzone__help-text__title">
                                {t('Drop your files here')}
                            </div>
                            <div className="dropzone__help-text__tip">
                                {t('Accepted file types')}: .jpeg, .png, .gif,
                                .crw, .mp4, .m4a, .m4v, .wmv, .avi, .wav, .webp,
                                .mov
                            </div>
                        </div>
                    </div>
                ) : (
                    <div className="dropzone__help">
                        <div className="dropzone__file-icon">
                            <FileImage size={48} />
                        </div>
                        <div className="dropzone__help-text">
                            <div className="dropzone__help-text__title">
                                {t('Upload or drag your files here')}
                            </div>
                            <div className="dropzone__help-text__tip">
                                {t('Accepted file types')}: .jpeg, .png, .gif,
                                .crw, .mp4, .m4a, .m4v, .wmv, .avi, .wav, .webp,
                                .mov
                            </div>
                        </div>
                    </div>
                )}
            </div>
            <UploadProgressBar
                progress={progress}
                filesInProgress={uploadedFiles}
            />
            <div className="row mt-3">
                <AnimatePresence>
                    {completedFiles.map((file) => (
                        <motion.div
                            className="col-6 col-md-4 col-lg-2 mb-3"
                            key={file._id}
                            initial={{
                                opacity: 0,
                                transform: 'translateY(-5%)',
                            }}
                            animate={{
                                opacity: 1,
                                transform: 'translateY(0)',
                                transition: { duration: 0.8 },
                            }}
                            exit={{ opacity: 0, transform: 'translateY(-5%)' }}
                        >
                            <SingleUploadedFile file={file} />
                        </motion.div>
                    ))}
                </AnimatePresence>
            </div>
        </>
    );
}

function SingleUploadedFile({ file }: { file: UserUploadedFile }) {
    const imageId = file.mediaResponse?.files[0]?.id;
    const { t } = useTranslation();

    const displayImage = () => {
        if (file.state === 'complete' && imageId) {
            return (
                <img
                    className="card-image uploaded-file__image"
                    src={mediaApi.getFileUrl(imageId)}
                ></img>
            );
        }

        return (
            <div className="card-image uploaded-file__image">
                {t('Validating')}...
            </div>
        );
    };

    return (
        <div
            className={clsx('card uploaded-file', {
                'card-danger': file.state === 'failed',
            })}
        >
            {displayImage()}
            <div className="card-body">
                <div className="card-text">{file.file.name}</div>
            </div>
        </div>
    );
}

function UploadProgressBar({
    progress,
    filesInProgress,
}: {
    progress: number;
    filesInProgress: UserUploadedFile[];
}) {
    const [showFade, setShowFade] = useState(filesInProgress.length > 3);

    useEffect(() => {
        setShowFade(filesInProgress.length > 3);
    }, [filesInProgress]);

    if (progress === 0 && filesInProgress.length === 0) {
        return null;
    }

    return (
        <div className="card">
            <div className="card-body">
                <div
                    className="progress mb-2"
                    role="progressbar"
                    aria-label="Basic example"
                    aria-valuenow={progress * 100}
                    aria-valuemin={0}
                    aria-valuemax={100}
                >
                    <div
                        className={clsx('progress-bar', {
                            'bg-success': progress >= 0.999,
                            'progress-bar-striped progress-bar-animated':
                                progress < 0.999,
                        })}
                        style={{
                            width: progress * 100 + '%',
                        }}
                    >
                        {
                            filesInProgress.filter((f) =>
                                TERMINAL_STATES.includes(f.state),
                            ).length
                        }{' '}
                        / {filesInProgress.length}
                    </div>
                </div>

                <div className="file-upload-list position-relative">
                    <AnimatePresence>
                        {filesInProgress.slice(0, 3).map((file) => (
                            <motion.div
                                className={clsx(
                                    'inner-card file-upload__single-file',
                                    {
                                        'inner-card-success':
                                            file.progress === 1,
                                    },
                                )}
                                key={file._id}
                                initial={{
                                    opacity: 0,
                                    transform: 'translateY(-5%)',
                                }}
                                animate={{
                                    opacity: 1,
                                    transform: 'translateY(0)',
                                    transition: { duration: 0.5 },
                                }}
                            >
                                <div className="file-upload__single-file__name">
                                    <FileImage className="me-1" />
                                    {file.file.name}
                                </div>
                                <div className="file-upload__single-file__progress">
                                    {Math.round(file.progress * 100)}%
                                </div>
                            </motion.div>
                        ))}
                    </AnimatePresence>
                    {showFade ? <div className="list-fadeout"></div> : null}
                </div>
            </div>
        </div>
    );
}
