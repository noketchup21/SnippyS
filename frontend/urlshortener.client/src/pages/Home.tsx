import { useEffect, useRef, useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { linksApi } from '../api/links';
import { buildShortUrl } from '../api/config';
import { useAuth } from '../auth/AuthContext';
import { useRevealOnScroll } from '../hooks/useRevealOnScroll';
import { Navbar } from '../components/Navbar';
import { HeroSplash } from '../components/HeroSplash';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Card } from '../components/ui/Card';
import { CopyableCode } from '../components/ui/CopyableCode';
import { Footer } from '../components/Footer';
import {
  ArrowRight, Lock, QrCode, Sparkles, Zap, ShieldCheck,
  BarChart3, Link2, MousePointerClick, Layers, Bot, ChevronDown,
} from 'lucide-react';

interface ShortenedResult {
  id: string;
  shortCode: string;
  originalUrl: string;
}

const EXAMPLE_URLS = [
  'https://your-portfolio.com/projects/2026/design-system-v2',
  'https://shop.example.com/products/electronics/laptop?ref=email',
  'https://docs.example.com/api/v3/reference/authentication',
];

export function Home() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const isPlus = user?.tier === 'Plus';
  const shortenSectionRef = useRef<HTMLDivElement>(null);

  const [url, setUrl] = useState('');
  const [customAlias, setCustomAlias] = useState('');
  const [password, setPassword] = useState('');
  const [showPlusOptions, setShowPlusOptions] = useState(false);
  const [placeholderIndex, setPlaceholderIndex] = useState(0);

  const [result, setResult] = useState<ShortenedResult | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [upsellMessage, setUpsellMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const howItWorks = useRevealOnScroll();
  const features = useRevealOnScroll();

  const scrollToShorten = () => {
    shortenSectionRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const scrollToNext = () => {
    document.getElementById('below-fold')?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    const interval = setInterval(() => {
      setPlaceholderIndex((i) => (i + 1) % EXAMPLE_URLS.length);
    }, 3200);
    return () => clearInterval(interval);
  }, []);

  const goToUpgrade = () => navigate(user ? '/account/upgrade' : '/register');

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setUpsellMessage(null);
    setResult(null);
    setIsSubmitting(true);

    try {
      const res = await linksApi.create({
        originalUrl: url,
        customAlias: customAlias || undefined,
        password: password || undefined,
      });
      setResult({ id: res.data.id, shortCode: res.data.shortCode, originalUrl: res.data.originalUrl });
      setUrl('');
      setCustomAlias('');
      setPassword('');
    } catch (err: any) {
      const status = err.response?.status;
      const detail = err.response?.data?.detail;

      if (status === 429) {
        setUpsellMessage(
          user
            ? "You've hit your daily limit of 10 links. Upgrade to Plus for unlimited shortening."
            : "You've used your 2 free links. Create a free account for 10 links a day."
        );
      } else if (status === 403) {
        setUpsellMessage(detail ?? 'This feature requires a Plus subscription.');
      } else if (status === 422) {
        setError(detail ?? 'This URL was flagged as unsafe and could not be shortened.');
      } else if (status === 409) {
        setError('That custom alias is already taken — try another.');
      } else {
        setError(detail ?? 'Something went wrong. Please try again.');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen">
      <Navbar transparentUntilScroll />

      {/* ============ DARK SPLASH HERO ============ */}
      <HeroSplash onScrollToShorten={scrollToShorten} />

      {/* ============ SHORTEN SECTION — full viewport height ============ */}
      <section
        ref={shortenSectionRef}
        className="relative min-h-screen flex flex-col items-center justify-center max-w-6xl mx-auto px-6 py-20"
      >
        <div className="absolute top-0 right-0 w-72 h-72 md:w-[28rem] md:h-[28rem] bg-tertiary/25 rounded-full -z-10 animate-pulse" style={{ animationDuration: '4s' }} aria-hidden="true" />
        <div className="absolute bottom-0 -left-16 w-64 h-64 bg-quaternary/20 rounded-full -z-10" aria-hidden="true" />
        <div className="absolute top-1/4 left-[8%] w-28 h-28 bg-dot-grid -z-10 hidden lg:block" aria-hidden="true" />

        <div className="text-center max-w-3xl mx-auto mb-10">
          <span className="inline-flex items-center gap-1.5 bg-white border-2 border-foreground rounded-full px-4 py-1.5 text-xs font-bold shadow-pop mb-6 animate-pop-in">
            <Sparkles size={13} className="text-accent" />
            Scanned safe, AI-summarized, on every Plus link
          </span>

          <h2 className="text-3xl md:text-5xl leading-tight mb-4">
            Paste a link.{' '}
            <span className="relative inline-block">
              <span className="relative z-10 text-accent">Get magic back.</span>
              <span className="absolute inset-x-0 bottom-1 h-3 md:h-4 bg-tertiary/50 -z-0 rounded" aria-hidden="true" />
            </span>
          </h2>
          <p className="text-lg md:text-xl font-body text-mutedForeground max-w-xl mx-auto">
            Free to start, built to scale with you.
          </p>
        </div>

        <Card className="w-full max-w-2xl animate-pop-in" shadowColor="pink">
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            <div className="flex flex-col sm:flex-row gap-3">
              <input
                type="url"
                required
                value={url}
                onChange={(e) => setUrl(e.target.value)}
                placeholder={url ? '' : EXAMPLE_URLS[placeholderIndex]}
                className="flex-1 bg-white border-2 border-border rounded-md px-4 py-3 font-body focus:outline-none focus:border-accent focus:shadow-[4px_4px_0px_0px_#8B5CF6] transition-all"
              />
              <Button type="submit" disabled={isSubmitting} icon={<ArrowRight size={16} strokeWidth={2.5} />}>
                {isSubmitting ? 'Shortening...' : 'Shorten'}
              </Button>
            </div>

            <button
              type="button"
              onClick={() => setShowPlusOptions(!showPlusOptions)}
              className="text-sm font-bold text-mutedForeground hover:text-accent self-start flex items-center gap-1"
            >
              <Sparkles size={14} /> Custom alias & password protection
            </button>

            {showPlusOptions && (
              <div className="flex flex-col sm:flex-row gap-3 pt-2 border-t-2 border-dashed border-border">
                <div className="flex-1 relative">
                  <Input
                    label="Custom alias"
                    placeholder="my-cool-link"
                    value={customAlias}
                    onChange={(e) => setCustomAlias(e.target.value)}
                    disabled={!isPlus}
                  />
                  {!isPlus && <PlusLockNote onClick={goToUpgrade} />}
                </div>
                <div className="flex-1 relative">
                  <Input
                    label="Password"
                    type="password"
                    placeholder="Optional"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    disabled={!isPlus}
                  />
                  {!isPlus && <PlusLockNote onClick={goToUpgrade} />}
                </div>
              </div>
            )}
          </form>

          {error && (
            <p className="mt-4 text-sm font-bold text-pink-700 bg-secondary/10 border-2 border-secondary rounded-md px-3 py-2 animate-pop-in">
              {error}
            </p>
          )}

          {upsellMessage && (
            <div className="mt-4 bg-tertiary/10 border-2 border-tertiary rounded-md px-4 py-3 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 animate-pop-in">
              <p className="font-bold text-sm">{upsellMessage}</p>
              <Link to={user ? '/account/upgrade' : '/register'}>
                <Button variant="secondary" className="whitespace-nowrap">
                  {user ? 'Upgrade to Plus' : 'Sign up free'}
                </Button>
              </Link>
            </div>
          )}

          {result && (
            <div className="mt-6 pt-6 border-t-2 border-dashed border-border animate-pop-in">
              <p className="text-xs font-bold uppercase tracking-wide text-mutedForeground mb-2">
                Your short link
              </p>
              <div className="flex flex-wrap items-center gap-3">
                <CopyableCode value={buildShortUrl(result.shortCode)} />
                {isPlus ? (
                  <a
                    href={linksApi.getQrCode(result.id)}
                    target="_blank"
                    rel="noreferrer"
                    className="inline-flex items-center gap-1 text-sm font-bold text-accent hover:underline"
                  >
                    <QrCode size={16} /> QR code
                  </a>
                ) : (
                  <button
                    type="button"
                    onClick={goToUpgrade}
                    className="inline-flex items-center gap-1 text-sm font-bold text-mutedForeground hover:text-accent"
                  >
                    <QrCode size={16} /> <Lock size={12} /> QR code (Plus)
                  </button>
                )}
              </div>
              <p className="text-sm text-mutedForeground font-body mt-2 truncate">
                → {result.originalUrl}
              </p>
            </div>
          )}
        </Card>

        {!user && (
          <p className="text-center text-sm font-body text-mutedForeground mt-4">
            No account needed for your first 2 links.{' '}
            <Link to="/register" className="font-bold text-accent hover:underline">Sign up</Link>{' '}
            for 10 a day, free.
          </p>
        )}

        <button
          onClick={scrollToNext}
          className="flex flex-col items-center gap-1 mt-16 text-mutedForeground hover:text-accent transition-colors group"
          aria-label="Scroll to learn more"
        >
          <span className="text-xs font-bold uppercase tracking-wide">See how it works</span>
          <ChevronDown size={20} className="animate-bounce" />
        </button>
      </section>

      {/* ============ BELOW THE FOLD ============ */}
      <div id="below-fold">
        <section className="relative bg-white border-y-2 border-foreground">
          <div className="max-w-6xl mx-auto px-6 py-16 md:py-20">

            <div ref={howItWorks.ref} className={`transition-all duration-700 ${howItWorks.className}`}>
              <div className="text-center max-w-lg mx-auto mb-14">
                <span className="inline-block text-xs font-bold uppercase tracking-widest text-accent mb-2">
                  The process
                </span>
                <h2 className="text-2xl md:text-3xl mb-2">Three steps. No hassle.</h2>
                <p className="font-body text-mutedForeground">From long link to tracked short link in seconds.</p>
              </div>

              <div className="grid md:grid-cols-3 gap-6 mb-20">
                <StepCard step="1" icon={<Link2 size={22} />} color="bg-accent" title="Paste your link" description="Drop in any long, ugly URL — we'll scan it for safety before anything else." />
                <StepCard step="2" icon={<Zap size={22} />} color="bg-secondary" title="Get your short link" description="Instantly receive a clean, shareable link — plus a QR code if you're on Plus." />
                <StepCard step="3" icon={<BarChart3 size={22} />} color="bg-tertiary" title="Track every click" description="Watch clicks roll in on your dashboard — no spreadsheets required." />
              </div>
            </div>

            <div className="max-w-2xl mx-auto mb-20">
              <div className="text-center mb-6">
                <span className="inline-block text-xs font-bold uppercase tracking-widest text-accent mb-2">New with Plus</span>
                <h3 className="text-xl md:text-2xl mb-2">See what's behind every link</h3>
                <p className="font-body text-mutedForeground text-sm">AI reads the page so your visitors don't have to guess.</p>
              </div>
              <div className="bg-accent border-2 border-foreground rounded-2xl p-5 shadow-pop-pink text-left">
                <div className="text-white text-xs font-bold uppercase tracking-wide mb-2 flex items-center gap-1.5">
                  <Bot size={14} /> AI Summary
                </div>
                <p className="text-white text-sm mb-3 leading-relaxed">
                  "A beginner-friendly guide to building REST APIs with ASP.NET Core, covering routing, middleware, and authentication."
                </p>
                <div className="flex flex-wrap gap-2">
                  <span className="bg-white/20 text-white text-xs font-bold px-3 py-1 rounded-full">📖 ~6 min read</span>
                  <span className="bg-white/20 text-white text-xs font-bold px-3 py-1 rounded-full">API Development</span>
                  <span className="bg-white/20 text-white text-xs font-bold px-3 py-1 rounded-full">ASP.NET Core</span>
                </div>
              </div>
            </div>

            <div ref={features.ref} className={`transition-all duration-700 ${features.className}`}>
              <div className="text-center max-w-lg mx-auto mb-12">
                <span className="inline-block text-xs font-bold uppercase tracking-widest text-secondary mb-2">
                  What's included
                </span>
                <h2 className="text-2xl md:text-3xl mb-2">Everything you need</h2>
                <p className="font-body text-mutedForeground">Free features that already go further than most.</p>
              </div>

              <div className="grid sm:grid-cols-2 lg:grid-cols-5 gap-5">
                <FeatureCard icon={<ShieldCheck size={20} />} title="Malware scanning" description="Every link checked against VirusTotal before it ever goes live." />
                <FeatureCard icon={<MousePointerClick size={20} />} title="Click analytics" description="See exactly how your links perform, day by day." />
                <FeatureCard icon={<QrCode size={20} />} title="QR codes" description="Generate a scannable code for any link — great for print." plusOnly onClick={!isPlus ? goToUpgrade : undefined} />
                <FeatureCard icon={<Layers size={20} />} title="Bulk shorten" description="Paste a whole list of URLs and shorten them all at once." plusOnly onClick={!isPlus ? goToUpgrade : undefined} />
                <FeatureCard icon={<Bot size={20} />} title="AI link summaries" description="Know what's behind a link before you click — auto-generated by AI." plusOnly onClick={!isPlus ? goToUpgrade : undefined} />
              </div>

              {!isPlus && (
                <div className="text-center mt-12">
                  <Link to={user ? '/account/upgrade' : '/register'}>
                    <Button icon={<Sparkles size={16} strokeWidth={2.5} />}>
                      {user ? 'Upgrade to Plus — $3.99/mo' : 'Get started free'}
                    </Button>
                  </Link>
                </div>
              )}
            </div>

          </div>
        </section>

        <Footer />
      </div>
    </div>
  );
}

function StepCard({ step, icon, color, title, description }: {
  step: string; icon: React.ReactNode; color: string; title: string; description: string;
}) {
  return (
    <Card className="relative pt-9">
      <div className={`absolute -top-5 left-6 w-10 h-10 rounded-full ${color} border-2 border-foreground flex items-center justify-center text-white font-heading font-extrabold`}>
        {step}
      </div>
      <div className="flex items-center gap-2 mb-2 text-accent">{icon}</div>
      <h3 className="text-lg mb-1">{title}</h3>
      <p className="font-body text-mutedForeground text-sm">{description}</p>
    </Card>
  );
}

function FeatureCard({ icon, title, description, plusOnly, onClick }: {
  icon: React.ReactNode; title: string; description: string; plusOnly?: boolean; onClick?: () => void;
}) {
  const Wrapper = onClick ? 'button' : 'div';
  return (
    <Wrapper
      onClick={onClick}
      className="border-2 border-foreground rounded-lg p-5 bg-background hover:-translate-y-1 hover:shadow-pop transition-all duration-300 ease-bounce relative text-left w-full"
    >
      {plusOnly && (
        <span className="absolute top-3 right-3 text-[10px] font-bold bg-tertiary border-2 border-foreground rounded-full px-2 py-0.5">
          PLUS
        </span>
      )}
      <div className="w-10 h-10 rounded-full bg-accent/10 text-accent flex items-center justify-center mb-3">
        {icon}
      </div>
      <h3 className="font-heading font-bold text-base mb-1">{title}</h3>
      <p className="font-body text-mutedForeground text-sm">{description}</p>
    </Wrapper>
  );
}

function PlusLockNote({ onClick }: { onClick: () => void }) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="absolute inset-0 top-6 flex items-center justify-center bg-white/70 rounded-md cursor-pointer group"
    >
      <span className="inline-flex items-center gap-1 text-xs font-bold bg-tertiary border-2 border-foreground rounded-full px-3 py-1 group-hover:scale-105 transition-transform">
        <Lock size={12} /> Plus only — click to upgrade
      </span>
    </button>
  );
}