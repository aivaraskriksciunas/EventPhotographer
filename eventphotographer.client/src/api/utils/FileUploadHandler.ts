import { MediaResponse } from '../media';

export abstract class FileUploadHandler extends EventTarget {
    protected file: File;

    public constructor(file: File) {
        super();
        this.file = file;
    }

    public onProgress(callback: (progress: number) => void) {
        this.addEventListener('progress', (ev: Event) =>
            callback((ev as CustomEvent<number>).detail),
        );
    }

    public onError(callback: (error: Error) => void) {
        this.addEventListener('error', (ev: Event) =>
            callback((ev as CustomEvent<Error>).detail),
        );
    }

    public onUploaded(callback: () => void) {
        this.addEventListener('uploaded', () => callback());
    }

    public onValidated(callback: (media: MediaResponse) => void) {
        this.addEventListener('validated', (ev: Event) =>
            callback((ev as CustomEvent<MediaResponse>).detail),
        );
    }

    public abstract uploadFile(): Promise<void>;

    protected emitProgressEvent(progress: number) {
        this.dispatchEvent(new CustomEvent('progress', { detail: progress }));
    }

    protected emitUploadedEvent() {
        this.dispatchEvent(new CustomEvent('uploaded'));
    }

    protected emitValidatedEvent(data: MediaResponse) {
        this.dispatchEvent(new CustomEvent('validated', { detail: data }));
    }

    protected emitErrorEvent(error: Error) {
        this.dispatchEvent(new CustomEvent('error', { detail: error }));
    }
}
