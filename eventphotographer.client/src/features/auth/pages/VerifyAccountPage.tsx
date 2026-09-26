import { authApi } from '@/api/auth';
import Button from '@/components/ui/Button';
import { Spinner } from '@/components/ui/Spinner';
import { useAuth } from '@/state/auth';
import { useAccountVerification } from '@/utils/hooks';
import { CheckCircle, OctagonX } from 'lucide-react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useParams } from 'react-router-dom';

export default function VerifyAccountPage() {
    const { t } = useTranslation();
    const { token } = useParams<{ token: string }>();
    const setUserEmailConfirmed = useAuth((s) => s.setUserEmailConfirmed);
    const isConfirmed = useAuth((s) => s.user?.emailConfirmed);
    const [hasError, setHasError] = useState(false);
    const redirect = useNavigate();

    useEffect(() => {
        const verify = async () => {
            try {
                await authApi.verify(token ?? '');
                setUserEmailConfirmed(true);
            } catch {
                setHasError(true);
            }
        };

        verify();
    }, [token, redirect, setUserEmailConfirmed, setHasError]);

    if (hasError) {
        return (
            <div className="card card-danger">
                <div className="card-body text-center">
                    <OctagonX className="text-danger fs-3 mb-2" />
                    <div className="mb-2">
                        {t('The verification link has expired or is invalid.')}
                    </div>
                    <ResendVerificationEmailButton />
                </div>
            </div>
        );
    }

    if (isConfirmed === true) {
        return (
            <div className="card text-center">
                <div className="card-body">
                    <div className="mb-2">
                        <CheckCircle /> {t('Your account is verified')}!
                    </div>
                    <Link to="/">
                        <Button>{t('Open app')}</Button>
                    </Link>
                </div>
            </div>
        );
    }

    return (
        <div className="card text-center">
            <div className="card-body">
                <div className="mb-3">
                    <Spinner />
                </div>
                <div>{t('Verifying your account')}</div>
            </div>
        </div>
    );
}

function ResendVerificationEmailButton() {
    const { status, resendVerificationEmail, nextResendDate } =
        useAccountVerification();
    const { t } = useTranslation();

    switch (status) {
        case 'unauthenticated':
            return <Link to="/login">{t('Login')}</Link>;
        case 'cooldown':
            return (
                <Button disabled>
                    {t('Resend again at')} {nextResendDate!.toFormat('t')}
                </Button>
            );
        case 'error':
            return (
                <p>
                    {t(
                        'Could not send verification email. Please refresh the page and try again.',
                    )}
                </p>
            );
        case 'sent':
            return (
                <>
                    <div className="mb-2">
                        <CheckCircle className="text-success me-1" />
                        {t('Email sent. Check your inbox!')}
                    </div>
                    <Button disabled>
                        {t('Resend again at')} {nextResendDate!.toFormat('t')}
                    </Button>
                </>
            );
        case 'loading':
            return <Button loading>{t('Resend verification email')}</Button>;
        case 'ready':
            return (
                <Button onClick={resendVerificationEmail}>
                    {t('Resend verification email')}
                </Button>
            );
    }
}
