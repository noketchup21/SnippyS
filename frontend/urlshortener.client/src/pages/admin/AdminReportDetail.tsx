import { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { adminApi } from '../../api/admin';
import type { AdminReportDetail } from '../../api/admin';
import { buildShortUrl } from '../../api/config';
import { AdminLayout } from '../../layouts/AdminLayout';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { CopyableCode } from '../../components/ui/CopyableCode';
import { Spinner } from '../../components/ui/Spinner';
import { ArrowLeft } from 'lucide-react';

export function AdminReportDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [report, setReport] = useState<AdminReportDetail | null>(null);
  const [isActing, setIsActing] = useState(false);

  useEffect(() => {
    if (!id) return;
    adminApi.getReportDetail(id).then((res) => setReport(res.data));
  }, [id]);

  const resolve = async (deactivateLink: boolean) => {
    if (!id) return;
    setIsActing(true);
    try {
      await adminApi.resolveReport(id, deactivateLink);
      navigate('/admin/reports');
    } finally {
      setIsActing(false);
    }
  };

  return (
    <AdminLayout>
      <Link to="/admin/reports" className="inline-flex items-center gap-1 font-bold text-sm text-mutedForeground hover:text-accent mb-4">
        <ArrowLeft size={16} /> Back to reports
      </Link>

      {!report && <Spinner label="Loading report..." />}

      {report && (
        <div className="flex flex-col gap-6">
          <Card>
            <div className="flex items-center gap-2 mb-3">
              <Badge status="warning">{report.reason}</Badge>
              <Badge status={report.status === 'Pending' ? 'warning' : 'neutral'}>{report.status}</Badge>
            </div>
            {report.description && <p className="font-body mb-2">{report.description}</p>}
            <p className="text-xs text-mutedForeground font-body">
              Reported {new Date(report.createdAt).toLocaleString()}
              {report.reportedByUserId ? ' by a registered user' : ' anonymously'}
            </p>
          </Card>

          <Card>
            <h3 className="text-lg mb-3">Reported Link</h3>
            <div className="flex items-center gap-3 mb-2">
              <CopyableCode value={buildShortUrl(report.link.shortCode)} />
              <Badge status={report.link.isActive ? 'success' : 'neutral'}>
                {report.link.isActive ? 'Active' : 'Inactive'}
              </Badge>
              <Badge status={report.link.vtStatus === 'Malicious' ? 'danger' : 'neutral'}>
                {report.link.vtStatus}
              </Badge>
            </div>
            <p className="font-body text-mutedForeground break-all mb-3">→ {report.link.originalUrl}</p>
            <Link to={`/admin/links/${report.link.id}`} className="text-sm font-bold text-accent hover:underline">
              View full link details →
            </Link>
          </Card>

          {report.status === 'Pending' && (
            <div className="flex gap-3">
              <Button variant="secondary" disabled={isActing} onClick={() => resolve(false)}>
                Dismiss
              </Button>
              <Button disabled={isActing} onClick={() => resolve(true)}>
                Deactivate link
              </Button>
            </div>
          )}
        </div>
      )}
    </AdminLayout>
  );
}