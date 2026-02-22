import { Link, useRouteLoaderData } from "react-router-dom";

export default function ViewEventPage() {
    const event = useRouteLoaderData('view-event');
    
    return (
        <div>
            <h2>{event.name}</h2>
            <Link to="share">
                <div className="btn btn-primary">Share</div>
            </Link>
        </div>
    )
}