import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { linksApi } from '../api/links';
import type { LinkDetail as LinkDetailType } from '../api/links';
import type { DailyStatDto } from '../api/types';
import { buildShortUrl } from '../api/config';
import { useAuth } from '../auth/AuthContext';
import { DashboardLayout } from '../layouts/DashboardLayout';
import { Card } from '../components/ui/Card';
import { CopyableCode } from '../components/ui/CopyableCode';
import { VtStatusBadge } from '../components/ui/StatusBadge';
import { Badge } from '../components/ui/Badge';
import { AnalyticsChart } from '../components/AnalyticsChart';
import { Spinner } from '../components/ui/Spinner';
import { ArrowLeft, Lock, QrCode, Sparkles, Calendar, TrendingUp } from 'lucide-react';

export function LinkDetail() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();
  const isPlus = user?.tier === 'Plus';

  const [link, setLink] = useState<LinkDetailType | null>(null);
  const [stats, setStats] = useState<DailyStatDto[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    Promise.all([linksApi.getById(id), linksApi.getAnalytics(id)])
      .then(([linkRes, statsRes]) => {
        setLink(linkRes.data);
        setStats(statsRes.data);
      })
      .catch(() => setError('Could not load this link.'));
  }, [id]);

  if (error) {
    return (
      <DashboardLayout>
        <p className="font-bold text-pink-700">{error}</p>
      </DashboardLayout>
    );
  }

  if (!link) {
    return (
      <DashboardLayout>
        <Spinner label="Loading link details..." />
      </DashboardLayout>
    );
  }

  const totalClicks = stats.reduce((sum, s) => sum + s.clickCount, 0);

  return (
    <DashboardLayout>
      <Link to="/dashboard" className="inline-flex items-center gap-1 font-bold text-sm text-mutedForeground hover:text-accent mb-4">
        <ArrowLeft size={16} /> Back to links
      </Link>

      {/* Hero-style header card */}
      <Card className="mb-6 relative overflow-hidden" shadowColor="pink">
        <div className="absolute -top-8 -right-8 w-32 h-32 bg-accent/10 rounded-full -z-0" aria-hidden="true" />
        <div className="relative flex items-start justify-between flex-wrap gap-4">
          <div>
            <div className="flex items-center gap-3 mb-2 flex-wrap">
              <CopyableCode value={buildShortUrl(link.shortCode)} />
              {link.isCustomAlias && (
                <Badge status="neutral"><Sparkles size={12} /> Custom alias</Badge>
              )}
              {link.hasPassword && (
                <span title="Password protected"><Lock size={15} className="text-mutedForeground" /></span>
              )}
            </div>
            <p className="font-body text-mutedForeground break-all max-w-md">→ {link.originalUrl}</p>
            <p className="text-xs text-mutedForeground font-body mt-2 flex items-center gap-1">
              <Calendar size={12} /> Created {new Date(link.createdAt).toLocaleDateString()}
            </p>
          </div>
          <div className="flex items-center gap-2">
            <Badge status={link.isActive ? 'success' : 'neutral'}>
              {link.isActive ? 'Active' : 'Inactive'}
            </Badge>
            <VtStatusBadge status={link.vtStatus} />
          </div>
        </div>
      </Card>

      {/* Quick stat strip */}
      <div className="grid grid-cols-2 gap-4 mb-6">
        <div className="flex items-center gap-3 rounded-lg border-2 border-foreground bg-accent/10 text-accent px-4 py-3">
          <TrendingUp size={18} />
          <div>
            <p className="font-heading font-extrabold text-xl leading-none">{totalClicks}</p>
            <p className="text-xs font-bold uppercase tracking-wide opacity-70">Total clicks</p>
          </div>
        </div>
        <div className="flex items-center gap-3 rounded-lg border-2 border-foreground bg-quaternary/10 text-emerald-700 px-4 py-3">
          <Calendar size={18} />
          <div>
            <p className="font-heading font-extrabold text-xl leading-none">{stats.length}</p>
            <p className="text-xs font-bold uppercase tracking-wide opacity-70">Active days</p>
          </div>
        </div>
      </div>

      <Card className="mb-6">
        <h3 className="text-lg mb-4">Clicks — last 30 days</h3>
        <AnalyticsChart data={stats} />
      </Card>

      <div className="grid sm:grid-cols-2 gap-6">
        <Card>
          <h3 className="text-lg mb-3 flex items-center gap-2">
            <QrCode size={18} /> QR Code
          </h3>
          {isPlus ? (
            <img
              src={linksApi.getQrCode(link.id)}
              alt="QR code for this link"
              className="w-40 h-40 border-2 border-foreground rounded-md"
            />
          ) : (
            <PlusUpsell feature="QR codes" />
          )}
        </Card>

        <Card>
          <h3 className="text-lg mb-3 flex items-center gap-2">
            <Lock size={18} /> Password Protection
          </h3>
          {isPlus ? (
            <p className="font-body text-mutedForeground">
              {link.hasPassword ? 'This link is password protected.' : 'No password set on this link.'}
            </p>
          ) : (
            <PlusUpsell feature="password protection" />
          )}
        </Card>
      </div>
    </DashboardLayout>
  );
}

function PlusUpsell({ feature }: { feature: string }) {
  return (
    <div className="flex flex-col items-center text-center py-6">
      <div className="w-10 h-10 rounded-full bg-tertiary/20 flex items-center justify-center mb-2">
        <Lock size={16} className="text-tertiary" />
      </div>
      <p className="font-body text-mutedForeground mb-3 text-sm">
        {feature[0].toUpperCase() + feature.slice(1)} is a Plus feature.
      </p>
      <Link to="/account/upgrade" className="font-bold text-accent hover:underline text-sm">
        Upgrade to Plus →
      </Link>
    </div>
  );
}