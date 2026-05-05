import { RouteObject } from 'react-router-dom';
import NewEventPage from './pages/NewEventPage';
import ListEventsPage from './pages/ListEventsPage';
import { eventsApi } from '@/api/events';
import ViewEventPage from './pages/ViewEventPage';
import ShareEventPage from './pages/ShareEventPage';
import DashboardLayout from '@/layouts/DashboardLayout';

export const eventsRoutes: RouteObject[] = [
    {
        element: <DashboardLayout />,
        path: '/events',
        children: [
            {
                path: '',
                element: <ListEventsPage />,
                loader: eventsApi.getAllEvents,
            },
            {
                path: 'new',
                element: <NewEventPage />,
            },
            {
                id: 'view-event',
                path: ':eventId',
                loader: async ({ params }) => {
                    const [event, media] = await Promise.all([
                        eventsApi.getEvent(params.eventId!),
                        eventsApi.getEventMedia(params.eventId!),
                    ]);

                    return { event, media };
                },
                children: [
                    {
                        path: '',
                        element: <ViewEventPage />,
                    },
                    {
                        path: 'share',
                        element: <ShareEventPage />,
                        loader: async ({ params }) => {
                            return await eventsApi.getShareableLinks(
                                params.eventId!,
                            );
                        },
                    },
                ],
            },
        ],
    },
];
