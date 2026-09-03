import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { DashboardLayout } from '../layouts/DashboardLayout';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { PartyPopper } from 'lucide-react';

export function UpgradeSuccess() {
  const { logout } = useAuth();
  const [loggedOut, setLoggedOut] = useState(false);

  useEffect(() => {
    // Webhook needs a moment to process before the tier is actually updated server-side.
    // Force a logout so the next login issues a fresh token with the correct tier claim,
    // rather than trusting a stale in-memory refresh.
    const timeout = setTimeout(async () => {
      await logout();
      setLoggedOut(true);
    }, 2000);
    return () => clearTimeout(timeout);
  }, [logout]);

  return (
    <DashboardLayout>
      <Card className="max-w-md mx-auto text-center py-10" shadowColor="pink">
        <div className="w-16 h-16 rounded-full bg-quaternary/20 flex items-center justify-center mx-auto mb-4">
          <PartyPopper size={28} className="text-quaternary" />
        </div>
        <h1 className="text-2xl mb-2">You're on Plus!</h1>
        <p className="font-body text-mutedForeground mb-6">
          {loggedOut
            ? 'Please log back in to see your new Plus features.'
            : 'Finalizing your subscription...'}
        </p>
        {loggedOut ? (
          <Link to="/login"><Button>Log in again</Button></Link>
        ) : (
          <p className="text-sm text-mutedForeground font-body">One moment...</p>
        )}
      </Card>
    </DashboardLayout>
  );
}