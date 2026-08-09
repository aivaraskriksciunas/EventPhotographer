import axios, { AxiosProgressEvent } from 'axios';
import { fetchApi } from './client';
import { EventMediaUploadHandler } from './utils/EventMediaUploadHandler';

export interface MediaFileResponse {
    id: string;
    mimeType: string;
    fileSize: string;
}

export interface MediaResponse {
    id: string;
    createdAt: Date;
    status: string;
    files: MediaFileResponse[];
}

export interface CreateMediaResponse {
    uploadUrl: string;
    media: MediaResponse;
}

export const mediaApi = {
    createMedia: (fileType: string, fileSize: number) =>
        fetchApi<CreateMediaResponse>('/api/media', 'POST', {
            fileType,
            fileSize,
        }),
    getStatus: (id: string) =>
        fetchApi<MediaResponse>(`/api/media/${id}/status`, 'GET'),
    uploadFile: async (
        uploadUrl: string,
        file: File,
        onUploadProgress: (progressEvent: AxiosProgressEvent) => void,
    ) => {
        let fullUrl = `${uploadUrl}`;

        if (import.meta.env.DEV) {
            fullUrl = fullUrl.replace('minio', 'localhost');
        }

        return await axios.put(fullUrl, file, {
            onUploadProgress,
            headers: {},
        });
    },
    createMediaUploadHandler: (file: File) => new EventMediaUploadHandler(file),
    getFileUrl: (fileId: string) =>
        `${import.meta.env.VITE_API_BASE_URL}/api/media/file/${fileId}`,
};
