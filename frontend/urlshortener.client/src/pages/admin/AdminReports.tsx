import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { adminApi } from '../../api/admin';
import type { AdminReport } from '../../api/types';
import { AdminLayout } from '../../layouts/AdminLayout';
import { Card } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Spinner } from '../../components/ui/Spinner';

export function AdminReports() {
  const [reports, setReports] = useState<AdminReport[] | null>(null);
  const [actingOn, setActingOn] = useState<string | null>(null);

  const loadReports = () =>
    adminApi.getPendingReports().then((res) => setReports(res.data));

  useEffect(() => {
    loadReports();
  }, []);

  const resolve = async (reportId: string, deactivateLink: boolean) => {
    setActingOn(reportId);

    try {
      await adminApi.resolveReport(reportId, deactivateLink);
      await loadReports();
    } finally {
      setActingOn(null);
    }
  };

  return (
    <AdminLayout>
      <h1 className="text-2xl mb-6">Pending Reports</h1>

      {reports === null && <Spinner label="Loading reports..." />}

      {reports && reports.length === 0 && (
        <Card>
          <p className="font-body text-mutedForeground text-center py-8">
            No pending reports — nice and clean.
          </p>
        </Card>
      )}

      {reports && reports.length > 0 && (
        <div className="flex flex-col gap-3">
          {reports.map((report) => (
            <Card key={report.id}>
              <div className="flex items-start justify-between flex-wrap gap-4">
                <Link
                  to={`/admin/reports/${report.id}`}
                  className="flex-1 min-w-0"
                >
                  <div className="flex items-center gap-2 mb-1">
                    <Badge status="warning">{report.reason}</Badge>

                    <span className="text-xs text-mutedForeground font-body">
                      {new Date(report.createdAt).toLocaleString()}
                    </span>
                  </div>

                  {report.description && (
                    <p className="font-body text-sm mb-1">
                      {report.description}
                    </p>
                  )}

                  <p className="text-xs text-mutedForeground font-body mb-2">
                    Link ID: {report.linkId}
                  </p>

                  <p className="text-xs text-accent font-bold hover:underline">
                    View details →
                  </p>
                </Link>

                <div className="flex gap-2">
                  <Button
                    variant="secondary"
                    className="!px-3 !py-1.5 !text-xs"
                    disabled={actingOn === report.id}
                    onClick={() => resolve(report.id, false)}
                  >
                    Dismiss
                  </Button>

                  <Button
                    className="!px-3 !py-1.5 !text-xs"
                    disabled={actingOn === report.id}
                    onClick={() => resolve(report.id, true)}
                  >
                    Deactivate link
                  </Button>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}
    </AdminLayout>
  );
}