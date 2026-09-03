import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { adminApi } from '../../api/admin';
import type { AdminLinkDetail } from '../../api/admin';
import { buildShortUrl } from '../../api/config';
import { AdminLayout } from '../../layouts/AdminLayout';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { VtStatusBadge } from '../../components/ui/StatusBadge';
import { CopyableCode } from '../../components/ui/CopyableCode';
import { Spinner } from '../../components/ui/Spinner';
import { ArrowLeft, Lock, User } from 'lucide-react';

export function AdminLinkDetail() {
  const { id } = useParams<{ id: string }>();
  const [link, setLink] = useState<AdminLinkDetail | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    adminApi.getLinkDetail(id)
      .then((res) => setLink(res.data))
      .catch(() => setError('Could not load this link.'));
  }, [id]);

  return (
    <AdminLayout>
      <Link to="/admin/links" className="inline-flex items-center gap-1 font-bold text-sm text-mutedForeground hover:text-accent mb-4">
        <ArrowLeft size={16} /> Back to flagged links
      </Link>

      {error && <p className="font-bold text-pink-700">{error}</p>}
      {!link && !error && <Spinner label="Loading link..." />}

      {link && (
        <Card>
          <div className="flex items-start justify-between flex-wrap gap-4 mb-4">
            <div>
              <div className="flex items-center gap-3 mb-2">
                <CopyableCode value={buildShortUrl(link.shortCode)} />
                {link.isCustomAlias && <Badge status="neutral">Custom alias</Badge>}
                {link.hasPassword && (
                  <span title="Password protected"><Lock size={16} className="text-mutedForeground" /></span>
                )}
              </div>
              <p className="font-body text-mutedForeground break-all">→ {link.originalUrl}</p>
            </div>
            <div className="flex items-center gap-2">
              <Badge status={link.isActive ? 'success' : 'neutral'}>
                {link.isActive ? 'Active' : 'Inactive'}
              </Badge>
              <VtStatusBadge status={link.vtStatus} />
            </div>
          </div>

          <div className="grid sm:grid-cols-2 gap-4 pt-4 border-t-2 border-dashed border-border text-sm font-body">
            <DetailRow label="Created" value={new Date(link.createdAt).toLocaleString()} />
            <DetailRow
              label="Owner"
              value={link.userId ? link.userId : 'Anonymous'}
              icon={<User size={14} />}
            />
            <DetailRow label="VT check attempts" value={String(link.vtCheckAttempts)} />
            <DetailRow
              label="Last VT check"
              value={link.vtCheckedAt ? new Date(link.vtCheckedAt).toLocaleString() : 'Never'}
            />
          </div>
        </Card>
      )}
    </AdminLayout>
  );
}

function DetailRow({ label, value, icon }: { label: string; value: string; icon?: React.ReactNode }) {
  return (
    <div>
      <p className="text-xs font-bold uppercase tracking-wide text-mutedForeground mb-0.5">{label}</p>
      <p className="flex items-center gap-1">{icon}{value}</p>
    </div>
  );
}