import { JSX, useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { PaginationContext } from './PaginationContext';
import Button from '../ui/Button';

interface LoadMorePaginationParams {
    children?: JSX.Element[];
}

export function LoadMorePaginator({ children }: LoadMorePaginationParams) {
    const { t } = useTranslation();
    const context = useContext(PaginationContext);
    if (null === context) {
        // Element used outside of the pagination context
        return null;
    }

    if (!context.hasNextPage) {
        return null;
    }

    return (
        <span onClick={() => context.loadMore()}>
            {children ?? (
                <Button loading={context.isLoading}>{t('Load more')}</Button>
            )}
        </span>
    );
}
