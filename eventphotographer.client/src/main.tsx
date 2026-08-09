import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { RouterProvider } from 'react-router-dom';
import { router } from './router.jsx';

import './scss/index.scss';
import 'bootstrap';
import './i18n.js';
import ServicesWrapper from './components/ServicesWrapper.js';
import FileUploaderWrapper from './components/FileUploaderWrapper.js';

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <ServicesWrapper>
            <FileUploaderWrapper>
                <RouterProvider router={router} />
            </FileUploaderWrapper>
        </ServicesWrapper>
    </StrictMode>,
);
