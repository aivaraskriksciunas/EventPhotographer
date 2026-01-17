import { fetchApi } from '@/api/client';

export const getEventDurationOptions = async (): Promise<string[]> => {
    return fetchApi('/api/events/durations');
};
