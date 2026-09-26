import { RouteObject } from 'react-router-dom';
import ListEventsPage from './pages/ListEventsPage';
import { eventsApi } from '@/api/events';
import ViewEventPage from './pages/ViewEventPage';
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
                lazy: {
                    Component: async () =>
                        (await import('./pages/NewEventPage')).default,
                },
            },
            {
                id: 'view-event',
                path: ':eventId',
                loader: async ({ params }) => {
                    const [event, media] = await Promise.all([
                        eventsApi.getEvent(params.eventId!),
                        eventsApi.getEventMedia(params.eventId!, {
                            page: 1,
                            pageSize: 16,
                        }),
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
                        lazy: {
                            Component: async () =>
                                (await import('./pages/ShareEventPage'))
                                    .default,
                        },
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
