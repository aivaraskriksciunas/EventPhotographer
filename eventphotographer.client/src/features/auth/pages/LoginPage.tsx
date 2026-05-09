import AjaxForm from '@/components/forms/AjaxForm';
import * as Yup from 'yup';
import { authApi, CurrentUserResponse } from '@/api/auth';
import TextField from '@/components/forms/TextField';
import SubmitField from '@/components/forms/SubmitField';
import CheckboxField from '@/components/forms/CheckboxField';
import { useAuth } from '@/state/auth';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export default function LoginPage() {
    const { t } = useTranslation();

    const intialValues = { email: '', password: '', rememberMe: false };
    const schema = Yup.object({
        email: Yup.string().required().email(),
        password: Yup.string().required(),
        rememberMe: Yup.boolean(),
    });
    const setUser = useAuth((state) => state.setUser);
    const redirect = useNavigate();

    const onLogin = (response: CurrentUserResponse) => {
        setUser(response);
        redirect('/');
    };

    return (
        <div className="card">
            <div className="card-body">
                <AjaxForm
                    handler={authApi.login}
                    initialValues={intialValues}
                    validationSchema={schema}
                    onSuccess={onLogin}
                >
                    <TextField name="email" type="email">
                        {t('Email')}
                    </TextField>
                    <TextField name="password" type="password">
                        {t('Password')}
                    </TextField>
                    <CheckboxField name="rememberMe">
                        {t('Remember me?')}
                    </CheckboxField>
                    <SubmitField>{t('Save')}</SubmitField>
                </AjaxForm>
                <small>{t('Don\'t have an account yet?')} <a href="/register">{t('Register')}</a></small>
            </div>
        </div>
    );
}
