import type { ReactNode } from 'react';
import { Link } from 'react-router-dom';
import { Link2, ShieldCheck, Zap, BarChart3 } from 'lucide-react';

export function AuthLayout({ children }: { children: ReactNode }) {
  return (
    <div className="min-h-screen flex">
      {/* Left branded panel — hidden on mobile */}
      <div className="hidden lg:flex lg:w-[45%] relative bg-accent overflow-hidden flex-col justify-between p-12">
        <div className="absolute top-10 -right-16 w-64 h-64 bg-white/10 rounded-full" aria-hidden="true" />
        <div className="absolute bottom-0 -left-10 w-72 h-72 bg-secondary/30 rounded-full" aria-hidden="true" />
        <div className="absolute top-1/2 right-1/4 w-20 h-20 border-4 border-white/20 rounded-full animate-wiggle" style={{ animationIterationCount: 'infinite', animationDuration: '4s' }} aria-hidden="true" />
        <div className="absolute bottom-1/3 left-1/4 w-10 h-10 bg-tertiary rotate-45 rounded-md" aria-hidden="true" />

        <Link to="/" className="relative z-10 flex items-center gap-2 font-heading font-extrabold text-xl text-white">
          <span className="flex items-center justify-center w-9 h-9 rounded-full bg-white text-accent">
            <Link2 size={18} strokeWidth={2.5} />
          </span>
          Snippy
        </Link>

        <div className="relative z-10">
          <h2 className="text-3xl text-white mb-4 leading-tight">
            Short links.<br />Big picture.
          </h2>
          <div className="flex flex-col gap-4">
            <FeaturePoint icon={<ShieldCheck size={16} />} text="Every link scanned for safety" />
            <FeaturePoint icon={<BarChart3 size={16} />} text="Real-time click analytics" />
            <FeaturePoint icon={<Zap size={16} />} text="Custom aliases & QR codes" />
          </div>
        </div>

        <p className="relative z-10 text-white/60 text-xs font-body">
          © {new Date().getFullYear()} Snippy. All links, tracked responsibly.
        </p>
      </div>

      {/* Right form panel */}
      <div className="flex-1 flex items-center justify-center bg-dot-grid relative px-4 py-10">
        <div className="absolute top-16 right-16 w-32 h-32 bg-tertiary/25 rounded-full -z-10 lg:hidden" aria-hidden="true" />
        <div className="w-full max-w-md">
          <Link to="/" className="lg:hidden flex items-center justify-center gap-2 font-heading font-extrabold text-xl mb-8">
            <span className="flex items-center justify-center w-9 h-9 rounded-full bg-accent text-white">
              <Link2 size={18} strokeWidth={2.5} />
            </span>
            Snippy
          </Link>
          {children}
        </div>
      </div>
    </div>
  );
}

function FeaturePoint({ icon, text }: { icon: ReactNode; text: string }) {
  return (
    <div className="flex items-center gap-3">
      <span className="flex items-center justify-center w-8 h-8 rounded-full bg-white/15 text-white shrink-0">
        {icon}
      </span>
      <span className="font-body text-white/90 text-sm">{text}</span>
    </div>
  );
}