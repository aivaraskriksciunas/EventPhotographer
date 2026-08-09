import { MediaResponse } from '@/api/media';
import { FileUploadHandler } from '@/api/utils/FileUploadHandler';
import { create } from 'zustand';

export type UserUploadedFileState =
    | 'pending'
    | 'uploading'
    | 'validating'
    | 'complete'
    | 'failed';

let counter = 0;
export const TERMINAL_STATES: UserUploadedFileState[] = [
    'validating',
    'complete',
    'failed',
];
export const PENDING_STATES: UserUploadedFileState[] = ['pending', 'uploading'];

export interface UserUploadedFile {
    _id: number;
    _batchId: number;
    file: File;
    state: UserUploadedFileState;
    progress: number;
    uploadHandler: FileUploadHandler;
    message?: string;
    mediaResponse?: MediaResponse;
}

export interface FileUploadState {
    fileQueue: UserUploadedFile[];
    progress: number;
    currentBatch: number;
    currentBatchFileQueue: () => UserUploadedFile[];
    addFile: (file: File, uploadHandler: FileUploadHandler) => void;
    addFiles: (
        files: File[],
        uploadHandlerFactory: (file: File) => FileUploadHandler,
    ) => void;
    setFileState: (
        file: UserUploadedFile,
        fileState: UserUploadedFileState,
        additionalData?: { message?: string; mediaResponse?: MediaResponse },
    ) => void;
    setFileProgress: (file: UserUploadedFile, progress: number) => void;
    getAndUpdateBatchId: () => number;
}

export const useFileUploadState = create<FileUploadState>((set, get) => ({
    fileQueue: [],
    progress: 0,
    currentBatch: 0,
    currentBatchFileQueue: () =>
        get().fileQueue.filter((f) => f._batchId === get().currentBatch),
    addFile: (file: File, uploadHandler: FileUploadHandler) => {
        set((state) => ({
            fileQueue: [
                ...state.fileQueue,
                {
                    _id: counter++,
                    _batchId: get().getAndUpdateBatchId(),
                    file,
                    state: 'pending',
                    progress: 0,
                    uploadHandler,
                },
            ],
        }));
    },
    addFiles: (
        files: File[],
        uploadHandlerFactory: (file: File) => FileUploadHandler,
    ) => {
        set((state) => ({
            fileQueue: [
                ...state.fileQueue,
                ...files.map<UserUploadedFile>((f) => ({
                    _id: counter++,
                    _batchId: get().getAndUpdateBatchId(),
                    file: f,
                    state: 'pending',
                    progress: 0,
                    uploadHandler: uploadHandlerFactory(f),
                })),
            ],
            progress: state.progress > 0.999 ? 0 : state.progress,
        }));
    },
    setFileState: (
        file: UserUploadedFile,
        fileState: UserUploadedFileState,
        additionalData?: { message?: string; mediaResponse?: MediaResponse },
    ) =>
        set((state) => ({
            fileQueue: state.fileQueue.map<UserUploadedFile>((f) => {
                if (f._id === file._id) {
                    return { ...f, state: fileState, ...additionalData };
                }

                return f;
            }),
        })),
    setFileProgress: (file: UserUploadedFile, progress: number) => {
        set((state) => ({
            fileQueue: state.fileQueue.map<UserUploadedFile>((f) => {
                if (f._id === file._id) {
                    return { ...f, progress };
                }

                return f;
            }),
        }));

        const currentBatch = get().currentBatch;
        const batchQueue = get().fileQueue.filter(
            (f) => f._batchId === currentBatch,
        );
        const totalProgress =
            batchQueue.reduce((acc, file) => {
                return acc + (file.progress > 0.999 ? 1 : file.progress);
            }, 0) / batchQueue.length;

        set({ progress: totalProgress > 0.999 ? 1 : totalProgress });
    },
    getAndUpdateBatchId: () => {
        let batchId = get().currentBatch;
        if (get().progress > 0.999) {
            set({
                currentBatch: ++batchId,
                progress: 0,
            });
        }

        return batchId;
    },
}));
