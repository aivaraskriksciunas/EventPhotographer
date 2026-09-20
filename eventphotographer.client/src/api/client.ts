import axios, { AxiosRequestConfig } from 'axios';
import { useState, useEffect } from 'react';

type RequestMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

export const api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true,
});

export interface PagedResponse<T> {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
}

export interface PaginationQueryParameters {
    page: number;
    pageSize: number;
}

export async function fetchApi<T>(
    url: string,
    method: RequestMethod = 'GET',
    data?: any,
    requestParams: AxiosRequestConfig = {},
): Promise<T> {
    const response = await api.request<T>({
        url,
        method,
        data,
        ...requestParams,
    });

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

export function makePaginatedHandler<T>(
    fetcher: (...params: any[]) => Promise<PagedResponse<T>>,
    ...params: any[]
): (pagination: PaginationQueryParameters) => Promise<PagedResponse<T>> {
    return (pagination) => fetcher(...params, pagination);
}
