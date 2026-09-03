import { Navbar } from '../components/Navbar';
import { Card } from '../components/ui/Card';
import { FileText } from 'lucide-react';

export function Terms() {
  return (
    <div className="min-h-screen">
      <Navbar />
      <section className="max-w-3xl mx-auto px-6 pt-12 pb-24">
        <div className="text-center mb-10">
          <div className="w-14 h-14 rounded-full bg-accent/10 flex items-center justify-center mx-auto mb-4">
            <FileText size={22} className="text-accent" />
          </div>
          <h1 className="text-3xl mb-2">Terms of Service</h1>
          <p className="font-body text-mutedForeground text-sm">Last updated: August 2026</p>
        </div>

        <Card className="!hover:rotate-0 !hover:scale-100">
          <div className="flex flex-col gap-6 font-body text-sm leading-relaxed text-foreground">
            <Section title="1. Acceptance of terms">
              By creating an account or using this URL shortening service ("the Service"), you agree to
              these Terms of Service. If you do not agree, please do not use the Service.
            </Section>

            <Section title="2. What the Service does">
              The Service lets you shorten URLs, view click analytics, generate QR codes, and optionally
              password-protect or customize your shortened links. Every submitted URL is scanned for
              known malware or phishing indicators before a short link is created.
            </Section>

            <Section title="3. Account tiers">
              Anonymous visitors may shorten a limited number of links without an account. Registered
              accounts default to the Standard tier with a daily link limit. The optional Plus subscription
              unlocks unlimited shortening, custom aliases, QR codes, bulk shortening, password protection,
              and AI-generated link summaries, billed monthly until cancelled.
            </Section>

            <Section title="4. Acceptable use">
              You agree not to use the Service to create or distribute links to malicious, illegal,
              infringing, or harassing content. We reserve the right to disable any link, suspend, or
              terminate any account found in violation of this policy, with or without prior notice.
            </Section>

            <Section title="5. Reporting and moderation">
              Anyone may report a short link they believe is malicious, spammy, or otherwise abusive.
              Reports are reviewed and may result in a link being deactivated. Automated scanning also
              flags suspicious links for review.
            </Section>

            <Section title="6. No warranty">
              The Service is provided "as is." While we scan links for known threats, we cannot guarantee
              that every destination is safe, accurate, or available at all times. You are responsible for
              verifying the destination of any link before relying on it.
            </Section>

            <Section title="7. Limitation of liability">
              To the extent permitted by law, the Service and its operator are not liable for any indirect,
              incidental, or consequential damages arising from your use of the Service, including damages
              resulting from a link's destination content.
            </Section>

            <Section title="8. Changes to these terms">
              These terms may be updated from time to time. Continued use of the Service after changes
              are posted constitutes acceptance of the revised terms.
            </Section>

            <Section title="9. Contact">
              Questions about these terms can be directed through the project's{' '}
              <a
                href="https://github.com/noketchup21"
                target="_blank"
                rel="noreferrer"
                className="text-accent font-bold hover:underline"
              >
                GitHub profile
              </a>.
            </Section>
          </div>
        </Card>
      </section>
    </div>
  );
}

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div>
      <h2 className="font-heading font-bold text-base mb-1.5">{title}</h2>
      <p className="text-mutedForeground">{children}</p>
    </div>
  );
}