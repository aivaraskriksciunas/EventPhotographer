import { Link } from 'react-router-dom';
import { AuthenticatedUser, useAuth } from '@/state/auth';

export default function Navbar() {
    const user = useAuth((state) => state.user);

    return (
        <nav className="navbar navbar-expand navbar-dark bg-dark px-3">
            <Link to="/" className="navbar-brand">
                Event Photographer
            </Link>
            <div className="collapse navbar-collapse">
                <div className="navbar-nav">
                    <Link to="/events" className="nav-link">
                        My events
                    </Link>
                    <Link to="/events/new" className="nav-link">
                        New event
                    </Link>
                </div>

                <div className="navbar-nav me-auto">
                    <Link to="/join" className="nav-link">
                        <div className="btn btn-primary">Join</div>
                    </Link>
                </div>

                <div className="navbar-nav">
                    {user ? (
                        <AccountDropdown user={user} />
                    ) : (
                        <Link to="/login" className="nav-link">
                            Login
                        </Link>
                    )}
                </div>
            </div>
        </nav>
    );
}

function AccountDropdown({ user }: { user: AuthenticatedUser }) {
    return (
        <div className="nav-item dropdown">
            <a
                className="nav-link dropdown-toggle"
                href="#"
                role="button"
                data-bs-toggle="dropdown"
                aria-expanded="false"
            >
                {user.name}
            </a>
            <ul className="dropdown-menu">
                <li>
                    <Link to="/logout" className="dropdown-item">
                        Logout
                    </Link>
                </li>
            </ul>
        </div>
    );
}
