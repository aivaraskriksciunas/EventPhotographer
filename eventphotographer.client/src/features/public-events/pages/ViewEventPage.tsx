import { useJoinedEvent } from "@/state/joinedEvent"

export default function ViewEventPage() {
    const { event } = useJoinedEvent();

    return (
        <>
            <h1>{event!.name}</h1>
        </>
    )
}