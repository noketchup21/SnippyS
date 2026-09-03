import { apiClient } from './client';
import type { AdminUser, AdminReport, AdminLogEntry } from './types';

export const adminApi = {
  getUsers: () => apiClient.get<AdminUser[]>('/api/admin/users'),
  banUser: (userId: string) => apiClient.patch(`/api/admin/users/${userId}/ban`),
  unbanUser: (userId: string) => apiClient.patch(`/api/admin/users/${userId}/unban`),

  getPendingReports: () => apiClient.get<AdminReport[]>('/api/admin/reports'),
  resolveReport: (reportId: string, deactivateLink: boolean) =>
    apiClient.post(`/api/admin/reports/${reportId}/resolve?deactivateLink=${deactivateLink}`),

  getLogs: () => apiClient.get<AdminLogEntry[]>('/api/admin/logs'),

  getUnresolvedLinks: () => apiClient.get<UnresolvedLink[]>('/api/admin/links/unresolved'),

  getStats: () => apiClient.get<AdminStats>('/api/admin/stats'),

  getLinkDetail: (id: string) => apiClient.get<AdminLinkDetail>(`/api/admin/links/${id}`),

  getFlaggedLinks: () => apiClient.get<FlaggedLink[]>('/api/admin/links/flagged'),

  getReportDetail: (id: string) => apiClient.get<AdminReportDetail>(`/api/admin/reports/${id}`),
};

export interface UnresolvedLink {
  id: string;
  shortCode: string;
  originalUrl: string;
  vtCheckAttempts: number;
  vtCheckedAt: string | null;
  createdAt: string;
  isActive: boolean;
}

export interface AdminStats {
  totalUsers: number;
  plusUsers: number;
  bannedUsers: number;
  totalLinks: number;
  activeLinks: number;
  inactiveLinks: number;
  cleanLinks: number;
  pendingLinks: number;
  maliciousLinks: number;
  unresolvedLinks: number;
  pendingReports: number;
}

export interface AdminLinkDetail {
  id: string;
  shortCode: string;
  originalUrl: string;
  userId: string | null;
  isActive: boolean;
  isCustomAlias: boolean;
  hasPassword: boolean;
  vtStatus: 'Unchecked' | 'Pending' | 'Clean' | 'Malicious' | 'Unresolved';
  vtCheckAttempts: number;
  vtCheckedAt: string | null;
  createdAt: string;
}

export interface FlaggedLink {
  id: string;
  shortCode: string;
  originalUrl: string;
  vtStatus: 'Malicious' | 'Unresolved';
  vtCheckAttempts: number;
  vtCheckedAt: string | null;
  createdAt: string;
  isActive: boolean;
}

export interface AdminReportDetail {
  id: string;
  reason: string;
  description: string | null;
  status: string;
  reportedByUserId: string | null;
  createdAt: string;
  resolvedAt: string | null;
  link: {
    id: string;
    shortCode: string;
    originalUrl: string;
    isActive: boolean;
    vtStatus: string;
    userId: string | null;
  };
}