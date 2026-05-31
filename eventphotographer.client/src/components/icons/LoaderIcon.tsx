import { LoaderCircle } from "lucide-react";

export function LoaderIcon({ size, spinning = true }: { size?: number, spinning: boolean }) {
    return (
        <LoaderCircle size={size} className="spinner"/>
    )
}