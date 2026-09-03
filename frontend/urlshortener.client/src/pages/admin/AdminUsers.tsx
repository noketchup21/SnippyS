import { useEffect, useState } from 'react';
import { adminApi } from '../../api/admin';
import type { AdminUser } from '../../api/types';
import { AdminLayout } from '../../layouts/AdminLayout';
import { Card } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { Spinner } from '../../components/ui/Spinner';

export function AdminUsers() {
  const [users, setUsers] = useState<AdminUser[] | null>(null);
  const [search, setSearch] = useState('');
  const [actingOn, setActingOn] = useState<string | null>(null);

  const loadUsers = () => adminApi.getUsers().then((res) => setUsers(res.data));

  useEffect(() => {
    loadUsers();
  }, []);

  const toggleBan = async (user: AdminUser) => {
    setActingOn(user.id);

    try {
      if (user.isBanned) {
        await adminApi.unbanUser(user.id);
      } else {
        await adminApi.banUser(user.id);
      }

      await loadUsers();
    } finally {
      setActingOn(null);
    }
  };

  const filtered = users?.filter((u) =>
    u.email.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <AdminLayout>
      <h1 className="text-2xl mb-6">Users</h1>

      <input
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        placeholder="Search by email..."
        className="w-full max-w-sm bg-white border-2 border-border rounded-md px-4 py-2 font-body mb-4 focus:outline-none focus:border-accent"
      />

      {users === null ? (
        <Spinner label="Loading users..." />
      ) : (
        <Card className="!p-0 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-muted">
              <tr className="text-left font-bold uppercase text-xs tracking-wide text-mutedForeground">
                <th className="px-4 py-3">Email</th>
                <th className="px-4 py-3">Role</th>
                <th className="px-4 py-3">Tier</th>
                <th className="px-4 py-3">Status</th>
                <th className="px-4 py-3">Joined</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>

            <tbody>
              {filtered?.map((user) => (
                <tr key={user.id} className="border-t border-border">
                  <td className="px-4 py-3 font-body">{user.email}</td>

                  <td className="px-4 py-3">{user.role}</td>

                  <td className="px-4 py-3">
                    <Badge status={user.tier === 'Plus' ? 'success' : 'neutral'}>
                      {user.tier}
                    </Badge>
                  </td>

                  <td className="px-4 py-3">
                    <Badge status={user.isBanned ? 'danger' : 'success'}>
                      {user.isBanned ? 'Banned' : 'Active'}
                    </Badge>
                  </td>

                  <td className="px-4 py-3 text-mutedForeground">
                    {new Date(user.createdAt).toLocaleDateString()}
                  </td>

                  <td className="px-4 py-3">
                    <Button
                      variant="secondary"
                      className="!px-3 !py-1.5 !text-xs"
                      disabled={actingOn === user.id || user.role === 'Admin'}
                      onClick={() => toggleBan(user)}
                    >
                      {user.isBanned ? 'Unban' : 'Ban'}
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      )}
    </AdminLayout>
  );
}