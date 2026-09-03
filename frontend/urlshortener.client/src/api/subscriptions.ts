import { apiClient } from './client';

export const subscriptionsApi = {
  createCheckoutSession: (provider: 'PayOS' | 'Stripe') =>
    apiClient.post<{ checkoutUrl: string }>('/api/subscriptions/checkout', { provider }),
  getStatus: () => apiClient.get<SubscriptionStatus>('/api/subscriptions/me'),
};

export interface SubscriptionStatus {
  tier: 'Standard' | 'Plus';
  status: string | null;
  currentPeriodEnd: string | null;
}