import { useCallback, useState } from 'react';
import { useDropzone } from 'react-dropzone';
import UploadedBatchHandler from './UploadedBatchHandler';
import { AnimatePresence, motion } from "motion/react";

export default function UploadEventFilesDropzone() {
    const [batches, setBatches] = useState<{ id: string, files: File[] }[]>([]);

    const onDrop = useCallback(
        (acceptedFiles: File[]) => {
            setBatches(prev => [
                { id: crypto.randomUUID(), files: acceptedFiles },
                ...prev,
            ]);
        },
        [batches],
    );

    const { getRootProps, getInputProps, isDragActive } = useDropzone({
        onDrop,
        multiple: true,
    });

    const hideUploadedBatch = (id: string) => {
        setTimeout(() => {
            setBatches(prev => prev.filter(batch => batch.id !== id));
        }, 3000);
    }

    return (
        <>
            <div className="dropzone mb-3" {...getRootProps()}>
                <input {...getInputProps()}></input>
                {isDragActive ? (
                    <span>Dragging...</span>
                ) : (
                    <span>Drag files here</span>
                )}
            </div>
            <div className="row">
                <AnimatePresence>
                    {batches.map(batch => (
                        <motion.div
                            className="col-12 mb-3"
                            key={batch.id}
                            initial={{ opacity: 0, transform: 'translateX(-10%)' }}
                            animate={{ opacity: 1, transform: 'translateX(0)', transition: { duration: 0.8 } }}
                            exit={{ opacity: 0, transform: 'translateX(-10%)' }}
                        >
                            <UploadedBatchHandler batch={batch.files} onSuccess={() => hideUploadedBatch(batch.id)}/>
                        </motion.div>
                    ))}
                </AnimatePresence>
            </div>
        </>
    );
}
