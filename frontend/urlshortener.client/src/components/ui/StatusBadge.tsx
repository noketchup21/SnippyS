import { Badge } from './Badge';
import { Loader2, ShieldCheck, ShieldAlert } from 'lucide-react';

type VtStatus = 'Unchecked' | 'Pending' | 'Clean' | 'Malicious' | 'Unresolved';

export function VtStatusBadge({ status }: { status: VtStatus }) {
  switch (status) {
    case 'Clean':
      return <Badge status="success"><ShieldCheck size={14} /> Scanned safe</Badge>;
    case 'Pending':
      return <Badge status="warning"><Loader2 size={14} className="animate-spin" /> Scanning...</Badge>;
    case 'Malicious':
      return <Badge status="danger"><ShieldAlert size={14} /> Flagged unsafe</Badge>;
    default:
      return <Badge status="neutral">Unresolved</Badge>;
  }
}