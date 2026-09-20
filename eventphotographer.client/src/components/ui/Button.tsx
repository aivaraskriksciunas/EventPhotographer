import clsx from 'clsx';
import { LoaderCircle } from 'lucide-react';
import { JSX } from 'react';

interface ButtonProps {
    children: JSX.Element | string;
    loading?: boolean;
    disabled?: boolean;
    style?: 'primary' | 'secondary' | 'danger';
}

export default function Button({
    style = 'primary',
    loading = false,
    disabled = false,
    children,
}: ButtonProps) {
    return (
        <div
            className={clsx('btn', `btn-${style}`, {
                disabled: loading || disabled,
            })}
        >
            {loading ? (
                <span className="me-1">
                    <LoaderCircle aria-label="Loading" className="spinner" />
                </span>
            ) : null}
            {children}
        </div>
    );
}
