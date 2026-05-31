import { MiddlewareFunction, redirect, RouteObject } from 'react-router-dom';
import JoinEventPage from './pages/JoinEventPage';
import ViewEventPage from './pages/ViewEventPage';
import { useParticipant } from '@/state/participant';
import { eventsApi } from '@/api/events';

const hasActiveEventMiddleware: MiddlewareFunction = async (_, next) => {
    const joinedEvent = useParticipant.getState().participant;

    if (null === joinedEvent) {
        throw redirect('/login');
    }

    await next();
};

export const publicEventsRoutes: RouteObject[] = [
    {
        path: '/join/:code?',
        element: <JoinEventPage />,
        loader: async ({ params }) => {
            if (params.code != null) {
                let link = null;
                try {
                    link = await eventsApi.getShareableLinkByCode(params.code)
                } catch { link = null; }

                return link;
            }

            return null;
        },
    },
    {
        path: '/events/current',
        element: <ViewEventPage />,
        middleware: [hasActiveEventMiddleware],
    },
];
