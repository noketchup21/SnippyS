import type { ReactNode } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { clsx } from 'clsx';
import { Link2, LayoutGrid, Layers, User, Lock, Sparkles } from 'lucide-react';

export function DashboardLayout({ children }: { children: ReactNode }) {
  const { user } = useAuth();
  const location = useLocation();
  const isPlus = user?.tier === 'Plus';

  const navItems = [
    { to: '/dashboard', label: 'My Links', icon: LayoutGrid },
    { to: '/dashboard/bulk', label: 'Bulk Shorten', icon: Layers, plusOnly: true },
    { to: '/account', label: 'Account', icon: User },
  ];

  return (
      <div className="min-h-screen flex flex-col md:flex-row">
        <aside className="w-full md:w-64 border-b-2 md:border-b-0 md:border-r-2 border-foreground bg-white p-6 flex md:flex-col gap-1 overflow-x-auto">
        <Link to="/" className="flex items-center gap-2 font-heading font-extrabold text-lg mb-8">
          <span className="flex items-center justify-center w-8 h-8 rounded-full bg-accent text-white">
            <Link2 size={16} strokeWidth={2.5} />
          </span>
          Snippy
        </Link>

        {navItems.map(({ to, label, icon: Icon, plusOnly }) => {
          const locked = plusOnly && !isPlus;
          const active = location.pathname === to;
          return (
            <Link
              key={to}
              to={locked ? '/account/upgrade' : to}
              className={clsx(
                'flex items-center gap-2 px-3 py-2 rounded-md font-bold text-sm transition-colors',
                active ? 'bg-accent text-white' : 'hover:bg-muted',
                locked && 'text-mutedForeground'
              )}
            >
              <Icon size={16} />
              {label}
              {locked && <Lock size={12} className="ml-auto" />}
            </Link>
          );
        })}

        <div className="mt-auto pt-6 border-t-2 border-dashed border-border">
          <div className="flex items-center justify-between mb-1">
            <p className="text-xs font-bold uppercase text-mutedForeground">Plan</p>
            {isPlus && <Sparkles size={12} className="text-tertiary" />}
          </div>
          <p className="font-heading font-bold mb-2 flex items-center gap-1.5">
            {user?.tier}
            {isPlus && <span className="text-[10px] bg-tertiary border border-foreground rounded-full px-1.5 py-0.5">PRO</span>}
          </p>
          {!isPlus && (
            <Link to="/account/upgrade" className="text-sm font-bold text-accent hover:underline">
              Upgrade to Plus →
            </Link>
          )}
        </div>
      </aside>

      <main className="flex-1 p-4 md:p-8">{children}</main>
    </div>
  );
}