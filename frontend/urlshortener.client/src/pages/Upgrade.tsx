import { useState } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { subscriptionsApi } from '../api/subscriptions';
import { DashboardLayout } from '../layouts/DashboardLayout';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Check, Sparkles, MapPin, Globe2, Lock } from 'lucide-react';
import clsx from 'clsx';

const STANDARD_FEATURES = [
  '10 links per day',
  'Basic click analytics',
  'Malware scanning on every link',
];

const PLUS_FEATURES = [
  'Unlimited link shortening',
  'Custom aliases',
  'QR code generation',
  'Bulk shorten',
  'Password-protected links',
  'AI-generated link summaries',
];

export function Upgrade() {
  const { user } = useAuth();
  const [isRedirecting, setIsRedirecting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedProvider, setSelectedProvider] =
    useState<'PayOS' | 'Stripe'>('PayOS');

  if (user?.tier === 'Plus') {
    return <Navigate to="/account" replace />;
  }

  const handleUpgrade = async () => {
    if (selectedProvider === 'Stripe') return;

    setError(null);
    setIsRedirecting(true);

    try {
      const res = await subscriptionsApi.createCheckoutSession(
        selectedProvider
      );
      window.location.href = res.data.checkoutUrl;
    } catch {
      setError('Could not start checkout. Please try again.');
      setIsRedirecting(false);
    }
  };

  return (
    <DashboardLayout>
      <h1 className="text-2xl text-center mb-8">Choose your plan</h1>

      <div className="grid md:grid-cols-2 gap-6 max-w-3xl mx-auto items-start">
        {/* Standard — current plan */}
        <Card className="relative">
          <h2 className="text-xl mb-1">Standard</h2>

          <p className="font-heading font-extrabold text-3xl mb-4">
            Free
          </p>

          <ul className="flex flex-col gap-2 mb-6">
            {STANDARD_FEATURES.map((feature) => (
              <li
                key={feature}
                className="flex items-center gap-2 font-body text-sm"
              >
                <span className="flex items-center justify-center w-5 h-5 rounded-full bg-muted text-mutedForeground shrink-0">
                  <Check size={12} strokeWidth={3} />
                </span>
                {feature}
              </li>
            ))}
          </ul>

          <Button
            variant="secondary"
            disabled
            className="w-full justify-center opacity-60 cursor-default"
          >
            Your current plan
          </Button>
        </Card>

        {/* Plus — upgrade target */}
        <Card shadowColor="pink" className="relative">
          <div className="absolute -top-4 -right-4 bg-tertiary border-2 border-foreground rounded-full px-4 py-1 rotate-12 font-heading font-extrabold text-sm">
            MOST POPULAR
          </div>

          <div className="flex items-center gap-2 mb-1">
            <Sparkles size={20} className="text-accent" />
            <h2 className="text-xl">Plus</h2>
          </div>

          <p className="font-heading font-extrabold text-3xl mb-4">
            $3.99
            <span className="text-base font-body font-normal text-mutedForeground">
              /month
            </span>
          </p>

          <ul className="flex flex-col gap-2 mb-6">
            {PLUS_FEATURES.map((feature) => (
              <li
                key={feature}
                className="flex items-center gap-2 font-body text-sm"
              >
                <span className="flex items-center justify-center w-5 h-5 rounded-full bg-quaternary/20 text-quaternary shrink-0">
                  <Check size={12} strokeWidth={3} />
                </span>
                {feature}
              </li>
            ))}
          </ul>

          {error && (
            <p className="text-sm font-bold text-pink-700 bg-secondary/10 border-2 border-secondary rounded-md px-3 py-2 mb-4">
              {error}
            </p>
          )}

          {/* Payment method selector */}
          <div className="mb-4">
            <p className="text-xs font-bold uppercase tracking-wide text-mutedForeground mb-2">
              Payment method
            </p>

            <div className="grid grid-cols-2 gap-3">
              <button
                type="button"
                onClick={() => setSelectedProvider('PayOS')}
                className={clsx(
                  'flex flex-col items-center gap-1.5 rounded-xl border-2 px-3 py-3 transition-all',
                  selectedProvider === 'PayOS'
                    ? 'border-accent bg-accent/5 shadow-[3px_3px_0px_0px_#8B5CF6]'
                    : 'border-border bg-white hover:border-mutedForeground'
                )}
              >
                <MapPin
                  size={18}
                  className={
                    selectedProvider === 'PayOS'
                      ? 'text-accent'
                      : 'text-mutedForeground'
                  }
                />

                <span className="text-xs font-bold">Vietnam</span>

                <span className="text-[10px] text-mutedForeground">
                  via PayOS
                </span>
              </button>

              <button
                type="button"
                onClick={() => setSelectedProvider('Stripe')}
                className="flex flex-col items-center gap-1.5 rounded-xl border-2 border-border bg-muted px-3 py-3 opacity-60 cursor-not-allowed relative"
                disabled
              >
                <Globe2 size={18} className="text-mutedForeground" />

                <span className="text-xs font-bold text-mutedForeground">
                  Global
                </span>

                <span className="text-[10px] text-mutedForeground">
                  via Stripe
                </span>

                <span className="absolute -top-2 -right-2 flex items-center gap-0.5 bg-tertiary border-2 border-foreground rounded-full px-1.5 py-0.5 text-[9px] font-bold">
                  <Lock size={8} />
                  Soon
                </span>
              </button>
            </div>
          </div>

          <Button
            onClick={handleUpgrade}
            disabled={isRedirecting}
            className="w-full justify-center"
          >
            {isRedirecting
              ? 'Redirecting to checkout...'
              : 'Upgrade now'}
          </Button>
        </Card>
      </div>
    </DashboardLayout>
  );
}