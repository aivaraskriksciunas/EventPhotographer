import { useCallback, useState } from 'react';
import { useDropzone } from 'react-dropzone';
import UploadedFileHandler from './UploadedFileHandler';

export default function UploadEventFilesDropzone() {
    const [files, setFiles] = useState<File[]>([]);

    const onDrop = useCallback(
        (acceptedFiles: File[]) => {
            setFiles([...files, ...acceptedFiles]);
        },
        [files],
    );

    const { getRootProps, getInputProps, isDragActive } = useDropzone({
        onDrop,
        multiple: true,
    });

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
                {files.map((file, i) => (
                    <div className="col-md-3" key={i}>
                        <UploadedFileHandler rawFile={file} />
                    </div>
                ))}
            </div>
        </>
    );
}
