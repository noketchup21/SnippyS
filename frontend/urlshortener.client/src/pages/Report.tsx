import { useRef, useState } from 'react';
import type { FormEvent } from 'react';
import { Link } from 'react-router-dom';
import type { TurnstileInstance } from '@marsidev/react-turnstile';
import { reportsApi } from '../api/reports';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { CaptchaWidget } from '../components/ui/CaptchaWidget';
import { Flag, CheckCircle2 } from 'lucide-react';

const REASONS = ['Malicious', 'Phishing', 'Spam', 'Illegal', 'Other'] as const;

export function Report() {
  const captchaRef = useRef<TurnstileInstance>(null);

  const [shortCodeOrUrl, setShortCodeOrUrl] = useState('');
  const [reason, setReason] = useState<typeof REASONS[number]>('Spam');
  const [description, setDescription] = useState('');
  const [captchaToken, setCaptchaToken] = useState<string | null>(null);

  const [submitted, setSubmitted] = useState(false);
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
      await reportsApi.create({ shortCodeOrUrl, reason, description: description || undefined, captchaToken });
      setSubmitted(true);
    } catch (err: any) {
      setError(err.response?.data?.detail ?? 'Could not submit report. Please try again.');
      captchaRef.current?.reset();
      setCaptchaToken(null);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen">
      <Navbar />

      <section className="relative max-w-2xl mx-auto px-6 pt-12 pb-24">
        <div className="absolute top-0 right-10 w-64 h-64 bg-secondary/15 rounded-full -z-10" aria-hidden="true" />

        <div className="text-center mb-8">
          <div className="w-14 h-14 rounded-full bg-secondary/15 flex items-center justify-center mx-auto mb-4">
            <Flag size={22} className="text-secondary" />
          </div>
          <h1 className="text-3xl mb-2">Report a link</h1>
          <p className="font-body text-mutedForeground">
            Help keep the community safe — let us know about suspicious or malicious short links.
          </p>
        </div>

        <Card shadowColor="pink">
          {submitted ? (
            <div className="text-center py-6">
              <CheckCircle2 size={32} className="text-quaternary mx-auto mb-3" />
              <h2 className="text-xl mb-2">Report submitted</h2>
              <p className="font-body text-mutedForeground mb-6">
                Thanks for helping keep the platform safe. Our team will review this link shortly.
              </p>
              <div className="flex items-center justify-center gap-4">
                <button
                  onClick={() => {
                    setSubmitted(false);
                    setShortCodeOrUrl('');
                    setDescription('');
                    captchaRef.current?.reset();
                    setCaptchaToken(null);
                  }}
                  className="text-sm font-bold text-accent hover:underline"
                >
                  Report another link
                </button>
                <Link to="/" className="text-sm font-bold text-mutedForeground hover:text-accent">
                  Back home
                </Link>
              </div>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
              <div>
                <label className="text-xs font-bold uppercase tracking-wide text-mutedForeground block mb-1.5">
                  Short link or code
                </label>
                <input
                  value={shortCodeOrUrl}
                  onChange={(e) => setShortCodeOrUrl(e.target.value)}
                  placeholder="e.g. short.ly/abc123 or just abc123"
                  required
                  className="w-full bg-white border-2 border-border rounded-md px-4 py-3 font-body focus:outline-none focus:border-accent focus:shadow-[4px_4px_0px_0px_#8B5CF6] transition-all"
                />
              </div>

              <div>
                <label className="text-xs font-bold uppercase tracking-wide text-mutedForeground block mb-1.5">
                  Reason
                </label>
                <select
                  value={reason}
                  onChange={(e) => setReason(e.target.value as typeof reason)}
                  className="w-full bg-white border-2 border-border rounded-md px-3 py-2.5 font-body focus:outline-none focus:border-accent"
                >
                  {REASONS.map((r) => <option key={r} value={r}>{r}</option>)}
                </select>
              </div>

              <div>
                <label className="text-xs font-bold uppercase tracking-wide text-mutedForeground block mb-1.5">
                  Details <span className="normal-case font-normal text-mutedForeground/70">(optional)</span>
                </label>
                <textarea
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  placeholder="Anything else we should know?"
                  rows={3}
                  className="w-full bg-white border-2 border-border rounded-md px-4 py-3 font-body focus:outline-none focus:border-accent resize-none"
                />
              </div>

              <CaptchaWidget ref={captchaRef} onVerify={setCaptchaToken} onExpire={() => setCaptchaToken(null)} />

              {error && (
                <p className="text-sm font-bold text-pink-700 bg-secondary/10 border-2 border-secondary rounded-md px-3 py-2">
                  {error}
                </p>
              )}

              <Button
                type="submit"
                variant="secondary"
                disabled={isSubmitting || !captchaToken}
                className="justify-center"
                icon={<Flag size={16} strokeWidth={2.5} />}
              >
                {isSubmitting ? 'Submitting...' : 'Submit report'}
              </Button>
            </form>
          )}
        </Card>
      </section>
    </div>
  );
}