import { ErrorMessage, useField, useFormikContext } from 'formik';
import { clsx } from 'clsx';

interface ChoiceFieldProps {
    name: string;
    choices: Array<{ value: string; label: string }>;
    disabled?: boolean;
    children?: React.ReactNode;
}

export default function ChoiceField({
    name,
    choices,
    disabled = false,
    children,
}: ChoiceFieldProps) {
    const [field, meta] = useField(name);
    const { isSubmitting } = useFormikContext();
    const id = `choicefield-${name}`;
    const hasError = meta.touched && meta.error;

    return (
        <div className="form-group mb-3">
            <label htmlFor={id}>{children}</label>
            <select
                {...field}
                id={id}
                disabled={disabled || isSubmitting}
                className={clsx('form-control', { 'is-invalid': hasError })}
            >
                <option value=""></option>
                {choices.map((i) => (
                    <option value={i.value} key={i.value}>
                        {i.label}
                    </option>
                ))}
            </select>
            <ErrorMessage name={name}>
                {(msg) => <div className="invalid-feedback">{msg}</div>}
            </ErrorMessage>
        </div>
    );
}
