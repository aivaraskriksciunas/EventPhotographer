import {
    createBrowserRouter,
    MiddlewareFunction,
    Navigate,
    redirect,
} from 'react-router-dom';
import { eventsRoutes } from './features/events/events.routes';
import AuthLayout from './layouts/AuthLayout';
import { authRoutes } from './features/auth/auth.routes';
import { authApi } from './api/auth';
import { useAuth } from './state/auth';
import { publicEventsRoutes } from './features/public-events/public-events.routes';
import PageLayout from './layouts/PageLayout';

const authMiddleware: MiddlewareFunction = async (_, next) => {
    let currentUser = useAuth.getState().user;
    const setCurrentUser = useAuth.getState().setUser;

    if (null == currentUser) {
        try {
            currentUser = await authApi.getCurrentUser();
        } catch {
            // Ignore error for now
        }
    }

    if (null == currentUser) {
        throw redirect('/login');
    }

    setCurrentUser(currentUser);
    await next();
};

export const router = createBrowserRouter([
    {
        element: <PageLayout />,
        errorElement: <Navigate to="/404" replace />,
        children: [
            ...publicEventsRoutes,
            {
                children: [...eventsRoutes],
                middleware: [authMiddleware],
            },
        ],
    },
    {
        element: <AuthLayout />,
        errorElement: <Navigate to="/404" replace />,
        children: [{ path: '/' }, ...authRoutes],
    },
    {
        path: '/404',
        lazy: {
            Component: async () =>
                (await import('./pages/NotFoundErrorPage')).default,
        },
    },
]);
