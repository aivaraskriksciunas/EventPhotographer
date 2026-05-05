import { ErrorMessage, useField, useFormikContext } from 'formik';
import { clsx } from 'clsx';
import DatePicker from 'react-datepicker';

interface DateFieldProps {
    name: string;
    placeholder?: string;
    disabled?: boolean;
    children?: React.ReactNode;
    showTimeSelect?: boolean;
    timeIntervals?: number;
}

export default function DateField({
    name,
    placeholder,
    disabled = false,
    showTimeSelect = false,
    timeIntervals = 15,
    children,
}: DateFieldProps) {
    const [field, meta, helpers] = useField(name);
    const { isSubmitting } = useFormikContext();
    const id = `datefield-${name}`;
    const hasError = meta.touched && meta.error;
    
    let dateFormat = 'MMMM d, yyyy';
    if (showTimeSelect) {
        dateFormat += ' h:mm aa';
    }

    return (
        <div className="form-group mb-3 has-validation">
            <label htmlFor={id}>{children}</label>
            <div className={clsx({ 'is-invalid': hasError })}>
                <DatePicker 
                    id={id}
                    placeholderText={placeholder}
                    disabled={disabled || isSubmitting}
                    className={clsx('form-control', { 'is-invalid': hasError })}
                    selected={field.value ? new Date(field.value) : null}
                    onChange={(date: Date|null) => helpers.setValue(date)}
                    onBlur={field.onBlur}
                    showTimeSelect={showTimeSelect}
                    timeIntervals={timeIntervals}
                    dateFormat={dateFormat}
                />
            </div>
            <ErrorMessage name={name}>
                {(msg) => <div className="invalid-feedback">{msg}</div>}
            </ErrorMessage>
        </div>
    );
}
