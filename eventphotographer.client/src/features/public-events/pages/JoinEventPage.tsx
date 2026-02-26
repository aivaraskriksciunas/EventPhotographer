import { eventsApi, JoinEventResponse } from "@/api/events";
import AjaxForm from "@/components/forms/AjaxForm";
import SubmitField from "@/components/forms/SubmitField";
import TextField from "@/components/forms/TextField";
import { useJoinedEvent } from "@/state/joinedEvent";
import { useNavigate } from "react-router-dom";
import * as Yup from 'yup';

export default function JoinEventPage() {
    
    const { setJoinedEvent } = useJoinedEvent();
    const navigate = useNavigate();

    const initialValues = { code: '' };
    const validationSchema = Yup.object({
        code: Yup.string()
            .required()
    })

    const joinEvent = (data: JoinEventResponse) => {
        setJoinedEvent(data);
        navigate("/events/current");
    }

    return (
        <div>
            <AjaxForm<JoinEventResponse> 
                handler={eventsApi.joinEvent} 
                initialValues={initialValues}
                validationSchema={validationSchema}
                onSuccess={joinEvent}>
                <TextField name='code'>Code:</TextField>
                <SubmitField>Save</SubmitField>
            </AjaxForm>
        </div>
    )
}