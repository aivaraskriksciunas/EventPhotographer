import { useAuth } from '@/state/auth';
import { Navigate } from 'react-router-dom';

export default function HomePage() {
    const { user } = useAuth();

    if (user === null) {
        return <Navigate to="/join" />;
    }

    return <Navigate to="/events" />;
}
