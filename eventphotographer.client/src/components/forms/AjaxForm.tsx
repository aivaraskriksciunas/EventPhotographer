import { Formik, Form, FormikHelpers } from 'formik';
import type * as Yup from 'yup';

interface ApiFormProps<T> {
    handler: (data?: any) => Promise<T>;
    initialValues?: Record<string, any>;
    validationSchema?: Yup.AnyObject;
    onSuccess?: (response: T) => void;
    onError?: (error: any) => void;
    children: React.ReactNode;
}

export default function AjaxForm<T>({
    handler,
    initialValues = {},
    validationSchema,
    onSuccess,
    onError,
    children,
}: ApiFormProps<T>) {
    const handleSubmit = async (
        values: Record<string, any>,
        { setSubmitting }: FormikHelpers<Record<string, any>>,
    ) => {
        let res = null;
        try {
            res = await handler(values);
        } catch (error) {
            if (onError) {
                onError(error);
            }
            return;
        }

        if (onSuccess) {
            onSuccess(res);
        }
        setSubmitting(false);
    };

    return (
        <Formik
            initialValues={initialValues}
            validationSchema={validationSchema}
            onSubmit={handleSubmit}
        >
            <Form>{children}</Form>
        </Formik>
    );
}
