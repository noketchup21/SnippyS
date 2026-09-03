import { useRef, useState } from 'react';
import { useMemo } from 'react';
import { Link2, Zap, ShieldCheck, ChevronDown, Sparkles } from 'lucide-react';

const METEORS = Array.from({ length: 24 }, (_, i) => ({
  id: i,
  left: Math.random() * 100,
  delay: Math.random() * 6,
  duration: 3 + Math.random() * 3,
  opacity: 0.15 + Math.random() * 0.35,
}));

export function HeroSplash({ onScrollToShorten }: { onScrollToShorten: () => void }) {
  const tiltRef = useRef<HTMLDivElement>(null);
  const [tilt, setTilt] = useState({ x: 0, y: 0 });

  const meteors = useMemo(() => METEORS, []);

  const handleMouseMove = (e: React.MouseEvent<HTMLDivElement>) => {
    const rect = tiltRef.current?.getBoundingClientRect();
    if (!rect) return;
    const px = (e.clientX - rect.left) / rect.width - 0.5;
    const py = (e.clientY - rect.top) / rect.height - 0.5;
    setTilt({ x: py * -22, y: px * 22 });
  };

  const resetTilt = () => setTilt({ x: 0, y: 0 });

  return (
    <section className="relative min-h-screen flex flex-col overflow-hidden bg-gradient-to-b from-[#1a1533] via-[#221a3d] to-[#1a1533]">
      {/* Falling meteor lines */}
      <div className="absolute inset-0 -z-0 overflow-hidden" aria-hidden="true">
        {meteors.map((m) => (
          <span
            key={m.id}
            className="absolute top-0 w-px h-24 bg-gradient-to-b from-transparent via-accent to-transparent animate-meteor"
            style={{
              left: `${m.left}%`,
              animationDelay: `${m.delay}s`,
              animationDuration: `${m.duration}s`,
              opacity: m.opacity,
            }}
          />
        ))}
      </div>

      {/* Ambient glows */}
      <div className="absolute top-20 -left-20 w-96 h-96 bg-accent/20 rounded-full blur-3xl -z-0" aria-hidden="true" />
      <div className="absolute bottom-0 right-0 w-96 h-96 bg-secondary/15 rounded-full blur-3xl -z-0" aria-hidden="true" />

      <div className="flex-1 max-w-6xl mx-auto px-6 w-full grid lg:grid-cols-2 gap-10 items-center relative z-10">
        {/* Left: copy */}
        <div className="text-center lg:text-left">
          <span className="inline-flex items-center gap-1.5 bg-white/10 border border-white/20 rounded-full px-4 py-1.5 text-xs font-bold text-white/90 mb-6 backdrop-blur-sm">
            <Sparkles size={13} className="text-tertiary" />
            AI-powered, scanned safe, built for speed
          </span>

          <h1 className="text-4xl md:text-6xl leading-tight font-heading font-extrabold mb-5">
            <span className="text-white">Every link,</span>
            <br />
            <span className="bg-gradient-to-r from-tertiary via-secondary to-accent bg-clip-text text-transparent">
              reimagined.
            </span>
          </h1>
          <p className="text-lg text-white/60 font-body max-w-md mx-auto lg:mx-0 mb-8">
            Shorten, protect, and understand every link you share — with AI summaries,
            malware scanning, and analytics built in.
          </p>

          <div className="flex flex-col sm:flex-row items-center gap-3 justify-center lg:justify-start mb-12">
            <button
              onClick={onScrollToShorten}
              className="inline-flex items-center gap-2 rounded-full border-2 border-white bg-white text-foreground font-heading font-bold px-7 py-3.5 shadow-[4px_4px_0px_0px_rgba(139,92,246,0.6)] hover:shadow-[6px_6px_0px_0px_rgba(139,92,246,0.6)] hover:-translate-x-0.5 hover:-translate-y-0.5 transition-all"
            >
              Start shortening now <Zap size={16} strokeWidth={2.5} />
            </button>
            <button
              onClick={onScrollToShorten}
              className="inline-flex items-center gap-1.5 text-white/70 hover:text-white font-bold text-sm transition-colors"
            >
              or just scroll down <ChevronDown size={15} />
            </button>
          </div>

          {/* Stat chips */}
          <div className="grid grid-cols-3 gap-3 max-w-md mx-auto lg:mx-0">
            <StatChip icon={<Link2 size={15} />} value="Instant" label="link creation" />
            <StatChip icon={<ShieldCheck size={15} />} value="100%" label="scanned safe" />
            <StatChip icon={<Sparkles size={15} />} value="AI" label="summaries" />
          </div>
        </div>

        {/* Right: interactive 3D chain-link model */}
        <div
          ref={tiltRef}
          onMouseMove={handleMouseMove}
          onMouseLeave={resetTilt}
          className="relative h-[340px] md:h-[420px] flex items-center justify-center [perspective:1000px]"
        >
          <div
            className="relative animate-float transition-transform duration-200 ease-out [transform-style:preserve-3d]"
            style={{ transform: `rotateX(${tilt.x}deg) rotateY(${tilt.y}deg)` }}
          >
            {/* Chain link — two interlocking rounded rings */}
            <div className="relative w-56 h-56 md:w-72 md:h-72 [transform-style:preserve-3d]">
              <div
                className="absolute top-1/2 left-1/2 w-32 h-52 md:w-40 md:h-64 -translate-x-[65%] -translate-y-1/2 rounded-full border-[14px] md:border-[18px] border-accent shadow-[0_0_60px_rgba(139,92,246,0.5)]"
                style={{ transform: 'rotate(-15deg) translateZ(20px)' }}
              />
              <div
                className="absolute top-1/2 left-1/2 w-32 h-52 md:w-40 md:h-64 -translate-x-[35%] -translate-y-1/2 rounded-full border-[14px] md:border-[18px] border-secondary shadow-[0_0_60px_rgba(244,114,182,0.5)]"
                style={{ transform: 'rotate(15deg) translateZ(-20px)' }}
              />

              {/* Orbiting accents */}
              <div className="absolute top-1/2 left-1/2 w-4 h-4 -ml-2 -mt-2 bg-tertiary rounded-full shadow-[0_0_20px_rgba(251,191,36,0.8)] animate-orbit" aria-hidden="true" />
              <div className="absolute top-1/2 left-1/2 w-3 h-3 -ml-1.5 -mt-1.5 bg-quaternary rounded-full shadow-[0_0_16px_rgba(52,211,153,0.8)] animate-orbit" style={{ animationDuration: '11s', animationDirection: 'reverse' }} aria-hidden="true" />
            </div>
          </div>

          {/* Floating badges around the model */}
          <div className="absolute top-8 right-4 md:right-10 bg-white/10 backdrop-blur-sm border border-white/20 rounded-2xl px-4 py-2.5 animate-float" style={{ animationDelay: '0.5s', animationDuration: '5s' }}>
            <p className="text-white text-xs font-bold flex items-center gap-1.5"><ShieldCheck size={13} className="text-quaternary" /> Verified safe</p>
          </div>
          <div className="absolute bottom-10 left-2 md:left-6 bg-white/10 backdrop-blur-sm border border-white/20 rounded-2xl px-4 py-2.5 animate-float" style={{ animationDelay: '1.2s', animationDuration: '4.5s' }}>
            <p className="text-white text-xs font-bold flex items-center gap-1.5"><Sparkles size={13} className="text-tertiary" /> AI summarized</p>
          </div>
        </div>
      </div>

      <button
        onClick={onScrollToShorten}
        className="relative z-10 flex flex-col items-center gap-1 pb-8 text-white/50 hover:text-white transition-colors mx-auto"
        aria-label="Scroll to shorten a link"
      >
        <ChevronDown size={20} className="animate-bounce" />
      </button>
    </section>
  );
}

function StatChip({ icon, value, label }: { icon: React.ReactNode; value: string; label: string }) {
  return (
    <div className="bg-white/5 border border-white/15 rounded-xl px-3 py-3 text-center backdrop-blur-sm">
      <div className="flex items-center justify-center gap-1 text-tertiary mb-1">{icon}</div>
      <p className="font-heading font-extrabold text-white text-sm">{value}</p>
      <p className="text-white/50 text-[10px] font-bold uppercase tracking-wide">{label}</p>
    </div>
  );
}