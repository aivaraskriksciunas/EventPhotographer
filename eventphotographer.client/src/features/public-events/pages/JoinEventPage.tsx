import { eventsApi, ParticipantResponse } from '@/api/events';
import AjaxForm from '@/components/forms/AjaxForm';
import SubmitField from '@/components/forms/SubmitField';
import TextField from '@/components/forms/TextField';
import { useAuth } from '@/state/auth';
import { useParticipant } from '@/state/participant';
import { useTranslation } from 'react-i18next';
import { useLoaderData, useNavigate } from 'react-router-dom';
import { WhatsAppLinkButton } from '@/components/base/WhatsAppLinkButton';
import * as Yup from 'yup';

export default function JoinEventPage() {
    const { t } = useTranslation();
    const { setParticipant: setJoinedEvent } = useParticipant();
    const { user } = useAuth();
    const link = useLoaderData();
    const navigate = useNavigate();
    console.log(link)

    const initialValues = { code: link?.code || '', name: user?.name ?? '' };
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
                        {link?.event ? <h2>{t('Join')} '{link.event.name}'</h2> : <h2>{t('Join event')}</h2>}
                        <AjaxForm<ParticipantResponse>
                            handler={eventsApi.joinEvent}
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onSuccess={joinEvent}
                        >
                            <TextField name="code" disabled={link != null}>{t('Code')}:</TextField>
                            <TextField name="name">{t('Name')}:</TextField>
                            <div className='d-flex gap-2'>
                                <SubmitField>{t('Join')}</SubmitField>
                                {link != null ? <WhatsAppLinkButton shareableLink={link}/> : <></>}
                            </div>
                        </AjaxForm>
                    </div>
                </div>
            </div>
        </div>
    );
}
