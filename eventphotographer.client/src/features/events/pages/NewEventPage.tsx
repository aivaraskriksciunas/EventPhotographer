import AjaxForm from '@/components/forms/AjaxForm';
import TextField from '@/components/forms/TextField';
import SubmitField from '@/components/forms/SubmitField';
import * as Yup from 'yup';
import { useApiFetch } from '@/api/client';
import { eventsApi } from '@/api/events';
import ChoiceField from '@/components/forms/ChoiceField';
import { useTranslation } from 'react-i18next';

export default function NewEventPage() {
    const { t } = useTranslation();

    const [durations, isDurationsLoading] = useApiFetch(
        eventsApi.getEventDurationOptions,
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
            .oneOf(durations ?? [], 'Invalid duration')
            .required('Duration is required'),
    });

    return (
        <div className="card">
            <div className="card-body">
                <h1>{t('New event')}</h1>
                <AjaxForm
                    handler={eventsApi.createEvent}
                    initialValues={intialValues}
                    validationSchema={validationSchema}
                >
                    <TextField name="name" type="text">
                        {t('Title')}
                    </TextField>
                    <ChoiceField
                        name="eventDuration"
                        disabled={isDurationsLoading}
                        choices={
                            durations?.map((v) => ({ value: v, label: v })) ??
                            []
                        }
                    >
                        {t('Duration')}
                    </ChoiceField>
                    <SubmitField>{t('Create event')}</SubmitField>
                </AjaxForm>
            </div>
        </div>
    );
}
