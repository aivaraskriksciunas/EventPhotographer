import { ErrorMessage, useField, useFormikContext } from 'formik';
import { clsx } from 'clsx';

interface CheckboxFieldProps {
    name: string;
    disabled?: boolean;
    children?: React.ReactNode;
}

export default function CheckboxField({
    name,
    disabled = false,
    children,
}: CheckboxFieldProps) {
    const [field, meta] = useField(name);
    const { isSubmitting } = useFormikContext();
    const id = `checkboxfield-${name}`;
    const hasError = meta.touched && meta.error;

    return (
        <div className="form-check mb-3">
            <input
                {...field}
                id={id}
                type="checkbox"
                disabled={disabled || isSubmitting}
                className={clsx('form-check-input', { 'is-invalid': hasError })}
            />
            <label htmlFor={id} className="form-check-label">
                {children}
            </label>
            <ErrorMessage name={name}>
                {(msg) => <div className="invalid-feedback">{msg}</div>}
            </ErrorMessage>
        </div>
    );
}
