import AjaxForm from '@/components/forms/AjaxForm';
import TextField from '@/components/forms/TextField';
import SubmitField from '@/components/forms/SubmitField';
import * as Yup from 'yup';

export default function NewEventPage() {
    const intialValues = { name: '' };
    const validationSchema = Yup.object({
        name: Yup.string()
            .min(3, 'Title must be at least 3 characters')
            .max(100, 'Title must be at most 100 characters')
            .required('Title is required'),
    });

    return (
        <>
            <h1>New event</h1>
            <AjaxForm
                url="/api/events"
                method="POST"
                initialValues={intialValues}
                validationSchema={validationSchema}
            >
                <TextField name="name" label="Title" type="text"></TextField>
                <SubmitField>Create Event</SubmitField>
            </AjaxForm>
        </>
    );
}
