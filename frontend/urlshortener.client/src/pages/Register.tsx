import { useRef, useState } from 'react';
import type { FormEvent } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import type { TurnstileInstance } from '@marsidev/react-turnstile';
import { useAuth } from '../auth/AuthContext';
import { AuthLayout } from '../layouts/AuthLayout';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { CaptchaWidget } from '../components/ui/CaptchaWidget';
import { ArrowRight } from 'lucide-react';

export function Register() {
  const { register } = useAuth();
  const navigate = useNavigate();
  const captchaRef = useRef<TurnstileInstance>(null);

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [captchaToken, setCaptchaToken] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const passwordsMatch = password === confirmPassword;
  const showMismatch = confirmPassword.length > 0 && !passwordsMatch;

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!passwordsMatch) {
      setError('Passwords do not match.');
      return;
    }
    if (!captchaToken) {
      setError('Please complete the captcha.');
      return;
    }

    setIsSubmitting(true);
    try {
      await register({ email, password, captchaToken });
      navigate('/verify-email');
    } catch (err: any) {
      const status = err.response?.status;
      setError(
        status === 409
          ? 'An account with this email already exists.'
          : err.response?.data?.detail ?? 'Could not create account.'
      );
      captchaRef.current?.reset();
      setCaptchaToken(null);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <AuthLayout>
      <div className="mb-8">
        <h1 className="text-3xl mb-1">Create your account ✨</h1>
        <p className="text-mutedForeground font-body">Free — 10 links a day, no credit card</p>
      </div>

      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <Input
          label="Email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          placeholder="you@example.com"
        />
        <Input
          label="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          minLength={8}
          placeholder="At least 8 characters"
        />
        <div>
          <Input
            label="Confirm password"
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
            placeholder="Re-enter your password"
            className={showMismatch ? '!border-secondary' : ''}
          />
          {showMismatch && <p className="text-xs font-bold text-pink-700 mt-1">Passwords don't match</p>}
        </div>

        <CaptchaWidget ref={captchaRef} onVerify={setCaptchaToken} onExpire={() => setCaptchaToken(null)} />

        {error && (
          <p className="text-sm font-bold text-pink-700 bg-secondary/10 border-2 border-secondary rounded-md px-3 py-2">
            {error}
          </p>
        )}

        <Button
          type="submit"
          disabled={isSubmitting || !captchaToken}
          icon={<ArrowRight size={16} strokeWidth={2.5} />}
          className="justify-center mt-2"
        >
          {isSubmitting ? 'Creating account...' : 'Create account'}
        </Button>
      </form>

      <p className="text-center font-body text-mutedForeground mt-8">
        Already have an account?{' '}
        <Link to="/login" className="text-accent font-bold hover:underline">
          Log in
        </Link>
      </p>
    </AuthLayout>
  );
}