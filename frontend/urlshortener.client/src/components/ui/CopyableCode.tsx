import { useState } from 'react';
import { Copy, Check } from 'lucide-react';

export function CopyableCode({ value }: { value: string }) {
  const [copied, setCopied] = useState(false);

  const handleCopy = async () => {
    await navigator.clipboard.writeText(value);
    setCopied(true);
    setTimeout(() => setCopied(false), 1500);
  };

  return (
    <button
      onClick={handleCopy}
      className="inline-flex items-center gap-2 bg-muted border-2 border-foreground rounded-md px-3 py-2 font-body font-bold text-sm hover:bg-tertiary/20 transition-colors"
    >
      {value}
      {copied ? <Check size={14} className="text-quaternary" /> : <Copy size={14} />}
    </button>
  );
}