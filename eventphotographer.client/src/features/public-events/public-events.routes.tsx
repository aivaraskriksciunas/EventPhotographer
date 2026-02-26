import { MiddlewareFunction, redirect, RouteObject } from 'react-router-dom';
import JoinEventPage from './pages/JoinEventPage';
import ViewEventPage from './pages/ViewEventPage';
import { useParticipant } from '@/state/participant';

const hasActiveEventMiddleware: MiddlewareFunction = async (_, next) => {
    const joinedEvent = useParticipant.getState().participant;

    if (null === joinedEvent) {
        throw redirect("/login");
    }

    await next();
}

export const publicEventsRoutes: RouteObject[] = [
    {
        path: '/join',
        element: <JoinEventPage />,
    },
    {
        path: '/events/current',
        element: <ViewEventPage />,
        middleware: [hasActiveEventMiddleware],
    }
];
