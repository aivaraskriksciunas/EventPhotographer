import { useRouteLoaderData } from "react-router-dom";

export default function ShareEventPage() {
    const event = useRouteLoaderData('view-event');

    return (
        <>
            <h2>Share Event: {event.name}</h2>
            <div>
                
            </div>
        </>
    )
}