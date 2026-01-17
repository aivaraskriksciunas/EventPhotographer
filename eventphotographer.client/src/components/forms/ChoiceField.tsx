import { ErrorMessage, useField, useFormikContext } from 'formik';
import { clsx } from 'clsx';

interface ChoiceFieldProps {
    name: string;
    label: string;
    choices: Array<{ value: string; label: string }>;
    disabled?: boolean;
}

export default function ChoiceField({
    name,
    label = '',
    choices,
    disabled = false,
}: ChoiceFieldProps) {
    const [field, meta] = useField(name);
    const { isSubmitting } = useFormikContext();
    const id = `choicefield-${name}`;
    const hasError = meta.touched && meta.error;

    return (
        <div className="form-group mb-3">
            <label htmlFor={id}>{label}</label>
            <select
                {...field}
                id={id}
                disabled={disabled || isSubmitting}
                className={clsx('form-control', { 'is-invalid': hasError })}
            >
                {choices.map((i) => (
                    <option value={i.value}>{i.label}</option>
                ))}
            </select>
            <ErrorMessage name={name}>
                {(msg) => <div className="invalid-feedback">{msg}</div>}
            </ErrorMessage>
        </div>
    );
}
