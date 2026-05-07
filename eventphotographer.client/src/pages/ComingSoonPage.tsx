import { useNavigate } from 'react-router-dom';

export default function ComingSoonPage() {
    const navigate = useNavigate();

    return (
        <div className="d-flex flex-column align-items-center gap-3">
            <h1>Coming soon!</h1>
            <p className="text-muted">
                This feature is not yet available. We are still working on it!
            </p>
            <button onClick={() => navigate(-1)} className="btn btn-primary">
                Go back
            </button>
        </div>
    );
}
