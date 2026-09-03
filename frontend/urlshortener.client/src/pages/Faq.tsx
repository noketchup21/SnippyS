import { useState } from 'react';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/ui/Card';
import { HelpCircle, ChevronDown } from 'lucide-react';
import { clsx } from 'clsx';

const FAQS = [
  {
    q: 'Is it free to shorten links?',
    a: 'Yes. Anonymous visitors get 2 free shortened links before needing an account. Creating a free account bumps that up to 10 links a day. No credit card required for either.',
  },
  {
    q: 'What do I get with the Plus subscription?',
    a: 'Plus ($3.99/month) unlocks unlimited link shortening, custom aliases, QR code generation, bulk shortening, password-protected links, and AI-generated summaries of the page behind each link.',
  },
  {
    q: 'How does the malware scanning work?',
    a: "Every submitted URL is checked against VirusTotal's database before a short link is created. If a link is later flagged as malicious, it's automatically deactivated.",
  },
  {
    q: "What's the AI summary feature?",
    a: 'For links created by Plus subscribers, AI reads the destination page and generates a one-sentence summary, key topics, and an estimated reading time — shown to visitors before they continue to the link.',
  },
  {
    q: 'Why do I see a page before being redirected?',
    a: "That's the confirmation screen — it verifies you're not a bot, shows the AI summary (if available), and gives you a clear preview of where the link leads before you continue.",
  },
  {
    q: 'Can I password-protect a link?',
    a: "Yes, on the Plus tier. Anyone who clicks the link will need to enter the password you set before they're redirected.",
  },
  {
    q: 'How do I report a suspicious link?',
    a: 'Use the "Report it" link on the homepage or visit /report directly. You can report any short link without needing an account.',
  },
  {
    q: 'Can I cancel my Plus subscription?',
    a: 'Yes, anytime from your Account page. Your Plus features remain active until the end of your current billing period.',
  },
  {
    q: 'Do you track who clicks my links?',
    a: 'We record click counts and basic device/referrer data for your own analytics dashboard. We do not sell this data or share it with third parties.',
  },
];

export function Faq() {
  const [openIndex, setOpenIndex] = useState<number | null>(0);

  return (
    <div className="min-h-screen">
      <Navbar />
      <section className="relative max-w-3xl mx-auto px-6 pt-12 pb-24">
        <div className="absolute top-0 right-10 w-56 h-56 bg-tertiary/20 rounded-full -z-10" aria-hidden="true" />

        <div className="text-center mb-10">
          <div className="w-14 h-14 rounded-full bg-accent/10 flex items-center justify-center mx-auto mb-4">
            <HelpCircle size={22} className="text-accent" />
          </div>
          <h1 className="text-3xl mb-2">Frequently asked questions</h1>
          <p className="font-body text-mutedForeground">Everything you might want to know, in one place.</p>
        </div>

        <div className="flex flex-col gap-3">
          {FAQS.map((item, i) => {
            const isOpen = openIndex === i;
            return (
              <Card
                key={i}
                className="!hover:rotate-0 !hover:scale-100 !p-0 overflow-hidden cursor-pointer"
                onClick={() => setOpenIndex(isOpen ? null : i)}
              >
                <div className="flex items-center justify-between gap-4 px-6 py-4">
                  <h3 className="font-heading font-bold text-sm md:text-base">{item.q}</h3>
                  <ChevronDown
                    size={18}
                    className={clsx('shrink-0 transition-transform duration-300', isOpen && 'rotate-180')}
                  />
                </div>
                <div
                  className={clsx(
                    'grid transition-all duration-300 ease-bounce',
                    isOpen ? 'grid-rows-[1fr] opacity-100' : 'grid-rows-[0fr] opacity-0'
                  )}
                >
                  <div className="overflow-hidden">
                    <p className="font-body text-mutedForeground text-sm px-6 pb-4 leading-relaxed">
                      {item.a}
                    </p>
                  </div>
                </div>
              </Card>
            );
          })}
        </div>
      </section>
    </div>
  );
}