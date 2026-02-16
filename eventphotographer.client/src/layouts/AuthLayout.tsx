import Navbar from '@/components/base/Navbar';
import { Outlet } from 'react-router-dom';

export default function AuthLayout() {
    return (
        <div>
            <Navbar />

            <main className="container mt-4">
                <Outlet />
            </main>
        </div>
    );
}
