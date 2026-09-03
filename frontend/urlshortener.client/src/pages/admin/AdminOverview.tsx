import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { adminApi } from '../../api/admin';
import type { AdminStats } from '../../api/admin';
import type { AdminLogEntry } from '../../api/types';
import { AdminLayout } from '../../layouts/AdminLayout';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Spinner } from '../../components/ui/Spinner';
import {
  Flag, ShieldAlert, ShieldCheck, ShieldX, Users, Sparkles,
  Link2, Ban, Clock,
} from 'lucide-react';

export function AdminOverview() {
  const [stats, setStats] = useState<AdminStats | null>(null);
  const [recentLogs, setRecentLogs] = useState<AdminLogEntry[] | null>(null);

  useEffect(() => {
    adminApi.getStats().then((res) => setStats(res.data));
    adminApi.getLogs().then((res) => setRecentLogs(res.data.slice(0, 6)));
  }, []);

  if (!stats) {
    return (
      <AdminLayout>
        <Spinner label="Loading overview..." />
      </AdminLayout>
    );
  }

  return (
    <AdminLayout>
      <h1 className="text-2xl mb-6">Overview</h1>

      {/* Attention-needed row — surfaced first since these need action */}
      <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-4 mb-8">
        <AlertStatCard
          icon={<Flag size={20} />}
          label="Pending Reports"
          value={stats.pendingReports}
          to="/admin/reports"
          highlight={stats.pendingReports > 0}
        />
        <AlertStatCard
          icon={<ShieldAlert size={20} />}
          label="Unresolved VT Checks"
          value={stats.unresolvedLinks}
          to="/admin/links"
          highlight={stats.unresolvedLinks > 0}
        />
        <AlertStatCard
          icon={<ShieldX size={20} />}
          label="Malicious Links Flagged"
          value={stats.maliciousLinks}
          to="/admin/links"
          highlight={stats.maliciousLinks > 0}
        />
      </div>

      {/* Users breakdown */}
      <h2 className="text-lg font-heading font-extrabold mb-3">Users</h2>
      <div className="grid sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        <StatTile icon={<Users size={18} />} label="Total Users" value={stats.totalUsers} to="/admin/users" />
        <StatTile icon={<Sparkles size={18} />} label="Plus Subscribers" value={stats.plusUsers} />
        <StatTile
          icon={<Users size={18} />}
          label="Standard Users"
          value={stats.totalUsers - stats.plusUsers}
        />
        <StatTile icon={<Ban size={18} />} label="Banned" value={stats.bannedUsers} />
      </div>

      {/* Links breakdown */}
      <h2 className="text-lg font-heading font-extrabold mb-3">Links</h2>
      <div className="grid sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        <StatTile icon={<Link2 size={18} />} label="Total Links" value={stats.totalLinks} />
        <StatTile icon={<ShieldCheck size={18} />} label="Active" value={stats.activeLinks} />
        <StatTile icon={<Ban size={18} />} label="Inactive" value={stats.inactiveLinks} />
        <StatTile icon={<Clock size={18} />} label="Scan Pending" value={stats.pendingLinks} />
      </div>

      {/* VT scan breakdown — visual bar */}
      <h2 className="text-lg font-heading font-extrabold mb-3">Scan Status Breakdown</h2>
      <Card className="mb-8">
        <ScanBreakdownBar
          clean={stats.cleanLinks}
          pending={stats.pendingLinks}
          malicious={stats.maliciousLinks}
          unresolved={stats.unresolvedLinks}
          total={stats.totalLinks}
        />
      </Card>

      {/* Recent activity */}
      <h2 className="text-lg font-heading font-extrabold mb-3">Recent Admin Activity</h2>
      <Card className="!p-0 overflow-hidden">
        {recentLogs && recentLogs.length === 0 && (
          <p className="font-body text-mutedForeground text-center py-8">No admin activity yet.</p>
        )}
        {recentLogs?.map((log, i) => (
          <div
            key={log.id}
            className={`flex items-center justify-between gap-4 px-4 py-3 ${i !== 0 ? 'border-t border-border' : ''}`}
          >
            <div>
              <p className="font-bold text-sm">{log.action.replaceAll('_', ' ')}</p>
              <p className="text-xs text-mutedForeground font-body">
                {log.targetEntity} • {log.targetId.slice(0, 8)}...
              </p>
            </div>
            <span className="text-xs text-mutedForeground font-body whitespace-nowrap">
              {new Date(log.createdAt).toLocaleString()}
            </span>
          </div>
        ))}
        <Link
          to="/admin/logs"
          className="block text-center text-sm font-bold text-accent hover:underline py-3 border-t border-border"
        >
          View all logs →
        </Link>
      </Card>
    </AdminLayout>
  );
}

function AlertStatCard({ icon, label, value, to, highlight }: {
  icon: React.ReactNode; label: string; value: number; to?: string; highlight: boolean;
}) {
  const content = (
    <Card className={highlight ? '!border-secondary' : ''} shadowColor={highlight ? 'pink' : 'default'}>
      <div className="flex items-center gap-2 text-mutedForeground mb-2">
        {icon}
        <span className="text-sm font-bold uppercase tracking-wide">{label}</span>
      </div>
      <p className={`font-heading font-extrabold text-3xl ${highlight ? 'text-secondary' : ''}`}>
        {value}
      </p>
      {highlight && <Badge status="danger">Needs attention</Badge>}
    </Card>
  );
  return to ? <Link to={to}>{content}</Link> : content;
}

function StatTile({ icon, label, value, to }: {
  icon: React.ReactNode; label: string; value: number; to?: string;
}) {
  const content = (
    <Card>
      <div className="flex items-center gap-2 text-mutedForeground mb-2">
        {icon}
        <span className="text-xs font-bold uppercase tracking-wide">{label}</span>
      </div>
      <p className="font-heading font-extrabold text-2xl">{value}</p>
    </Card>
  );
  return to ? <Link to={to}>{content}</Link> : content;
}

function ScanBreakdownBar({ clean, pending, malicious, unresolved, total }: {
  clean: number; pending: number; malicious: number; unresolved: number; total: number;
}) {
  if (total === 0) {
    return <p className="font-body text-mutedForeground text-center py-4">No links yet.</p>;
  }

  const segments = [
    { label: 'Clean', value: clean, color: 'bg-quaternary' },
    { label: 'Pending', value: pending, color: 'bg-tertiary' },
    { label: 'Malicious', value: malicious, color: 'bg-secondary' },
    { label: 'Unresolved', value: unresolved, color: 'bg-mutedForeground' },
  ];

  return (
    <div>
      <div className="flex h-6 rounded-full overflow-hidden border-2 border-foreground mb-4">
        {segments.map((s) => (
          s.value > 0 && (
            <div
              key={s.label}
              className={s.color}
              style={{ width: `${(s.value / total) * 100}%` }}
              title={`${s.label}: ${s.value}`}
            />
          )
        ))}
      </div>
      <div className="flex flex-wrap gap-4">
        {segments.map((s) => (
          <div key={s.label} className="flex items-center gap-1.5 text-sm font-body">
            <span className={`w-3 h-3 rounded-full ${s.color}`} />
            {s.label}: <span className="font-bold">{s.value}</span>
          </div>
        ))}
      </div>
    </div>
  );
}