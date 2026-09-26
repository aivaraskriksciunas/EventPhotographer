import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { useAccountVerification, useIsAuthenticated } from '@/utils/hooks';
import { useAuth } from '@/state/auth';
import { CheckCircle, OctagonX } from 'lucide-react';
import { InlineSpinner } from '../ui/Spinner';

export default function Footer() {
    const year = new Date().getFullYear();
    const { t } = useTranslation();

    return (
        <>
            <UserEmailNotConfirmedWarning />
            <footer>
                <div className="container text-muted">
                    <div className="d-flex justify-content-between py-3 my-2">
                        <div className="col-md-4 d-flex align-items-center">
                            © {year} Event Photographer
                        </div>
                        <div className="col-md-4 d-flex justify-content-end align-items-center">
                            <Link to="/privacy-policy">
                                {t('Privacy policy')}
                            </Link>
                        </div>
                    </div>
                </div>
            </footer>
        </>
    );
}

function UserEmailNotConfirmedWarning() {
    const isAuthenticated = useIsAuthenticated();
    const confirmed = useAuth((state) => state.user?.emailConfirmed);
    const { status, nextResendDate, resendVerificationEmail } =
        useAccountVerification();
    const { t } = useTranslation();

    if (confirmed === true || !isAuthenticated) {
        return null;
    }

    const getContent = () => {
        switch (status) {
            case 'ready':
                return (
                    <>
                        {t(
                            'Your account is not verified. Please check your email for verification link.',
                        )}{' '}
                        <span
                            className="link"
                            onClick={resendVerificationEmail}
                        >
                            {t('Resend')}
                        </span>
                    </>
                );
            case 'error':
                return (
                    <>
                        <OctagonX /> {t('An error ocurred')}.{' '}
                        {t('Please try again lager')}
                    </>
                );
            case 'loading':
                return (
                    <>
                        <InlineSpinner /> {t('Loading')}
                    </>
                );
            case 'sent':
                return (
                    <>
                        <CheckCircle /> {t('Email sent. Check your inbox!')}
                    </>
                );
            case 'cooldown':
                return (
                    <>
                        {t(
                            'Your account is not verified. Please check your email for verification link.',
                        )}{' '}
                        {t('Resend again at')} {nextResendDate!.toFormat('t')}
                    </>
                );
            case 'unauthenticated':
                return (
                    <>
                        {t(
                            'Your account is not verified. Please check your email for verification link.',
                        )}
                    </>
                );
        }
    };

    return (
        <div className="userEmailNotConfirmedWarning bg-danger">
            {getContent()}
        </div>
    );
}
