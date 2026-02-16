import AjaxForm from '@/components/forms/AjaxForm';
import * as Yup from 'yup';
import { authApi } from '@/api/auth';
import TextField from '@/components/forms/TextField';
import SubmitField from '@/components/forms/SubmitField';
import CheckboxField from '@/components/forms/CheckboxField';

export default function LoginPage() {
    const intialValues = { email: '', password: '', rememberMe: false };
    const schema = Yup.object({
        email: Yup.string().required().email(),
        password: Yup.string().required(),
        rememberMe: Yup.boolean(),
    });

    return (
        <>
            <AjaxForm
                handler={authApi.login}
                initialValues={intialValues}
                validationSchema={schema}
            >
                <TextField name="email" type="email">
                    Email
                </TextField>
                <TextField name="password" type="password">
                    Password
                </TextField>
                <CheckboxField name="rememberMe">Remember me?</CheckboxField>
                <SubmitField>Save</SubmitField>
            </AjaxForm>
        </>
    );
}
