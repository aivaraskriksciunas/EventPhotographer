import { LoaderCircle } from 'lucide-react';

export function Spinner() {
    return (
        <div className="loading-spinner-container">
            <span className="loading-spinner"></span>
        </div>
    );
}

export const InlineSpinner = () => (
    <LoaderCircle aria-label="Loading" className="spinner" />
);
