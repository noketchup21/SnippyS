import { apiClient } from './client';
import type {
  CreateLinkRequest,
  LinkResponse,
  BulkCreateLinkRequest,
  BulkCreateLinkResultItem,
  DailyStatDto,
} from './types';

export interface LinkSummary {
  id: string;
  shortCode: string;
  originalUrl: string;
  isActive: boolean;
  isCustomAlias: boolean;
  hasPassword: boolean;
  vtStatus: 'Unchecked' | 'Pending' | 'Clean' | 'Malicious' | 'Unresolved';
  createdAt: string;
}

export interface LinkDetail {
  id: string;
  shortCode: string;
  originalUrl: string;
  isActive: boolean;
  isCustomAlias: boolean;
  hasPassword: boolean;
  vtStatus: 'Unchecked' | 'Pending' | 'Clean' | 'Malicious' | 'Unresolved';
  createdAt: string;
}

export interface PagedLinks {
  items: LinkSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export const linksApi = {
  create: (data: CreateLinkRequest) =>
    apiClient.post<LinkResponse>('/api/links', data),

  bulkCreate: (data: BulkCreateLinkRequest) =>
    apiClient.post<BulkCreateLinkResultItem[]>('/api/links/bulk', data),

  getAnalytics: (linkId: string) =>
    apiClient.get<DailyStatDto[]>(`/api/links/${linkId}/analytics`),

  getQrCode: (linkId: string) =>
    `${import.meta.env.VITE_API_BASE_URL}/api/links/${linkId}/qr`,

  getMyLinks: (page: number, pageSize: number = 10) =>
    apiClient.get<PagedLinks>('/api/links', { params: { page, pageSize } }),

  getById: (id: string) => apiClient.get<LinkDetail>(`/api/links/${id}`),
  
};
