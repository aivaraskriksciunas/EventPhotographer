import { RouteObject } from 'react-router-dom';
import NewEventPage from './pages/NewEventPage';

export const eventsRoutes: RouteObject[] = [
    {
        path: '/events/new',
        element: <NewEventPage />,
    },
];
