import * as Yup from 'yup';
import { authApi } from '@/api/auth';
import AjaxForm from '@/components/forms/AjaxForm';
import TextField from '@/components/forms/TextField';
import SubmitField from '@/components/forms/SubmitField';

export default function RegisterPage() {
    const intialValues = {
        name: '',
        email: '',
        password: '',
        passwordConfirmation: '',
    };

    const schema = Yup.object({
        name: Yup.string().required().min(3).max(100),
        email: Yup.string().required().email(),
        password: Yup.string().required().min(8),
        passwordConfirmation: Yup.string()
            .required()
            .oneOf([Yup.ref('password')], 'Passwords must match'),
    });

    return (
        <>
            <AjaxForm
                handler={authApi.register}
                initialValues={intialValues}
                validationSchema={schema}
            >
                <TextField name="name" type="text">
                    Name
                </TextField>
                <TextField name="email" type="email">
                    Email
                </TextField>
                <TextField name="password" type="password">
                    Password
                </TextField>
                <TextField name="passwordConfirmation" type="password">
                    Repeat password
                </TextField>
                <SubmitField>Save</SubmitField>
            </AjaxForm>
        </>
    );
}
