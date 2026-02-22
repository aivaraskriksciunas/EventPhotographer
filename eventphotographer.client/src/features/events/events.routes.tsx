import { RouteObject } from 'react-router-dom';
import NewEventPage from './pages/NewEventPage';
import ListEventsPage from './pages/ListEventsPage';
import { eventsApi } from '@/api/events';
import ViewEventPage from './pages/ViewEventPage';
import ShareEventPage from './pages/ShareEventPage';

export const eventsRoutes: RouteObject[] = [
    {
        path: '/events',
        element: <ListEventsPage />,
        loader: eventsApi.getAllEvents,
    },
    {
        path: '/events/new',
        element: <NewEventPage />,
    },
    {
        id: 'view-event',
        path: '/events/:eventId',
        loader: async ({ params }) => {
            return await eventsApi.getEvent(params.eventId!);
        },
        children: [
            {
                path: '',
                element: <ViewEventPage />,
            },
            {
                path: 'share',
                element: <ShareEventPage />,
            }
        ]
    },
];
