import { fetchApi } from '@/api/client';

interface CreateEventRequest {
    name: string;
    startDate: Date;
    duration: string;
}

export const eventsApi = {
    createEvent: async (request: CreateEventRequest) =>
        fetchApi('/api/events', 'POST', request),
    getEventDurationOptions: async () =>
        fetchApi<string[]>('/api/events/durations'),
};
