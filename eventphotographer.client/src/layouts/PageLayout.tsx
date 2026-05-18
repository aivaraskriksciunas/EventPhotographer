import Navbar from '@/components/base/Navbar';
import { Outlet } from 'react-router-dom';
import Footer from '@/components/base/Footer';

export default function PageLayout() {
    return (
        <div id='page-layout'>
            <Navbar />

            <main id='main-content' className="container mt-4">
                <Outlet />
            </main>

            <Footer />
        </div>
    );
}
