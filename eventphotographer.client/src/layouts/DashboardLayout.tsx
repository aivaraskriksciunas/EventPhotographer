import QuickLinks from '@/components/base/QuickLinks';
import { Outlet } from 'react-router-dom';

export default function DashboardLayout() {
    return (
        <main className="row">
            <div className="col-md-2">
                <QuickLinks />
            </div>
            <div className="col-md-10">
                <Outlet />
            </div>
        </main>
    );
}
