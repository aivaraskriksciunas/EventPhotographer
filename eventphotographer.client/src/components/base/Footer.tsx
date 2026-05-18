import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

export default function Footer() {
    const year = new Date().getFullYear();
    const { t } = useTranslation();

    return (
        <footer>
            <div className='container text-muted'>
                <div className='d-flex justify-content-between py-3 my-2'>
                    <div className='col-md-4 d-flex align-items-center'>
                        © {year} Event Photographer
                    </div>
                    <div className='col-md-4 d-flex justify-content-end align-items-center'>
                        <Link to='/privacy-policy'>{t('Privacy policy')}</Link>
                    </div>
                </div>
            </div>
        </footer>
    )
}