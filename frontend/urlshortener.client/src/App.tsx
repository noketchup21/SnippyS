import { Routes, Route, Navigate } from 'react-router-dom';
import { Home } from './pages/Home';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { ProtectedRoute } from './auth/ProtectedRoute';
import { Dashboard } from './pages/Dashboard';
import { LinkDetail } from './pages/LinkDetail';
import { BulkShorten } from './pages/BulkShorten';
import { UpgradeCancelled } from './pages/UpgradeCancelled';
import { UpgradeSuccess } from './pages/UpgradeSuccess';
import { Upgrade } from './pages/Upgrade';
import { Account } from './pages/Account';
import { AdminLogs } from './pages/admin/AdminLogs';
import { AdminReports } from './pages/admin/AdminReports';
import { AdminUsers } from './pages/admin/AdminUsers';
import { AdminOverview } from './pages/admin/AdminOverview';
import { NotFound } from './pages/NotFound';
import { ForgotPassword } from './pages/ForgotPassword';
import { VerifyEmail } from './pages/VerifyEmail';
import { VerifyEmailPrompt } from './pages/VerifyEmailPrompt';
import { AdminLinkDetail } from './pages/admin/AdminLinkDetail';
import { AdminFlaggedLinks } from './pages/admin/AdminFlaggedLinks';
import { AdminReportDetail } from './pages/admin/AdminReportDetail';
import { Report } from './pages/Report';
import { About } from './pages/About';
import { Faq } from './pages/Faq';
import { Terms } from './pages/Terms';

function App() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />

      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        }
      />

      <Route
        path="/dashboard/links/:id"
        element={
          <ProtectedRoute>
            <LinkDetail />
          </ProtectedRoute>
        }
      />

      <Route
        path="/dashboard/bulk"
        element={
          <ProtectedRoute>
            <BulkShorten />
          </ProtectedRoute>
        }
      />

      <Route path="/account" element={<ProtectedRoute><Account /></ProtectedRoute>} />

      <Route path="/account/upgrade" element={<ProtectedRoute><Upgrade /></ProtectedRoute>} />

      <Route path="/account/upgrade/success" element={<ProtectedRoute><UpgradeSuccess /></ProtectedRoute>} />
      
      <Route path="/account/upgrade/cancelled" element={<ProtectedRoute><UpgradeCancelled /></ProtectedRoute>} />

      <Route path="/admin" element={<ProtectedRoute requireAdmin><AdminOverview /></ProtectedRoute>} />

      <Route path="/admin/users" element={<ProtectedRoute requireAdmin><AdminUsers /></ProtectedRoute>} />
      
      <Route path="/admin/reports" element={<ProtectedRoute requireAdmin><AdminReports /></ProtectedRoute>} />
      
      <Route path="/admin/logs" element={<ProtectedRoute requireAdmin><AdminLogs /></ProtectedRoute>} />

      <Route path="/verify-email" element={<ProtectedRoute><VerifyEmail /></ProtectedRoute>} />
      
      <Route path="/forgot-password" element={<ForgotPassword />} />

      <Route path="/verify-email-prompt" element={<VerifyEmailPrompt />} />
      
      <Route path="/admin/links" element={<ProtectedRoute requireAdmin><AdminFlaggedLinks /></ProtectedRoute>} />
      
      <Route path="/admin/links/:id" element={<ProtectedRoute requireAdmin><AdminLinkDetail /></ProtectedRoute>} />

      <Route path="/admin/reports/:id" element={<ProtectedRoute requireAdmin><AdminReportDetail /></ProtectedRoute>} />

      <Route path="/admin/links/:id" element={<ProtectedRoute requireAdmin><AdminLinkDetail /></ProtectedRoute>} />
      
      <Route path="/report" element={<Report />} />

      <Route path="/terms" element={<Terms />} />

      <Route path="/faq" element={<Faq />} />

      <Route path="/about" element={<About />} />
      
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
}

export default App;