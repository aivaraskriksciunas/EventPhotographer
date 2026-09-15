import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_API_BASE_URL}/hubs/upload-notifications`, {
        withCredentials: true,
    })
    .withAutomaticReconnect()
    .build();

export interface UploadFileNotification {
    mediaId: string;
    status: 'Validated' | 'Invalid';
}

export interface GeneratedThumbnailNotification {
    mediaId: string;
    thumbnailFileId: string;
}

export const uploadNotificationsApi = {
    subscribeToNotifications: async () => {
        if (connection.state === signalR.HubConnectionState.Disconnected) {
            await connection.start();
        }

        if (connection.state !== signalR.HubConnectionState.Connected) {
            return;
        }

        await connection.send('SubscribeToUploadNotifications');
    },
    unsubscribeFromNotifications: async () => {
        if (connection.state === signalR.HubConnectionState.Connected) {
            await connection.stop();
        }
    },
    onUploadNotification: (
        callback: (notification: UploadFileNotification) => void,
    ) => {
        connection.on('ReceiveUploadCompletedNotification', callback);
    },
    offUploadNotification: (
        callback: (notification: UploadFileNotification) => void,
    ) => {
        connection.off('ReceiveUploadCompletedNotification', callback);
    },
    onThumbnailGeneratedNotification: (
        callback: (notification: GeneratedThumbnailNotification) => void,
    ) => {
        connection.on('ReceiveThumbnailGeneratedNotification', callback);
    },
    offThumbnailGeneratedNotification: (
        callback: (notification: GeneratedThumbnailNotification) => void,
    ) => {
        connection.off('ReceiveThumbnailGeneratedNotification', callback);
    },
};
