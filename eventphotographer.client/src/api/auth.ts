import { fetchApi } from './client';

export interface LoginRequest {
    email: string;
    password: string;
    rememberMe: string;
}

export interface RegisterRequest {
    name: string;
    email: string;
    password: string;
}

export interface CurrentUserResponse {
    id: string;
    name: string;
    email: string;
}

export const authApi = {
    getCurrentUser: async () =>
        fetchApi<CurrentUserResponse>('/api/auth/user', 'GET'),
    login: async (loginRequest: LoginRequest) =>
        fetchApi<CurrentUserResponse>('/api/auth/login', 'POST', loginRequest),
    register: async (registerRequest: RegisterRequest) =>
        fetchApi<CurrentUserResponse>('/api/auth/register', 'POST', registerRequest),
    logout: async () => fetchApi('/api/auth/logout'),
};
