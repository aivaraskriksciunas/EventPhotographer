import { PagedResponse, PaginationQueryParameters } from '@/api/client';
import { JSX, startTransition, useActionState, useEffect } from 'react';
import { PaginationContext } from './PaginationContext';

interface PaginatedViewParams<T> {
    initialData?: PagedResponse<T>;
    requestHandler: (
        paginationParams: PaginationQueryParameters,
    ) => Promise<PagedResponse<T>>;
    initialPage?: number;
    pageSize?: number;
    children: (items: T[]) => JSX.Element[] | JSX.Element;
}

export function PaginatedView<T>({
    initialData,
    requestHandler,
    initialPage = 1,
    pageSize = 16,
    children,
}: PaginatedViewParams<T>) {
    const [data, loadData, isLoading] = useActionState(
        async (
            prev: PagedResponse<T> | null,
            params: { newPage: number; appendNewItems: boolean },
        ) => {
            if (prev?.page === params.newPage) {
                return prev;
            }

            const result = await requestHandler({
                page: params.newPage,
                pageSize: prev?.pageSize ?? pageSize,
            });
            if (params.appendNewItems) {
                return {
                    ...result,
                    items: [...(prev?.items ?? []), ...result.items],
                };
            }

            return result;
        },
        initialData ?? null,
    );

    useEffect(() => {
        if (initialData) return;
        startTransition(() =>
            loadData({ newPage: initialPage ?? 1, appendNewItems: false }),
        );
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const goToPage = (page: number) => {
        if (isLoading) {
            return;
        }

        startTransition(() =>
            loadData({ newPage: page, appendNewItems: false }),
        );
    };

    const loadMore = () => {
        if (isLoading || !data?.hasNextPage) {
            return;
        }

        startTransition(() =>
            loadData({ newPage: data.page + 1, appendNewItems: true }),
        );
    };

    if (data === null) {
        return <>No data</>;
    }

    return (
        <PaginationContext
            value={{
                page: data.page,
                pageSize: data.pageSize,
                totalCount: data.totalCount,
                totalPages: data.totalPages,
                hasPreviousPage: data.hasPreviousPage,
                hasNextPage: data.hasNextPage,
                isLoading,
                goToPage,
                loadMore,
            }}
        >
            {children(data.items)}
        </PaginationContext>
    );
}
