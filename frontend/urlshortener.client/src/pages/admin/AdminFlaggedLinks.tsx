import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { adminApi } from '../../api/admin';
import type { FlaggedLink } from '../../api/admin';
import { AdminLayout } from '../../layouts/AdminLayout';
import { Card } from '../../components/ui/Card';
import { VtStatusBadge } from '../../components/ui/StatusBadge';
import { Badge } from '../../components/ui/Badge';
import { Spinner } from '../../components/ui/Spinner';

export function AdminFlaggedLinks() {
  const [links, setLinks] = useState<FlaggedLink[] | null>(null);

  useEffect(() => {
    adminApi.getFlaggedLinks().then((res) => setLinks(res.data));
  }, []);

  return (
    <AdminLayout>
      <h1 className="text-2xl mb-6">Flagged Links</h1>

      {links === null && <Spinner label="Loading flagged links..." />}

      {links && links.length === 0 && (
        <Card><p className="font-body text-mutedForeground text-center py-8">Nothing flagged right now.</p></Card>
      )}

      <div className="flex flex-col gap-3">
        {links?.map((link) => (
          <Link key={link.id} to={`/admin/links/${link.id}`}>
            <Card className="flex items-center justify-between gap-4 flex-wrap">
              <div>
                <p className="font-bold text-sm mb-1">{link.shortCode}</p>
                <p className="font-body text-mutedForeground text-sm truncate max-w-md">{link.originalUrl}</p>
              </div>
              <div className="flex items-center gap-2">
                <Badge status={link.isActive ? 'success' : 'neutral'}>
                  {link.isActive ? 'Active' : 'Inactive'}
                </Badge>
                <VtStatusBadge status={link.vtStatus} />
              </div>
            </Card>
          </Link>
        ))}
      </div>
    </AdminLayout>
  );
}