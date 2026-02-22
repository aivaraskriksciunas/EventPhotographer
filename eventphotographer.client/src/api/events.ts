import { fetchApi } from '@/api/client';

interface CreateEventRequest {
    name: string;
    startDate: Date;
    duration: string;
}

export interface EventResponse {
    id: string;
    name: string;
    startDate: Date;
    endDate: Date;
}

export const eventsApi = {
    createEvent: async (request: CreateEventRequest) =>
        fetchApi<EventResponse>('/api/events', 'POST', request),
    getAllEvents: async () => fetchApi<EventResponse[]>('/api/events'),
    getEvent: async (eventId: string) => fetchApi<EventResponse>(`/api/events/${eventId}`),
    getEventDurationOptions: async () =>
        fetchApi<string[]>('/api/events/durations'),
};
