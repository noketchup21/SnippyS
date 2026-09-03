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

export function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const captchaRef = useRef<TurnstileInstance>(null);

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [captchaToken, setCaptchaToken] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!captchaToken) {
      setError('Please complete the captcha.');
      return;
    }

    setIsSubmitting(true);
    try {
      await login({ email, password, captchaToken });
      navigate('/dashboard');
    } catch (err: any) {
      const status = err.response?.status;
      const data = err.response?.data;

      if (status === 429) {
        setError('Too many attempts — please wait a moment and try again.');
      } else if (data?.emailNotVerified) {
        navigate('/verify-email-prompt', { state: { userId: data.userId, email } });
        return;
      } else if (status === 401) {
        setError('Invalid email or password.');
      } else if (status === 403) {
        setError(data?.detail ?? 'This account has been banned.');
      } else {
        setError(data?.detail ?? 'Something went wrong. Please try again.');
      }
      captchaRef.current?.reset();
      setCaptchaToken(null);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <AuthLayout>
      <div className="mb-8">
        <h1 className="text-3xl mb-1">Welcome back 👋</h1>
        <p className="text-mutedForeground font-body">Log in to manage your links</p>
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
        <div>
          <Input
            label="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            placeholder="••••••••"
          />
          <Link to="/forgot-password" className="text-xs font-bold text-accent hover:underline mt-1.5 inline-block">
            Forgot password?
          </Link>
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
          {isSubmitting ? 'Logging in...' : 'Log in'}
        </Button>
      </form>

      <p className="text-center font-body text-mutedForeground mt-8">
        Don't have an account?{' '}
        <Link to="/register" className="text-accent font-bold hover:underline">
          Sign up free
        </Link>
      </p>
    </AuthLayout>
  );
}