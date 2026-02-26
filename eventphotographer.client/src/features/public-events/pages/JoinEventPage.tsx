import { eventsApi, ParticipantResponse } from "@/api/events";
import AjaxForm from "@/components/forms/AjaxForm";
import SubmitField from "@/components/forms/SubmitField";
import TextField from "@/components/forms/TextField";
import { useParticipant } from "@/state/participant";
import { useNavigate } from "react-router-dom";
import * as Yup from 'yup';

export default function JoinEventPage() {
    
    const { setParticipant: setJoinedEvent } = useParticipant();
    const navigate = useNavigate();

    const initialValues = { code: '', name: '' };
    const validationSchema = Yup.object({
        code: Yup.string()
            .required(),
        name: Yup.string()
            .min(3).max(100)
            .required(),
    })

    const joinEvent = (data: ParticipantResponse) => {
        setJoinedEvent(data);
        navigate("/events/current");
    }

    return (
        <div>
            <AjaxForm<ParticipantResponse> 
                handler={eventsApi.joinEvent} 
                initialValues={initialValues}
                validationSchema={validationSchema}
                onSuccess={joinEvent}>
                <TextField name='code'>Code:</TextField>
                <TextField name='name'>Name: </TextField>
                <SubmitField>Save</SubmitField>
            </AjaxForm>
        </div>
    )
}