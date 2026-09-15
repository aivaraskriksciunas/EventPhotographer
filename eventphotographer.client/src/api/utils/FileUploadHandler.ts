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

    public abstract uploadFile(): Promise<string | null>;

    protected emitProgressEvent(progress: number) {
        this.dispatchEvent(new CustomEvent('progress', { detail: progress }));
    }

    protected emitUploadedEvent() {
        this.dispatchEvent(new CustomEvent('uploaded'));
    }

    protected emitErrorEvent(error: Error) {
        this.dispatchEvent(new CustomEvent('error', { detail: error }));
    }
}
