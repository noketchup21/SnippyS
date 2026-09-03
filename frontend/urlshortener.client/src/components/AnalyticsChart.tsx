import { BarChart, Bar, XAxis, YAxis, ResponsiveContainer, Tooltip } from 'recharts';
import type { DailyStatDto } from '../api/types';

function formatDateLabel(value: unknown): string {
  const date = new Date(String(value));
  return isNaN(date.getTime()) ? '' : date.toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
}

function formatFullDateLabel(value: unknown): string {
  const date = new Date(String(value));
  return isNaN(date.getTime()) ? '' : date.toLocaleDateString();
}

export function AnalyticsChart({ data }: { data: DailyStatDto[] }) {
  if (data.length === 0) {
    return (
      <p className="font-body text-mutedForeground text-center py-12">
        No clicks yet — share your link to start seeing data here.
      </p>
    );
  }

  return (
    <ResponsiveContainer width="100%" height={220}>
      <BarChart data={data}>
        <XAxis
          dataKey="date"
          tick={{ fontSize: 12, fontFamily: 'Plus Jakarta Sans' }}
          tickFormatter={formatDateLabel}
        />
        <YAxis allowDecimals={false} tick={{ fontSize: 12, fontFamily: 'Plus Jakarta Sans' }} />
        <Tooltip
          contentStyle={{ border: '2px solid #1E293B', borderRadius: 8, fontFamily: 'Plus Jakarta Sans' }}
          labelFormatter={formatFullDateLabel}
        />
        <Bar dataKey="clickCount" fill="#8B5CF6" radius={[6, 6, 0, 0]} />
      </BarChart>
    </ResponsiveContainer>
  );
}