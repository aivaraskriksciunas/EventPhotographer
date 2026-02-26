import { MiddlewareFunction, redirect, RouteObject } from 'react-router-dom';
import JoinEventPage from './pages/JoinEventPage';
import ViewEventPage from './pages/ViewEventPage';
import { useJoinedEvent } from '@/state/joinedEvent';

const hasActiveEventMiddleware: MiddlewareFunction = async (_, next) => {
    const joinedEvent = useJoinedEvent.getState().event;

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
