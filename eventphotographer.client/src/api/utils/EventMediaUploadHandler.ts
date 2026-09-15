import { FileUploadHandler } from '@/api/utils/FileUploadHandler';
import { mediaApi } from '@/api/media';
import axios, { AxiosProgressEvent } from 'axios';

export class EventMediaUploadHandler extends FileUploadHandler {
    public constructor(file: File) {
        super(file);
    }

    public async uploadFile(): Promise<string | null> {
        let createMediaResponse = null;
        try {
            createMediaResponse = await mediaApi.createMedia(
                this.file.type,
                this.file.size,
            );
        } catch {
            this.emitErrorEvent(
                new Error(
                    'File size is too large. Maximum allowed size is 50MB.',
                ),
            );

            return null;
        }

        try {
            await mediaApi.uploadFile(
                createMediaResponse.uploadUrl,
                this.file,
                (ev: AxiosProgressEvent) =>
                    this.emitProgressEvent(ev.progress ?? 0),
            );

            this.emitUploadedEvent();
        } catch (e) {
            if (axios.isAxiosError(e)) {
                if (e.response?.status === 405) {
                    this.emitErrorEvent(
                        new Error(
                            'File size is too large. Maximum allowed size is 50MB.',
                        ),
                    );
                }

                return null;
            }

            this.emitErrorEvent(
                new Error(
                    'Unexpected error occurred during file upload. Please try again.',
                ),
            );

            return null;
        }

        return createMediaResponse.media?.id ?? null;
    }
}
