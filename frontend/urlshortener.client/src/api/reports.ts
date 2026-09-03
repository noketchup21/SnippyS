import { apiClient } from './client';
import type { CreateReportRequest } from './types';

export const reportsApi = {
  create: (data: CreateReportRequest) => apiClient.post('/api/reports', data),
};