import { useEffect, useState } from 'react';
import { adminApi } from '../../api/admin';
import type { AdminLogEntry } from '../../api/types';
import { AdminLayout } from '../../layouts/AdminLayout';
import { Card } from '../../components/ui/Card';
import { Spinner } from '../../components/ui/Spinner';

export function AdminLogs() {
  const [logs, setLogs] = useState<AdminLogEntry[] | null>(null);

  useEffect(() => {
    adminApi.getLogs().then((res) => setLogs(res.data));
  }, []);

  return (
    <AdminLayout>
      <h1 className="text-2xl mb-6">Audit Logs</h1>

      {logs === null && <Spinner label="Loading logs..." />}

      {logs && logs.length === 0 && (
        <Card>
          <p className="font-body text-mutedForeground text-center py-8">
            No audit logs found.
          </p>
        </Card>
      )}

      {logs && logs.length > 0 && (
        <Card className="!p-0 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-sm min-w-[600px]">
              <thead className="bg-muted">
                <tr className="text-left font-bold uppercase text-xs tracking-wide text-mutedForeground">
                  <th className="px-4 py-3">Time</th>
                  <th className="px-4 py-3">Action</th>
                  <th className="px-4 py-3">Target</th>
                </tr>
              </thead>

              <tbody>
                {logs.map((log) => (
                  <tr key={log.id} className="border-t border-border">
                    <td className="px-4 py-3 text-mutedForeground font-body">
                      {new Date(log.createdAt).toLocaleString()}
                    </td>

                    <td className="px-4 py-3 font-bold">
                      {log.action}
                    </td>

                    <td className="px-4 py-3 font-body text-mutedForeground">
                      {log.targetEntity} — {log.targetId}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </Card>
      )}
    </AdminLayout>
  );
}