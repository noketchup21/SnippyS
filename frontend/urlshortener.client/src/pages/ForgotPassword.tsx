import { useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authApi } from '../api/auth';
import { AuthLayout } from '../layouts/AuthLayout';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { KeyRound } from 'lucide-react';

export function ForgotPassword() {
  const navigate = useNavigate();
  const [step, setStep] = useState<'request' | 'reset'>('request');
  const [email, setEmail] = useState('');
  const [code, setCode] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const passwordsMatch = newPassword === confirmPassword;
  const showMismatch = confirmPassword.length > 0 && !passwordsMatch;

  const handleRequestCode = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsSubmitting(true);
    try {
      await authApi.forgotPassword({ email });
      setMessage('If that email exists, a reset code has been sent.');
      setStep('reset');
    } catch {
      setError('Something went wrong. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleReset = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    if (!passwordsMatch) {
      setError('Passwords do not match.');
      return;
    }
    setIsSubmitting(true);
    try {
      await authApi.resetPassword({ email, code, newPassword });
      navigate('/login');
    } catch {
      setError('Invalid or expired code.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <AuthLayout>
      <div className="text-center mb-8">
        <div className="w-14 h-14 rounded-full bg-accent/10 flex items-center justify-center mx-auto mb-4">
          <KeyRound size={24} className="text-accent" />
        </div>
        {step === 'request' ? (
          <>
            <h1 className="text-2xl mb-1">Reset your password</h1>
            <p className="font-body text-mutedForeground text-sm">We'll send a code to your email</p>
          </>
        ) : (
          <>
            <h1 className="text-2xl mb-1">Enter your code</h1>
            <p className="font-body text-mutedForeground text-sm">Check your email for the 6-digit code</p>
          </>
        )}
      </div>

      {step === 'request' ? (
        <form onSubmit={handleRequestCode} className="flex flex-col gap-4">
          <Input
            label="Email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            placeholder="you@example.com"
          />
          {error && (
            <p className="text-sm font-bold text-pink-700 bg-secondary/10 border-2 border-secondary rounded-md px-3 py-2">
              {error}
            </p>
          )}
          <Button type="submit" disabled={isSubmitting} className="justify-center">
            {isSubmitting ? 'Sending...' : 'Send reset code'}
          </Button>
        </form>
      ) : (
        <form onSubmit={handleReset} className="flex flex-col gap-4">
          <Input
            label="Code"
            value={code}
            onChange={(e) => setCode(e.target.value.replace(/\D/g, '').slice(0, 6))}
            placeholder="123456"
            className="text-center text-xl tracking-[0.4em] font-heading font-bold"
            maxLength={6}
            required
          />
          <Input
            label="New password"
            type="password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            required
            minLength={8}
          />
          <div>
            <Input
              label="Confirm new password"
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
              className={showMismatch ? '!border-secondary' : ''}
            />
            {showMismatch && <p className="text-xs font-bold text-pink-700 mt-1">Passwords don't match</p>}
          </div>
          {message && !error && <p className="text-sm font-bold text-quaternary">{message}</p>}
          {error && (
            <p className="text-sm font-bold text-pink-700 bg-secondary/10 border-2 border-secondary rounded-md px-3 py-2">
              {error}
            </p>
          )}
          <Button type="submit" disabled={isSubmitting} className="justify-center">
            {isSubmitting ? 'Resetting...' : 'Reset password'}
          </Button>
        </form>
      )}

      <p className="text-center font-body text-mutedForeground mt-8">
        <Link to="/login" className="text-accent font-bold hover:underline">← Back to login</Link>
      </p>
    </AuthLayout>
  );
}