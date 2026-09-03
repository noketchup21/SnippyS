import { useEffect, useState } from 'react';
import { useAuth } from '../auth/AuthContext';
import { subscriptionsApi } from '../api/subscriptions';
import type { SubscriptionStatus } from '../api/subscriptions';
import { DashboardLayout } from '../layouts/DashboardLayout';
import { Card } from '../components/ui/Card';
import { Badge } from '../components/ui/Badge';
import { Spinner } from '../components/ui/Spinner';
import { User, Mail, Calendar, ShieldCheck, Sparkles, IdCard } from 'lucide-react';
import { Link } from 'react-router-dom';

export function Account() {
  const { user } = useAuth();
  const [subStatus, setSubStatus] = useState<SubscriptionStatus | null>(null);

  useEffect(() => {
    subscriptionsApi.getStatus().then((res) => setSubStatus(res.data));
  }, []);

  if (!user) return null;
  const isPlus = user.tier === 'Plus';

  return (
    <DashboardLayout>
      <h1 className="text-2xl mb-6 flex items-center gap-2">
        <User size={22} /> Account
      </h1>

      <div className="grid md:grid-cols-2 gap-6 max-w-3xl">
        {/* Profile card */}
        <Card>
          <div className="flex items-center gap-4 mb-5">
            <div className="w-14 h-14 rounded-full bg-accent text-white flex items-center justify-center font-heading font-extrabold text-xl shrink-0">
              {user.email[0].toUpperCase()}
            </div>
            <div className="min-w-0">
              <p className="font-heading font-bold truncate">{user.email}</p>
              <Badge status={isPlus ? 'success' : 'neutral'}>{user.tier}</Badge>
            </div>
          </div>

          <div className="flex flex-col gap-3 pt-4 border-t-2 border-dashed border-border">
            <DetailRow icon={<Mail size={15} />} label="Email" value={user.email} />
            <DetailRow icon={<IdCard size={15} />} label="Account ID" value={user.id} mono />
            <DetailRow icon={<ShieldCheck size={15} />} label="Role" value={user.role} />
          </div>
        </Card>

        {/* Subscription card */}
        <Card shadowColor={isPlus ? 'pink' : 'default'}>
          <div className="flex items-center gap-2 mb-4">
            {isPlus && <Sparkles size={18} className="text-accent" />}
            <h3 className="text-lg">Subscription</h3>
          </div>

          {!subStatus && <Spinner />}

          {subStatus && (
            <div className="flex flex-col gap-3">
              <DetailRow icon={<ShieldCheck size={15} />} label="Plan" value={subStatus.tier} />
              {subStatus.status && (
                <DetailRow icon={<ShieldCheck size={15} />} label="Status" value={subStatus.status} />
              )}
              {subStatus.currentPeriodEnd && (
                <DetailRow
                  icon={<Calendar size={15} />}
                  label="End"
                  value={new Date(subStatus.currentPeriodEnd).toLocaleDateString(undefined, {
                    year: 'numeric', month: 'long', day: 'numeric'
                  })}
                />
              )}
            </div>
          )}

          {!isPlus && (
            <Link to="/account/upgrade" className="block mt-5">
              <button className="w-full inline-flex items-center justify-center gap-2 rounded-full border-2 border-foreground bg-accent text-white font-heading font-bold px-5 py-2.5 text-sm shadow-pop hover:shadow-pop-hover hover:-translate-x-0.5 hover:-translate-y-0.5 transition-all">
                <Sparkles size={15} /> Upgrade to Plus
              </button>
            </Link>
          )}
        </Card>
      </div>
    </DashboardLayout>
  );
}

function DetailRow({ icon, label, value, mono }: { icon: React.ReactNode; label: string; value: string; mono?: boolean }) {
  return (
    <div className="flex items-center justify-between gap-3">
      <span className="flex items-center gap-2 text-xs font-bold uppercase tracking-wide text-mutedForeground shrink-0">
        {icon} {label}
      </span>
      <span className={`text-sm font-body text-right truncate ${mono ? 'font-mono text-xs' : ''}`}>{value}</span>
    </div>
  );
}