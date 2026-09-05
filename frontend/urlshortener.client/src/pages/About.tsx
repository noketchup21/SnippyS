import { Navbar } from '../components/Navbar';
import { Card } from '../components/ui/Card';
import { Code2, Server, Smartphone, GraduationCap, ExternalLink } from 'lucide-react';

export function About() {
  return (
    <div className="min-h-screen">
      <Navbar />
      <section className="relative max-w-4xl mx-auto px-6 pt-12 pb-24">
        <div className="absolute top-0 right-10 w-64 h-64 bg-quaternary/20 rounded-full -z-10" aria-hidden="true" />
        <div className="absolute top-40 -left-10 w-40 h-40 bg-tertiary/25 rounded-full -z-10" aria-hidden="true" />

        <div className="text-center mb-12">
          <div className="w-16 h-16 rounded-full bg-accent text-white flex items-center justify-center mx-auto mb-4 font-heading font-extrabold text-2xl border-2 border-foreground shadow-pop">
            N
          </div>
          <h1 className="text-3xl mb-2">About this project</h1>
          <p className="font-body text-mutedForeground max-w-lg mx-auto">
            A full-stack URL shortener, built as a hands-on way to learn backend architecture — from
            the database up to the UI.
          </p>
        </div>

        <div className="grid md:grid-cols-3 gap-6 mb-10">
          <Card>
            <div className="w-10 h-10 rounded-full bg-accent/10 text-accent flex items-center justify-center mb-3">
              <GraduationCap size={20} />
            </div>
            <h3 className="font-heading font-bold text-base mb-1">Who built it</h3>
            <p className="font-body text-mutedForeground text-sm">
              A software engineering student at FPT University, currently focused on backend development.
            </p>
          </Card>
          <Card>
            <div className="w-10 h-10 rounded-full bg-secondary/10 text-secondary flex items-center justify-center mb-3">
              <Server size={20} />
            </div>
            <h3 className="font-heading font-bold text-base mb-1">Main focus</h3>
            <p className="font-body text-mutedForeground text-sm">
              ASP.NET Core backend development, with basic experience in AWS for cloud fundamentals.
            </p>
          </Card>
          <Card>
            <div className="w-10 h-10 rounded-full bg-tertiary/20 text-amber-700 flex items-center justify-center mb-3">
              <Smartphone size={20} />
            </div>
            <h3 className="font-heading font-bold text-base mb-1">Also learning</h3>
            <p className="font-body text-mutedForeground text-sm">
              Frontend and mobile development at a basic level, with React and Flutter.
            </p>
          </Card>
        </div>

        <Card shadowColor="pink" className="!hover:rotate-0 !hover:scale-100 mb-10">
          <div className="flex items-center gap-2 mb-3">
            <Code2 size={18} className="text-accent" />
            <h2 className="text-lg">Why this project</h2>
          </div>
          <p className="font-body text-mutedForeground text-sm leading-relaxed mb-3">
            This URL shortener started as a way to practice building a real backend end-to-end with
            ASP.NET Core and .NET Aspire — covering authentication, background workers, third-party API
            integrations, and service-to-service architecture — rather than just following a single
            tutorial.
          </p>
          <p className="font-body text-mutedForeground text-sm leading-relaxed">
            Along the way it grew to include malware scanning, AI-generated link summaries, subscription
            billing, and an admin moderation panel — each one a deliberate exercise in adding a new piece
            of real-world backend functionality and connecting it to a working frontend.
          </p>
        </Card>

        <div className="text-center">
          <a
            href="https://github.com/noketchup21"
            target="_blank"
            rel="noreferrer"
            className="inline-flex items-center gap-2 rounded-full border-2 border-foreground bg-foreground text-white font-heading font-bold px-6 py-3 shadow-pop hover:shadow-pop-hover hover:-translate-x-0.5 hover:-translate-y-0.5 transition-all"
          >
            <ExternalLink size={18} /> View on GitHub
          </a>
        </div>
      </section>
    </div>
  );
}