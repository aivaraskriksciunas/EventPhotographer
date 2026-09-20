import { createContext } from 'react';

export interface PaginationContextParams {
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
    isLoading: boolean;
    goToPage: (page: number) => void;
    loadMore: () => void;
}

export const PaginationContext = createContext<PaginationContextParams | null>(
    null,
);
