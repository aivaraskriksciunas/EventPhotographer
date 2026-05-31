import { EventShareableLinkResponse } from "@/api/events";
import { WhatsAppIcon } from "@/components/icons/WhatsAppIcon";
import clsx from "clsx";
import { useTranslation } from "react-i18next";

export function WhatsAppLinkButton({ shareableLink, className }: { shareableLink: EventShareableLinkResponse, className?: string }) {
    const encodedMessage = encodeURIComponent(`Hello! I am joining ${shareableLink.code}`);
    const fullUrl = `https://wa.me/${import.meta.env.VITE_WHATSAPP_PHONE_NUMBER}/?text=` + encodedMessage;
    const { t } = useTranslation();

    return (
        <a href={fullUrl} target="_blank" className={clsx('btn btn-whatsapp-outline', className)}>
            <WhatsAppIcon size={16}></WhatsAppIcon>
            <span className="ms-1">{t('Upload using WhatsApp')}</span>
        </a>
    )
}