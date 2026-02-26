import { create } from "zustand";
import { JoinEventResponse } from "@/api/events";

export interface JoinedEventState {
    event: null | JoinEventResponse,
    setJoinedEvent: (event: null | JoinEventResponse) => void,
    leaveEvent: () => void,
}

export const useJoinedEvent = create<JoinedEventState>(set => ({
    event: null,
    setJoinedEvent: (event: null | JoinEventResponse) => set({ event }),
    leaveEvent: () => set({ event: null })
}))