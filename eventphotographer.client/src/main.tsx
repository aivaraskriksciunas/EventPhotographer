import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { RouterProvider } from 'react-router-dom';
import { router } from './router.jsx';

import './scss/index.scss';
import 'bootstrap';
import ServicesWrapper from './ServicesWrapper.js';

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <ServicesWrapper>
            <RouterProvider router={router} />
        </ServicesWrapper>
    </StrictMode>,
);
