import { useParticipant } from "@/state/participant"

export default function ViewEventPage() {
    const { participant } = useParticipant();

    return (
        <>
            <h1>{participant!.event.name}</h1>
        </>
    )
}