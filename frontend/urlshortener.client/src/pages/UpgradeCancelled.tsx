import { Link } from 'react-router-dom';
import { DashboardLayout } from '../layouts/DashboardLayout';
import { Card } from '../components/ui/Card';

export function UpgradeCancelled() {
  return (
    <DashboardLayout>
      <Card className="max-w-md mx-auto text-center py-10">
        <h1 className="text-2xl mb-2">Checkout cancelled</h1>
        <p className="font-body text-mutedForeground mb-6">
          No worries — you can upgrade anytime.
        </p>
        <Link to="/account/upgrade" className="font-bold text-accent hover:underline">
          Try again →
        </Link>
      </Card>
    </DashboardLayout>
  );
}