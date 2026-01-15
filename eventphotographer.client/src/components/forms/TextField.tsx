import { ErrorMessage, useField, useFormikContext } from 'formik';
import { clsx } from 'clsx';

interface TextFieldProps {
    name: string;
    label: string;
    type: 'text' | 'password' | 'email' | 'number';
    placeholder?: string;
    disabled?: boolean;
}

export default function TextField({
    name,
    label = '',
    type = 'text',
    placeholder,
    disabled = false,
}: TextFieldProps) {
    const [field, meta] = useField(name);
    const { isSubmitting } = useFormikContext();
    const id = `textfield-${name}`;
    const hasError = meta.touched && meta.error;

    return (
        <div className="form-group mb-3">
            <label htmlFor={id}>{label}</label>
            <input
                {...field}
                type={type}
                id={id}
                placeholder={placeholder}
                disabled={disabled || isSubmitting}
                className={clsx('form-control', { 'is-invalid': hasError })}
            ></input>
            <ErrorMessage name={name}>
                {(msg) => <div className="invalid-feedback">{msg}</div>}
            </ErrorMessage>
        </div>
    );
}
