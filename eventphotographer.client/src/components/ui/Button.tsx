import clsx from 'clsx';
import { JSX } from 'react';
import { InlineSpinner } from './Spinner';

interface ButtonProps {
    children: JSX.Element | string | string[];
    loading?: boolean;
    disabled?: boolean;
    style?: 'primary' | 'secondary' | 'danger';
    onClick?: () => void;
}

export default function Button({
    style = 'primary',
    loading = false,
    disabled = false,
    onClick = () => {},
    children,
}: ButtonProps) {
    return (
        <div
            className={clsx('btn', `btn-${style}`, {
                disabled: loading || disabled,
            })}
            onClick={onClick}
        >
            {loading ? (
                <span className="me-1">
                    <InlineSpinner />
                </span>
            ) : null}
            {children}
        </div>
    );
}
