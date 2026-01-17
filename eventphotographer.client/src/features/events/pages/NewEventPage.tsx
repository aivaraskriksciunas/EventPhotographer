import AjaxForm from '@/components/forms/AjaxForm';
import TextField from '@/components/forms/TextField';
import SubmitField from '@/components/forms/SubmitField';
import * as Yup from 'yup';
import { useApiFetch } from '@/api/client';
import { getEventDurationOptions } from '@/api/events';
import ChoiceField from '@/components/forms/ChoiceField';

export default function NewEventPage() {
    const [durations, isDurationsLoading] = useApiFetch(
        getEventDurationOptions,
    );

    const intialValues = { name: '', startDate: null, eventDuration: null };
    const validationSchema = Yup.object({
        name: Yup.string()
            .min(3, 'Title must be at least 3 characters')
            .max(100, 'Title must be at most 100 characters')
            .required('Title is required'),
        startDate: Yup.date()
            .min(new Date(), 'Start date must be in the future')
            .nullable(),
        eventDuration: Yup.string()
            .oneOf([], 'Invalid duration')
            .required('Duration is required'),
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
                <TextField name="Name" label="Title" type="text"></TextField>
                <ChoiceField
                    name="EventDuration"
                    label="Duration"
                    disabled={isDurationsLoading}
                    choices={
                        durations?.map((v) => ({ value: v, label: v })) ?? []
                    }
                ></ChoiceField>
                <SubmitField>Create Event</SubmitField>
            </AjaxForm>
        </>
    );
}
