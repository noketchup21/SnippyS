import { useState } from 'react';
import type { FormEvent } from 'react';
import { Link, Navigate } from 'react-router-dom';
import { linksApi } from '../api/links';
import type { BulkCreateLinkResultItem } from '../api/types';
import { buildShortUrl } from '../api/config';
import { useAuth } from '../auth/AuthContext';
import { DashboardLayout } from '../layouts/DashboardLayout';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { CopyableCode } from '../components/ui/CopyableCode';
import { CheckCircle2, XCircle, Layers } from 'lucide-react';

export function BulkShorten() {
  const { user } = useAuth();
  const [urlsText, setUrlsText] = useState('');
  const [results, setResults] = useState<BulkCreateLinkResultItem[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Gate the whole page — Standard users get redirected to the upgrade page
  if (user && user.tier !== 'Plus') {
    return <Navigate to="/account/upgrade" replace />;
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setResults(null);

    const urls = urlsText
      .split('\n')
      .map((u) => u.trim())
      .filter((u) => u.length > 0);

    if (urls.length === 0) {
      setError('Paste at least one URL.');
      return;
    }

    setIsSubmitting(true);
    try {
      const res = await linksApi.bulkCreate({ urls });
      setResults(res.data);
    } catch (err: any) {
      setError(err.response?.data?.detail ?? 'Bulk shorten failed. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <DashboardLayout>
      <h1 className="text-2xl mb-2 flex items-center gap-2">
        <Layers size={22} /> Bulk Shorten
      </h1>
      <p className="font-body text-mutedForeground mb-6">
        Paste one URL per line — shorten up to a batch at once.
      </p>

      <Card className="mb-6">
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <textarea
            value={urlsText}
            onChange={(e) => setUrlsText(e.target.value)}
            rows={8}
            placeholder={'https://example.com/page-one\nhttps://example.com/page-two\nhttps://example.com/page-three'}
            className="bg-white border-2 border-border rounded-md px-4 py-3 font-body focus:outline-none focus:border-accent focus:shadow-[4px_4px_0px_0px_#8B5CF6] transition-all resize-y"
          />

          {error && (
            <p className="text-sm font-bold text-pink-700 bg-secondary/10 border-2 border-secondary rounded-md px-3 py-2">
              {error}
            </p>
          )}

          <Button type="submit" disabled={isSubmitting} className="self-start">
            {isSubmitting ? 'Shortening...' : 'Shorten all'}
          </Button>
        </form>
      </Card>

      {results && (
        <Card>
          <h3 className="text-lg mb-4">
            Results — {results.filter((r) => r.success).length} of {results.length} succeeded
          </h3>
          <div className="flex flex-col gap-2">
            {results.map((r, i) => (
              <div
                key={i}
                className="flex items-center justify-between gap-4 py-2 border-b border-border last:border-0 flex-wrap"
              >
                <div className="flex items-center gap-2 min-w-0">
                  {r.success ? (
                    <CheckCircle2 size={18} className="text-quaternary shrink-0" />
                  ) : (
                    <XCircle size={18} className="text-secondary shrink-0" />
                  )}
                  <p className="font-body text-sm text-mutedForeground truncate">{r.originalUrl}</p>
                </div>
                {r.success && r.shortCode ? (
                  <CopyableCode value={buildShortUrl(r.shortCode)} />
                ) : (
                  <span className="text-xs font-bold text-pink-700">{r.error}</span>
                )}
              </div>
            ))}
          </div>
        </Card>
      )}
    </DashboardLayout>
  );
}