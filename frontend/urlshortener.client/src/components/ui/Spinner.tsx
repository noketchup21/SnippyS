import { Loader2 } from 'lucide-react';

export function Spinner({ label }: { label?: string }) {
  return (
    <div className="flex flex-col items-center justify-center gap-2 py-12 text-mutedForeground">
      <Loader2 size={28} className="animate-spin text-accent" />
      {label && <p className="font-body text-sm">{label}</p>}
    </div>
  );
}