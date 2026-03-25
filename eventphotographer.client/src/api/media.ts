import { AxiosProgressEvent } from "axios";
import { api, fetchApi } from "./client";

export interface MediaFileResponse {
    id: string,
    mimeType: string,
    fileSize: string,
}

export interface MediaResponse {
    uploadToken: string,
    createdAt: Date,
    files: MediaFileResponse[],
}

export const mediaApi = {
    createMedia: () => fetchApi<MediaResponse>('/api/media', 'POST', {}),
    uploadFile: async (uploadKey: string, file: File, onUploadProgress: (progressEvent: AxiosProgressEvent) => void) => {{
        const form = new FormData();
        form.append('file', file);
        
        return await api.postForm(
            `/api/media/${uploadKey}/upload`,
            { file },
            { onUploadProgress }
        )
    }}
}