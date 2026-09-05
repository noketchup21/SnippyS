import { useEffect, useState } from 'react';
import { linksApi } from '../api/links';
import type { LinkSummary } from '../api/links';
import { buildShortUrl } from '../api/config';
import { useAuth } from '../auth/AuthContext';
import { DashboardLayout } from '../layouts/DashboardLayout';
import { Card } from '../components/ui/Card';
import { CopyableCode } from '../components/ui/CopyableCode';
import { VtStatusBadge } from '../components/ui/StatusBadge';
import { Badge } from '../components/ui/Badge';
import { EmptyState } from '../components/ui/EmptyState';
import { Spinner } from '../components/ui/Spinner';
import { Pagination } from '../components/ui/Pagination';
import { Link2, Lock, MousePointerClick, ShieldCheck, Sparkles } from 'lucide-react';
import { Link } from 'react-router-dom';

const PAGE_SIZE = 10;

export function Dashboard() {
  const { user: _user } = useAuth(); // only if you want to silence without deleting
  const [links, setLinks] = useState<LinkSummary[] | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLinks(null);
    linksApi.getMyLinks(page, PAGE_SIZE)
      .then((res) => {
        setLinks(res.data.items);
        setTotalCount(res.data.totalCount);
      })
      .catch(() => setError('Could not load your links.'));
  }, [page]);

  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));
  const activeCount = links?.filter((l) => l.isActive).length ?? 0;
  const cleanCount = links?.filter((l) => l.vtStatus === 'Clean').length ?? 0;

  return (
    <DashboardLayout>
      <div className="flex items-center justify-between flex-wrap gap-4 mb-6">
        <div>
          <h1 className="text-2xl mb-1">My Links</h1>
          <p className="font-body text-mutedForeground text-sm">
            {totalCount > 0 ? `${totalCount} link${totalCount === 1 ? '' : 's'} total` : 'Your shortened links live here'}
          </p>
        </div>
        <Link to="/">
          <button className="inline-flex items-center gap-2 rounded-full border-2 border-foreground bg-accent text-white font-heading font-bold px-5 py-2.5 text-sm shadow-pop hover:shadow-pop-hover hover:-translate-x-0.5 hover:-translate-y-0.5 transition-all">
            + New link
          </button>
        </Link>
      </div>

      {totalCount > 0 && (
        <div className="grid grid-cols-2 sm:grid-cols-3 gap-4 mb-8">
          <StatChip icon={<Link2 size={16} />} label="Total links" value={totalCount} color="bg-accent/10 text-accent" />
          <StatChip icon={<ShieldCheck size={16} />} label="Active" value={activeCount} color="bg-quaternary/15 text-emerald-700" />
          <StatChip icon={<MousePointerClick size={16} />} label="Clean scans" value={cleanCount} color="bg-tertiary/15 text-amber-700" />
        </div>
      )}

      {error && (
        <p className="font-bold text-pink-700 bg-secondary/10 border-2 border-secondary rounded-md px-4 py-3 mb-4">
          {error}
        </p>
      )}

      {links === null && !error && <Spinner label="Loading your links..." />}

      {links && links.length === 0 && page === 1 && (
        <Card>
          <EmptyState
            icon={<Link2 size={24} />}
            title="No links yet"
            description="Head to the home page to shorten your first link — it'll show up here."
            action={<Link to="/" className="font-bold text-accent hover:underline">Shorten a link →</Link>}
          />
        </Card>
      )}

      {links && links.length > 0 && (
        <>
          <div className="flex flex-col gap-3">
            {links.map((link) => (
              <Link key={link.id} to={`/dashboard/links/${link.id}`}>
                <Card className="!hover:-rotate-0 !hover:scale-[1.005] flex items-center justify-between gap-4 flex-wrap">
                  <div className="flex items-center gap-3 flex-wrap min-w-0">
                    <div className="w-9 h-9 rounded-full bg-accent/10 flex items-center justify-center shrink-0">
                      <Link2 size={14} className="text-accent" />
                    </div>
                    <div className="min-w-0">
                      <div className="flex items-center gap-2 flex-wrap">
                        <CopyableCode value={buildShortUrl(link.shortCode)} />
                        {link.isCustomAlias && (
                          <span title="Custom alias"><Sparkles size={13} className="text-tertiary" /></span>
                        )}
                        {link.hasPassword && (
                          <span title="Password protected"><Lock size={13} className="text-mutedForeground" /></span>
                        )}
                      </div>
                      <p className="text-sm text-mutedForeground font-body truncate max-w-xs mt-0.5">
                        → {link.originalUrl}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2 shrink-0">
                    <Badge status={link.isActive ? 'success' : 'neutral'}>
                      {link.isActive ? 'Active' : 'Inactive'}
                    </Badge>
                    <VtStatusBadge status={link.vtStatus} />
                  </div>
                </Card>
              </Link>
            ))}
          </div>

          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </>
      )}
    </DashboardLayout>
  );
}

function StatChip({ icon, label, value, color }: { icon: React.ReactNode; label: string; value: number; color: string }) {
  return (
    <div className={`flex items-center gap-3 rounded-lg border-2 border-foreground px-4 py-3 ${color}`}>
      <span className="shrink-0">{icon}</span>
      <div>
        <p className="font-heading font-extrabold text-lg leading-none">{value}</p>
        <p className="text-xs font-bold uppercase tracking-wide opacity-70">{label}</p>
      </div>
    </div>
  );
}