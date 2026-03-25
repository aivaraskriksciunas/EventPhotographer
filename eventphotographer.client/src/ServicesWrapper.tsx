import { useEffect } from 'react';
import { useAuth } from './state/auth';
import { useParticipant } from './state/participant';
import { eventsApi } from './api/events';
import { authApi } from './api/auth';

export default function ServicesWrapper({
    children,
}: {
    children: React.ReactNode;
}) {
    const { setUser, logout } = useAuth();
    const { setParticipant } = useParticipant();

    useEffect(() => {
        const initialize = async () => {
            try {
                const currentUser = await authApi.getCurrentUser();
                setUser(currentUser);
            } catch {
                logout();
            }

            try {
                const currentParticipant = await eventsApi.getCurrentEvent();
                setParticipant(currentParticipant);
            } catch {
                setParticipant(null);
            }
        };

        initialize();
    }, [setUser, logout, setParticipant]);

    return <>{children}</>;
}
