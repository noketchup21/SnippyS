import { Link } from 'react-router-dom';
import { Link2, ExternalLink,ShieldCheck, Sparkles} from 'lucide-react';

export function Footer() {
  return (
    <footer className="relative bg-foreground text-white overflow-hidden">
      <div className="absolute -top-16 -right-16 w-64 h-64 bg-accent/20 rounded-full" aria-hidden="true" />
      <div className="absolute bottom-0 left-1/4 w-32 h-32 bg-secondary/15 rounded-full" aria-hidden="true" />
      <div className="absolute top-10 left-10 w-16 h-16 border-4 border-tertiary/20 rounded-full hidden md:block" aria-hidden="true" />

      <div className="max-w-6xl mx-auto px-6 pt-16 pb-8 relative">
        {/* CTA strip */}
        <div className="bg-white/5 border-2 border-white/10 rounded-2xl px-6 py-8 md:px-10 md:py-10 mb-14 flex flex-col md:flex-row items-center justify-between gap-6">
          <div className="text-center md:text-left">
            <span className="inline-flex items-center gap-1.5 text-xs font-bold uppercase tracking-widest text-tertiary mb-2">
              <Sparkles size={13} /> Free to start
            </span>
            <h3 className="text-2xl font-heading font-extrabold">Ready to shorten your first link?</h3>
          </div>
          <Link to="/register">
            <button className="inline-flex items-center gap-2 rounded-full border-2 border-white bg-white text-foreground font-heading font-bold px-6 py-3 shadow-[4px_4px_0px_0px_rgba(255,255,255,0.3)] hover:shadow-[6px_6px_0px_0px_rgba(255,255,255,0.3)] hover:-translate-x-0.5 hover:-translate-y-0.5 transition-all whitespace-nowrap">
              Get started free
            </button>
          </Link>
        </div>

        {/* Main grid */}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-8 mb-12">
          <div className="col-span-2 md:col-span-1">
            <Link to="/" className="flex items-center gap-2 font-heading font-extrabold text-lg mb-3">
              <span className="flex items-center justify-center w-8 h-8 rounded-full bg-accent">
                <Link2 size={15} strokeWidth={2.5} />
              </span>
              Snippy
            </Link>
            <p className="text-sm font-body text-white/60 max-w-[220px]">
              Short links, scanned safe, tracked simply.
            </p>
          </div>

          <FooterColumn title="Product">
            <FooterLink to="/" label="Shorten a link" />
            <FooterLink to="/account/upgrade" label="Plus features" />
            <FooterLink to="/dashboard" label="Dashboard" />
          </FooterColumn>

          <FooterColumn title="Company">
            <FooterLink to="/about" label="About" />
            <FooterLink to="/faq" label="FAQ" />
            <FooterLink to="/terms" label="Terms of service" />
          </FooterColumn>

          <FooterColumn title="Trust & safety">
            <FooterLink to="/report" label="Report a link" />
            <div className="flex items-center gap-1.5 text-sm text-white/60 mt-1">
              <ShieldCheck size={13} className="text-quaternary shrink-0" />
              Every link scanned
            </div>
          </FooterColumn>
        </div>

        {/* Bottom bar */}
        <div className="flex flex-col sm:flex-row items-center justify-between gap-4 pt-6 border-t border-white/10">
          <p className="text-xs font-body text-white/40">
            © {new Date().getFullYear()} Snippy. Built as a learning project.
          </p>
          <a
            href="https://github.com/noketchup21"
            target="_blank"
            rel="noreferrer"
            className="flex items-center gap-1.5 text-xs font-bold text-white/60 hover:text-white transition-colors"
          >
            <ExternalLink size={13} /> GitHub
          </a>
        </div>
      </div>
    </footer>
  );
}

function FooterColumn({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div>
      <p className="text-xs font-bold uppercase tracking-widest text-white/40 mb-3">{title}</p>
      <div className="flex flex-col gap-2.5">{children}</div>
    </div>
  );
}

function FooterLink({ to, label }: { to: string; label: string }) {
  return (
    <Link to={to} className="text-sm font-body text-white/70 hover:text-white transition-colors w-fit">
      {label}
    </Link>
  );
}