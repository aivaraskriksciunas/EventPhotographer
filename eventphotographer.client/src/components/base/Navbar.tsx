import { Link } from 'react-router-dom';
import { AuthenticatedUser, useAuth } from '@/state/auth';
import { useParticipant } from '@/state/participant';
import { EventResponse, eventsApi } from '@/api/events';
import { truncate } from '@/utils/helpers';
import { X } from 'lucide-react';
import { useTranslation } from 'react-i18next';

export default function Navbar() {
    const { t } = useTranslation();
    const user = useAuth((state) => state.user);
    const participant = useParticipant((state) => state.participant);

    return (
        <nav className="navbar navbar-expand px-3">
            <Link to="/" className="navbar-brand">
                LiveAlbum
            </Link>
            <div className="collapse navbar-collapse">
                <div className="navbar-nav">
                    {user ? <AuthenticatedUserMenu /> : null}
                </div>

                <div className="navbar-nav me-auto">
                    {participant !== null ? (
                        <CurrentEventIndicator event={participant.event} />
                ) : (
                        <JoinEventButton />
                    )}
                </div>

                <div className="navbar-nav">
                    {user ? (
                        <AccountDropdown user={user} />
                    ) : (
                        <Link to="/login" className="nav-link">
                            {t('Login')}
                        </Link>
                    )}
                </div>
            </div>
        </nav>
    );
}

function AuthenticatedUserMenu() {
    const { t } = useTranslation();

    return (
        <>
            <Link to="/events" className="nav-link">
                {t('My events')}
            </Link>
            <Link to="/events/new" className="nav-link">
                {t('New event')}
            </Link>
        </>
    );
}

function AccountDropdown({ user }: { user: AuthenticatedUser }) {
    const { t } = useTranslation();
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
                        {t('Logout')}
                    </Link>
                </li>
            </ul>
        </div>
    );
}

function JoinEventButton() {
    const { t } = useTranslation();

    return (
        <Link to="/join" className="nav-link">
            <div className="btn btn-primary">{t('Join')}</div>
        </Link>
    );
}

function CurrentEventIndicator({ event }: { event: EventResponse }) {
    const { stopParticipation } = useParticipant();

    const leaveEvent = () => {
        stopParticipation();
        eventsApi.leaveCurrentEvent();
    };

    return (
        <Link to="/events/current">
            <span className="badge text-bg-secondary">
                {truncate(event.name, 15)}
                <span onClick={leaveEvent}>
                    <X size="16" />
                </span>
            </span>
        </Link>
    );
}
