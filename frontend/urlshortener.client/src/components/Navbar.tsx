import { Link, useNavigate, useLocation } from 'react-router-dom';
import { clsx } from 'clsx';
import { useAuth } from '../auth/AuthContext';
import { useScrollPosition } from '../hooks/useScrollPosition';
import { Button } from './ui/Button';
import { Link2, HelpCircle, Flag } from 'lucide-react';

export function Navbar({ transparentUntilScroll = false }: { transparentUntilScroll?: boolean }) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const scrolledPastHero = useScrollPosition(window.innerHeight - 96);
  const isSplashStyle = transparentUntilScroll && !scrolledPastHero;

  return (
      <nav
        className={clsx(
          'sticky top-0 z-50 w-full transition-colors duration-300',
          isSplashStyle
            ? 'bg-gradient-to-r from-[#1a1533]/95 via-[#221a3d]/95 to-[#1a1533]/95 backdrop-blur-md border-b border-white/10 shadow-[0_10px_30px_rgba(26,21,51,0.2)]'
            : 'bg-background/90 backdrop-blur-md border-b-2 border-foreground'
        )}
      >
      <div className={clsx('flex items-center justify-between px-6 py-4 max-w-6xl mx-auto', isSplashStyle && 'text-white')}>
        <Link to="/" className="flex items-center gap-2 font-heading font-extrabold text-xl">
          <span className="flex items-center justify-center w-9 h-9 rounded-full bg-accent text-white">
            <Link2 size={18} strokeWidth={2.5} />
          </span>
          Snippy
        </Link>

        <div className="flex items-center gap-2 sm:gap-3">
          <Link
            to="/faq"
            title="FAQ"
            className={clsx(
              'hidden sm:flex items-center justify-center w-9 h-9 rounded-full transition-colors',
              isSplashStyle ? 'text-white/70 hover:text-white hover:bg-white/10' : 'text-mutedForeground hover:text-accent hover:bg-accent/5'
            )}
          >
            <HelpCircle size={17} />
          </Link>
          <Link
            to="/report"
            title="Report a link"
            className={clsx(
              'hidden sm:flex items-center justify-center w-9 h-9 rounded-full transition-colors',
              isSplashStyle ? 'text-white/70 hover:text-white hover:bg-white/10' : 'text-mutedForeground hover:text-secondary hover:bg-secondary/5'
            )}
          >
            <Flag size={17} />
          </Link>

          <div className={clsx('w-px h-6 hidden sm:block mx-1', isSplashStyle ? 'bg-white/20' : 'bg-border')} aria-hidden="true" />

          {user ? (
            <>
              <Link
                to="/dashboard"
                className={clsx(
                  'font-bold text-sm hidden sm:inline transition-colors',
                  isSplashStyle ? 'text-white/90 hover:text-white' : 'hover:text-accent',
                  location.pathname.startsWith('/dashboard') && (isSplashStyle ? 'text-white' : 'text-accent')
                )}
              >
                Dashboard
              </Link>
              {user.role === 'Admin' && (
                <Link to="/admin" className={clsx('font-bold text-sm hidden sm:inline', isSplashStyle ? 'text-white/90 hover:text-white' : 'hover:text-accent')}>
                  Admin
                </Link>
              )}
              <Button variant="secondary" onClick={async () => { await logout(); navigate('/'); }} className={isSplashStyle ? '!border-white !text-white hover:!bg-white/10' : ''}>
                Log out
              </Button>
            </>
          ) : (
            <>
              <Link to="/login" className={clsx('font-bold text-sm hidden sm:inline', isSplashStyle ? 'text-white/90 hover:text-white' : 'hover:text-accent')}>
                Log in
              </Link>
              <Button onClick={() => navigate('/register')} className={isSplashStyle ? '!border-white/80 shadow-[4px_4px_0px_0px_rgba(244,114,182,0.55)] hover:shadow-[6px_6px_0px_0px_rgba(244,114,182,0.55)]' : ''}>
                Sign up
              </Button>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
