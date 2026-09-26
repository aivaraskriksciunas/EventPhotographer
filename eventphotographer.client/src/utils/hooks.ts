import { authApi } from '@/api/auth';
import { useAuth } from '@/state/auth';
import { DateTime } from 'luxon';
import { useEffect, useState } from 'react';

export type AccountVerificationStatus =
    | 'unauthenticated'
    | 'cooldown'
    | 'ready'
    | 'loading'
    | 'sent'
    | 'error';

export const useAccountVerification = (): {
    status: AccountVerificationStatus;
    nextResendDate: DateTime | null;
    resendVerificationEmail: () => void;
} => {
    const user = useAuth((s) => s.user);
    const resendState = useAuth((s) => s.accountVerificationResendState);
    const nextResendDate = useAuth((s) => s.nextVerificationResend);
    const setResendState = useAuth((s) => s.setAccountVerificationResendState);
    const setNextResendDate = useAuth((s) => s.setNextVerificationResend);

    const [now, setNow] = useState(() => DateTime.now());

    useEffect(() => {
        if (nextResendDate == null || now >= nextResendDate) {
            return;
        }

        const id = setInterval(() => setNow(DateTime.now()), 1000);
        return () => clearInterval(id);
    }, [nextResendDate, now]);

    const cooldownActive = nextResendDate != null && now < nextResendDate;

    let status: AccountVerificationStatus;
    if (user == null) {
        status = 'unauthenticated';
    } else if (resendState === 'loading') {
        status = 'loading';
    } else if (resendState === 'error') {
        status = 'error';
    } else if (cooldownActive) {
        status = resendState === 'sent' ? 'sent' : 'cooldown';
    } else {
        status = 'ready';
    }

    const resendVerificationEmail = async () => {
        if (user == null) {
            return;
        }

        try {
            setResendState('loading');

            const result = await authApi.sendVerification();
            setNextResendDate(DateTime.fromISO(result.nextResendDate));
            setResendState('sent');
        } catch {
            setResendState('error');
        }
    };

    return {
        status,
        nextResendDate,
        resendVerificationEmail,
    };
};

export const useIsAuthenticated = (): boolean => {
    const user = useAuth((state) => state.user);

    return user !== null;
};
