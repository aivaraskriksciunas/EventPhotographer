import { useEffect } from "react";
import { useAuth } from "./state/auth"
import { useParticipant } from "./state/participant";
import { eventsApi } from "./api/events";
import { authApi } from "./api/auth";

export default function ServicesWrapper({ children }: {children: React.ReactNode}) {
    const { setUser, logout } = useAuth();
    const { setParticipant } = useParticipant()
    
    const getCurrentUser = async () => {
        try {
            let currentUser = await authApi.getCurrentUser()
            setUser(currentUser)
        }
        catch { 
            logout()
        }
    }

    const getCurrentParticipation = async () => {
        try {
            let currentParticipant = await eventsApi.getCurrentEvent();
            setParticipant(currentParticipant)
        } catch {
            setParticipant(null)
        }
    }

    useEffect(() => {getCurrentUser()}, []);
    useEffect(() => {getCurrentParticipation()}, []);

    return (
        <>{children}</>
    )
}