import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import {
    SmilePlus,
    User,
    Hourglass,
    CalendarPlus,
    CalendarDays,
} from 'lucide-react';

export default function QuickLinks() {
    const { t } = useTranslation();

    return (
        <aside className="sidebar" aria-labelledby="sidebar-title">
            <h2 id="sidebar-title">{t('Quick links')}</h2>
            <nav aria-label={t('Quick links')} className="nav flex-column">
                <Link to="/events/new" className="nav-link">
                    <CalendarPlus className="me-2" />
                    {t('New event')}
                </Link>
                <Link to="/events" className="nav-link">
                    <CalendarDays className="me-2" />
                    {t('My events')}
                </Link>
                <Link to="/coming-soon" className="nav-link">
                    <Hourglass className="me-2" />
                    {t('Past events')}
                </Link>
                <Link to="/coming-soon" className="nav-link">
                    <User className="me-2" />
                    {t('Account')}
                </Link>
                <Link to="/join" className="nav-link">
                    <SmilePlus className="me-2" />
                    {t('Join')}
                </Link>
            </nav>
        </aside>
    );
}
