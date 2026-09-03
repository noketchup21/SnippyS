import { useEffect, useState } from 'react';
import type { FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../api/auth';
import { useAuth } from '../auth/AuthContext';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Card } from '../components/ui/Card';
import { MailCheck } from 'lucide-react';
import { AuthLayout } from '../layouts/AuthLayout';

export function VerifyEmail() {
  const { user, refreshUser } = useAuth();
  const navigate = useNavigate();

  const [code, setCode] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [cooldown, setCooldown] = useState(0);

  useEffect(() => {
    // Auto-send a code on first arrival at this page
    authApi.sendVerificationCode()
      .then(() => setMessage(`Code sent to ${user?.email}`))
      .catch(() => {});
  }, []);

  useEffect(() => {
    if (cooldown <= 0) return;
    const timer = setInterval(() => setCooldown((c) => Math.max(0, c - 1)), 1000);
    return () => clearInterval(timer);
  }, [cooldown]);

  const handleResend = async () => {
    setError(null);
    setMessage(null);
    try {
      await authApi.sendVerificationCode();
      setMessage('A new code has been sent.');
      setCooldown(60);
    } catch (err: any) {
      if (err.response?.status === 429) {
        const detail: string = err.response?.data?.detail ?? '';
        const match = detail.match(/\d+/);
        setCooldown(match ? parseInt(match[0]) : 60);
        setError('Please wait before requesting another code.');
      } else {
        setError('Could not send code. Try again.');
      }
    }
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsSubmitting(true);
    try {
      await authApi.verifyEmail({ code });
      await refreshUser();
      navigate('/dashboard');
    } catch {
      setError('Invalid or expired code. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <AuthLayout>
      <Card className="w-full max-w-md text-center" shadowColor="pink">
        <div className="w-16 h-16 rounded-full bg-accent/10 flex items-center justify-center mx-auto mb-4">
          <MailCheck size={28} className="text-accent" />
        </div>
        <h1 className="text-2xl mb-1">Verify your email</h1>
        <p className="font-body text-mutedForeground mb-6">
          Enter the 6-digit code we sent to <strong>{user?.email}</strong>
        </p>

        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <Input
            value={code}
            onChange={(e) => setCode(e.target.value.replace(/\D/g, '').slice(0, 6))}
            placeholder="123456"
            className="text-center text-2xl tracking-[0.5em] font-heading font-bold"
            maxLength={6}
            required
          />

          {message && <p className="text-sm font-bold text-quaternary">{message}</p>}
          {error && (
            <p className="text-sm font-bold text-pink-700 bg-secondary/10 border-2 border-secondary rounded-md px-3 py-2">
              {error}
            </p>
          )}

          <Button type="submit" disabled={isSubmitting || code.length !== 6} className="justify-center">
            {isSubmitting ? 'Verifying...' : 'Verify email'}
          </Button>
        </form>

        <button
          onClick={handleResend}
          disabled={cooldown > 0}
          className="text-sm font-bold text-accent hover:underline mt-4 disabled:text-mutedForeground disabled:no-underline disabled:cursor-not-allowed"
        >
          {cooldown > 0 ? `Resend code in ${cooldown}s` : 'Resend code'}
        </button>
      </Card>
    </AuthLayout>
  );
}