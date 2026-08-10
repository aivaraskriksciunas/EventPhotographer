import { FileUploadHandler } from '@/api/utils/FileUploadHandler';
import { mediaApi } from '@/api/media';
import axios, { AxiosProgressEvent } from 'axios';

export class EventMediaUploadHandler extends FileUploadHandler {
    public constructor(file: File) {
        super(file);
    }

    public async uploadFile(): Promise<void> {
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

            return;
        }

        try {
            await mediaApi.uploadFile(
                createMediaResponse.uploadUrl,
                this.file,
                (ev: AxiosProgressEvent) =>
                    this.emitProgressEvent(ev.progress ?? 0),
            );

            this.emitUploadedEvent();
            this.pollMediaStatus(createMediaResponse.media.id);
        } catch (e) {
            if (axios.isAxiosError(e)) {
                if (e.response?.status === 405) {
                    this.emitErrorEvent(
                        new Error(
                            'File size is too large. Maximum allowed size is 50MB.',
                        ),
                    );
                }

                return;
            }

            this.emitErrorEvent(
                new Error(
                    'Unexpected error occurred during file upload. Please try again.',
                ),
            );

            return;
        }
    }

    private async pollMediaStatus(
        id: string,
        tryCounter: number = 0,
        failureCounter = 0,
    ) {
        ++tryCounter;
        if (tryCounter >= 100 || failureCounter >= 3) {
            this.emitErrorEvent(
                new Error(
                    'Could not verify if the file was uploaded. Please try again later',
                ),
            );
        }

        try {
            const response = await mediaApi.getStatus(id);
            if (response.status === 'Validated') {
                this.emitValidatedEvent(response);

                return;
            }
        } catch {
            failureCounter++;
        }

        setTimeout(
            () => this.pollMediaStatus(id, tryCounter, failureCounter),
            5000,
        );
    }
}
