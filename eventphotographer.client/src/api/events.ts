import { fetchApi } from '@/api/client';

interface CreateEventRequest {
    name: string;
    startDate: Date;
    duration: string;
}

export interface JoinEventRequest {
    code: string;
    name: string;
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

export interface ParticipantResponse {
    token: string;
    event: EventResponse;
    name: string;
    createdAt: Date;
}

export interface EventMediaFileResponse {
    id: string;
    mimeType: string;
    fileSize: number;
}

export interface EventMediaResponse {
    id: string;
    createdAt: Date;
    files: EventMediaFileResponse[];
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
        fetchApi<ParticipantResponse>(
            `/api/participants/join`,
            'POST',
            request,
        ),
    getCurrentEvent: async () =>
        fetchApi<ParticipantResponse>(`/api/participants/current`),
    leaveCurrentEvent: async () => fetchApi<null>(`/api/participants/leave`),

    getEventMedia: async (eventId: string) =>
        fetchApi<EventMediaResponse[]>(`/api/events/${eventId}/media`),
};
