const longDateTimeFormatter = new Intl.DateTimeFormat(undefined, {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
});
export const formatLongDateTime = (date: Date | string) =>
    longDateTimeFormatter.format(new Date(date));
