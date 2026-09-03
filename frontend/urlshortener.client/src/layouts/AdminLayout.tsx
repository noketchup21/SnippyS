import type { ReactNode } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { clsx } from 'clsx';
import { ShieldAlert, LayoutDashboard, Users, Flag, ScrollText } from 'lucide-react';

export function AdminLayout({ children }: { children: ReactNode }) {
  const location = useLocation();

  const navItems = [
    { to: '/admin', label: 'Overview', icon: LayoutDashboard },
    { to: '/admin/users', label: 'Users', icon: Users },
    { to: '/admin/reports', label: 'Reports', icon: Flag },
    { to: '/admin/links', label: 'Flagged Links', icon: ShieldAlert },
    { to: '/admin/logs', label: 'Audit Logs', icon: ScrollText },
  ];

  return (
    <div className="min-h-screen bg-foreground text-white">
      <div className="flex items-center gap-2 px-6 py-4 border-b-2 border-white/10">
        <ShieldAlert size={20} className="text-tertiary" />
        <span className="font-heading font-extrabold tracking-wide">ADMIN</span>
      </div>

      <div className="flex">
        <aside className="w-56 p-4 flex flex-col gap-1 border-r-2 border-white/10 min-h-[calc(100vh-65px)]">
          {navItems.map(({ to, label, icon: Icon }) => {
            const active = location.pathname === to;
            return (
              <Link
                key={to}
                to={to}
                className={clsx(
                  'flex items-center gap-2 px-3 py-2 rounded-md font-bold text-sm transition-colors',
                  active ? 'bg-tertiary text-foreground' : 'text-white/70 hover:bg-white/10 hover:text-white'
                )}
              >
                <Icon size={16} />
                {label}
              </Link>
            );
          })}

          <Link
            to="/dashboard"
            className="mt-auto px-3 py-2 text-xs font-bold text-white/50 hover:text-white"
          >
            ← Back to app
          </Link>
        </aside>

        <main className="flex-1 p-8 bg-background text-foreground rounded-tl-lg">
          {children}
        </main>
      </div>
    </div>
  );
}