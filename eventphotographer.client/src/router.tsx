import { createBrowserRouter, Navigate } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import { eventsRoutes } from './features/events/events.routes';

export const router = createBrowserRouter([
    {
        element: <MainLayout />,
        errorElement: <Navigate to="/404" replace />,
        children: [{ path: '/' }, ...eventsRoutes],
    },
    {
        path: '/404',
        lazy: {
            Component: async () =>
                (await import('./pages/NotFoundErrorPage')).default,
        },
    },
]);
