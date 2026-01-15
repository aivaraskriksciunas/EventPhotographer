import { Formik, Form, FormikHelpers } from 'formik';
import { api } from '@/api/client';
import type * as Yup from 'yup';

interface ApiFormProps {
    url: string;
    method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';
    initialValues?: Record<string, any>;
    validationSchema?: Yup.AnyObject;
    onSuccess?: (response: any) => void;
    onError?: (error: any) => void;
    children: React.ReactNode;
}

export default function AjaxForm({
    url,
    method = 'POST',
    initialValues = {},
    validationSchema,
    onSuccess,
    onError,
    children,
}: ApiFormProps) {
    const handleSubmit = async (
        values: Record<string, any>,
        { setSubmitting }: FormikHelpers<Record<string, any>>,
    ) => {
        let res = null;
        try {
            res = await api.request({
                method: method,
                url: url,
                data: values,
            });
        } catch (error) {
            if (onError) {
                onError(error);
            }
            return;
        }

        if (onSuccess) {
            onSuccess(res.data);
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
