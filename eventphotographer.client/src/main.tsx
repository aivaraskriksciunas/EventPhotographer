import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { RouterProvider } from 'react-router-dom';
import './scss/index.scss';
import { router } from './router.jsx';

// Uncomment to import bootstrap js components
// import { Tooltip, Toast, Popover } from 'bootstrap';

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <RouterProvider router={router} />
    </StrictMode>,
);
