import { mediaApi } from "@/api/media";
import { useCallback, useState } from "react";
import { useDropzone } from "react-dropzone";

type FileUploadStatus = "Pending" | "Uploading" | "Success" | "Error";

interface UploadedFile {
    file: File,
    uploadId?: string,
    error?: string,
    status: FileUploadStatus,
}

export default function UploadEventFilesDropzone() {
    const [files, setFiles] = useState<UploadedFile[]>([]);
    
    const setSingleFileState = (file: UploadedFile) => {
        setFiles(state => state.map(f => f.file === file.file ? file : f))
    }

    const onFileUploadProgress = (ev) => {
        console.log(ev)
    }

    const uploadFile = async (uploadedFile: UploadedFile) => {
        try {
            uploadedFile.status = 'Uploading';
            setSingleFileState(uploadedFile);

            let response = await mediaApi.createMedia();
            await mediaApi.uploadFile(
                response.uploadToken,
                uploadedFile.file,
                onFileUploadProgress,
            )
        }
        catch (e) {
            console.log(e)
            uploadedFile.status = 'Error';
            uploadedFile.error = "Could not upload the file due to server error"
            setSingleFileState(uploadedFile);
            return;
        }

        uploadedFile.status = 'Success';
        setSingleFileState(uploadedFile);
    }

    const onDrop = useCallback((acceptedFiles: File[]) => {
        const uploadedFiles = acceptedFiles.map(f => ({file: f, status: 'Pending' as FileUploadStatus}));
        const uploadPromises = uploadedFiles.map(f => uploadFile(f));
        setFiles([...files, ...uploadedFiles]);
        Promise.all(uploadPromises);
    }, []);

    const {getRootProps, getInputProps, isDragActive} = useDropzone({
        onDrop,
        multiple: true,
    });

    return (
        <>
            <div className='dropzone' {...getRootProps()}>
                <input {...getInputProps()}></input>
                {
                    isDragActive ? <span>Dragging...</span> : <span>Drag files here</span>
                }
            </div>
            <div className='row'>
                {files.map((_, i) => <div className="col-md-4">File {i+1} uploading</div>)}
            </div>
        </>
        
    );
}