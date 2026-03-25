export const truncate = (text: null | string, maxLength: number = 50) => {
    if (text == null) {
        return '';
    }

    if (text.length > maxLength) {
        return text.substring(0, maxLength) + '…';
    }

    return text;
};
