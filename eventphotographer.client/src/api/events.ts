import { fetchApi } from '@/api/client';

interface CreateEventRequest {
    name: string;
    startDate: Date;
    duration: string;
}

export interface JoinEventRequest {
    code: string;
}

export interface EventResponse {
    id: string;
    name: string;
    startDate: Date;
    endDate: Date;
}

export interface EventShareableLinkResponse {
    id: string;
    code: string;
}

export interface JoinEventResponse {
    id: string;
    name: string;
    startDate: Date;
    endDate: Date;
}

export const eventsApi = {
    createEvent: async (request: CreateEventRequest) =>
        fetchApi<EventResponse>('/api/events', 'POST', request),
    getAllEvents: async () => fetchApi<EventResponse[]>('/api/events'),
    getEvent: async (eventId: string) =>
        fetchApi<EventResponse>(`/api/events/${eventId}`),
    getEventDurationOptions: async () =>
        fetchApi<string[]>('/api/events/durations'),

    getShareableLinks: async (eventId: string) =>
        fetchApi<EventShareableLinkResponse[]>(
            `/api/events/${eventId}/shareableLinks/`,
        ),
    createShareableLink: async (eventId: string) =>
        fetchApi<EventShareableLinkResponse>(
            `/api/events/${eventId}/shareableLinks/`,
            'POST',
        ),

    joinEvent: async (request: JoinEventRequest) => 
        fetchApi<JoinEventResponse>(
            `/api/events/join`,
            'POST',
            request
        ),
    
};
