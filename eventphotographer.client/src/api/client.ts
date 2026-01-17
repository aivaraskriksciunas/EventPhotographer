import axios from 'axios';
import { useState, useEffect } from 'react';

type RequestMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

export const api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

export async function fetchApi<T>(
    url: string,
    method: RequestMethod = 'GET',
): Promise<T> {
    const response = await api.request<T>({ url, method });

    return response.data;
}

export function useApiFetch<T>(fetcher: () => Promise<T>): [null | T, boolean] {
    const [data, setData] = useState<null | T>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function _fetchData() {
            const result = await fetcher();
            setData(result);
            setLoading(false);
        }

        _fetchData();
    }, [fetcher]);

    return [data, loading];
}
