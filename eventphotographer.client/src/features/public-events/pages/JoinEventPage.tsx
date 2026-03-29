import { eventsApi, ParticipantResponse } from '@/api/events';
import AjaxForm from '@/components/forms/AjaxForm';
import SubmitField from '@/components/forms/SubmitField';
import TextField from '@/components/forms/TextField';
import { useParticipant } from '@/state/participant';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import * as Yup from 'yup';

export default function JoinEventPage() {
    const { t } = useTranslation();
    const { setParticipant: setJoinedEvent } = useParticipant();
    const navigate = useNavigate();

    const initialValues = { code: '', name: '' };
    const validationSchema = Yup.object({
        code: Yup.string().required(),
        name: Yup.string().min(3).max(100).required(),
    });

    const joinEvent = (data: ParticipantResponse) => {
        setJoinedEvent(data);
        navigate('/events/current');
    };

    return (
        <div className="row justify-content-center">
            <div className="col-lg-8 col-md-10">
                <div className="card">
                    <div className="card-body">
                        <h2>{t('Join event')}</h2>
                        <AjaxForm<ParticipantResponse>
                            handler={eventsApi.joinEvent}
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onSuccess={joinEvent}
                        >
                            <TextField name="code">{t('Code')}:</TextField>
                            <TextField name="name">{t('Name')}:</TextField>
                            <SubmitField>{t('Join')}</SubmitField>
                        </AjaxForm>
                    </div>
                </div>
            </div>
        </div>
    );
}
