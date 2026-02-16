import { redirect, RouteObject } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import { authApi } from '@/api/auth';
import { useAuth } from '@/state/auth';

export const authRoutes: RouteObject[] = [
    {
        path: '/login',
        element: <LoginPage />,
    },
    {
        path: '/register',
        lazy: {
            Component: async () =>
                (await import('./pages/RegisterPage')).default,
        },
    },
    {
        path: '/logout',
        loader: async () => {
            try {
                await authApi.logout();
            } catch (e) {
                console.log(e); // TODO: Error handling
            }

            useAuth.getState().logout();
            throw redirect('/login');
        },
    },
];
