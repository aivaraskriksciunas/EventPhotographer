import Footer from '@/components/base/Footer';
import Navbar from '@/components/base/Navbar';
import { Outlet } from 'react-router-dom';

export default function AuthLayout() {
    return (
        <div id='page-layout'>
            <Navbar />

            <main id='main-content' className="container mt-4">
                <div className="row justify-content-center">
                    <div className="col-lg-4 col-md-6 col-12">
                        <Outlet />
                    </div>
                </div>
            </main>

            <Footer />
        </div>
    );
}
