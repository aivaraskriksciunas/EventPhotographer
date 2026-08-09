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
    fields: Record<string, string>;
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
        fields: Record<string, string>,
        file: File,
        onUploadProgress: (progressEvent: AxiosProgressEvent) => void,
    ) => {
        const formData = new FormData();
        Object.entries(fields).forEach(([key, value]) => {
            formData.append(key, value);
        });
        formData.append('file', file);

        if (import.meta.env.DEV) {
            uploadUrl = uploadUrl.replace('minio', 'localhost');
        }

        return await axios.postForm(uploadUrl, formData, {
            onUploadProgress,
        });
    },
    createMediaUploadHandler: (file: File) => new EventMediaUploadHandler(file),
    getFileUrl: (fileId: string) =>
        `${import.meta.env.VITE_API_BASE_URL}/api/media/file/${fileId}`,
};
