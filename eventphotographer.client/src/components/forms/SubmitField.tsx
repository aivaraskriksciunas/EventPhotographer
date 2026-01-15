import { useFormikContext } from 'formik';

interface SubmitFieldProps {
    children: React.ReactNode;
    disabled?: boolean;
}

export default function SubmitField({
    children,
    disabled = false,
}: SubmitFieldProps) {
    const { isSubmitting } = useFormikContext();

    return (
        <button
            type="submit"
            className="btn btn-primary"
            disabled={disabled || isSubmitting}
        >
            {children}
        </button>
    );
}
