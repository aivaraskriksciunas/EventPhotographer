import { Link, Outlet } from 'react-router-dom';

export default function MainLayout() {
    return (
        <div>
            <nav className="navbar navbar-expand navbar-dark bg-dark px-3">
                <Link to="/" className="navbar-brand">
                    MyApp
                </Link>
                <div className="navbar-nav">
                    <Link to="/events/new" className="nav-link">
                        New event
                    </Link>
                </div>
            </nav>

            <main className="container mt-4">
                <Outlet />
            </main>
        </div>
    );
}
