import type { ReactNode } from 'react';
import { clsx } from 'clsx';

type BadgeStatus = 'success' | 'warning' | 'danger' | 'neutral';

const statusStyles: Record<BadgeStatus, string> = {
  success: 'bg-quaternary/20 text-emerald-700 border-quaternary',
  warning: 'bg-tertiary/20 text-amber-700 border-tertiary',
  danger: 'bg-secondary/20 text-pink-700 border-secondary',
  neutral: 'bg-muted text-mutedForeground border-border',
};

export function Badge({ status, children }: { status: BadgeStatus; children: ReactNode }) {
  return (
    <span className={clsx(
      'inline-flex items-center gap-1 px-3 py-1 rounded-full border-2 text-sm font-bold',
      statusStyles[status]
    )}>
      {children}
    </span>
  );
}