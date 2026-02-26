import { create } from "zustand";
import { ParticipantResponse } from "@/api/events";

export interface ParticipantState {
    participant: null | ParticipantResponse,
    setParticipant: (event: null | ParticipantResponse) => void,
    stopParticipation: () => void,
}

export const useParticipant = create<ParticipantState>(set => ({
    participant: null,
    setParticipant: (event: null | ParticipantResponse) => set({ participant: event }),
    stopParticipation: () => set({ participant: null })
}))