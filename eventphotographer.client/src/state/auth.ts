import { DateTime } from 'luxon';
import { create } from 'zustand';

export interface AuthenticatedUser {
    id: string;
    name: string;
    email: string;
    emailConfirmed: boolean;
}

export interface AuthState {
    user: null | AuthenticatedUser;
    accountVerificationResendState: AccountVerificationResendState;
    nextVerificationResend: null | DateTime;
    setAccountVerificationResendState: (
        state: AccountVerificationResendState,
    ) => void;
    setNextVerificationResend: (date: DateTime) => void;
    setUser: (user: AuthenticatedUser) => void;
    setUserEmailConfirmed: (verified: boolean) => void;
    logout: () => void;
}

export type AccountVerificationResendState =
    | 'ready'
    | 'loading'
    | 'sent'
    | 'error';

export const useAuth = create<AuthState>((set) => ({
    user: null,
    accountVerificationResendState: 'ready',
    nextVerificationResend: null,
    setUser: (user) => set({ user }),
    setAccountVerificationResendState: (state) =>
        set({ accountVerificationResendState: state }),
    setNextVerificationResend: (date) => set({ nextVerificationResend: date }),
    setUserEmailConfirmed: (verified) => {
        set((state) => {
            if (state.user === null) {
                return state;
            }

            return {
                user: { ...state.user, emailConfirmed: verified },
            };
        });
    },
    logout: () => set({ user: null }),
}));
