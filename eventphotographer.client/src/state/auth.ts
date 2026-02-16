import { create } from 'zustand';

export interface AuthenticatedUser {
    id: string;
    name: string;
    email: string;
}

export interface AuthState {
    user: null | AuthenticatedUser;
    setUser: (user: AuthenticatedUser) => void;
    logout: () => void;
}

export const useAuth = create<AuthState>((set) => ({
    user: null,
    setUser: (user: AuthenticatedUser) => set({ user }),
    logout: () => set({ user: null }),
}));
