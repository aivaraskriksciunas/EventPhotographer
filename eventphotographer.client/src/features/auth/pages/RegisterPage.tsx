import * as Yup from 'yup';
import { authApi, CurrentUserResponse } from '@/api/auth';
import AjaxForm from '@/components/forms/AjaxForm';
import TextField from '@/components/forms/TextField';
import SubmitField from '@/components/forms/SubmitField';
import { useAuth } from '@/state/auth';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export default function RegisterPage() {
    const { t } = useTranslation();

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
    const setUser = useAuth((state) => state.setUser);
    const redirect = useNavigate();

    const onRegister = (response: CurrentUserResponse) => {
        setUser(response);
        redirect('/');
    };

    return (
        <>
            <AjaxForm
                handler={authApi.register}
                initialValues={intialValues}
                validationSchema={schema}
                onSuccess={onRegister}
            >
                <TextField name="name" type="text">
                    {t('Name')}
                </TextField>
                <TextField name="email" type="email">
                    {t('Email')}
                </TextField>
                <TextField name="password" type="password">
                    {t('Password')}
                </TextField>
                <TextField name="passwordConfirmation" type="password">
                    {t('Repeat password')}
                </TextField>
                <SubmitField>{t('Save')}</SubmitField>
            </AjaxForm>
        </>
    );
}
