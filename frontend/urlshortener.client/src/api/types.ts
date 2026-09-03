export interface RegisterRequest {
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

// Returned by /api/auth/me — replaces the old DecodedToken approach
export interface CurrentUser {
  id: string;
  email: string;
  role: 'User' | 'Admin';
  tier: 'Standard' | 'Plus';
}

export interface CreateLinkRequest {
  originalUrl: string;
  customAlias?: string;
  password?: string;
}

export interface LinkResponse {
  id: string;
  shortCode: string;
  originalUrl: string;
  createdAt: string;
}

export interface BulkCreateLinkRequest {
  urls: string[];
}

export interface BulkCreateLinkResultItem {
  originalUrl: string;
  success: boolean;
  shortCode: string | null;
  error: string | null;
}

export interface DailyStatDto {
  date: string;
  clickCount: number;
}

export interface CreateReportRequest {
  shortCodeOrUrl: string;
  reason: 'Malicious' | 'Phishing' | 'Spam' | 'Illegal' | 'Other';
  description?: string;
  captchaToken: string;
}

export interface AdminUser {
  id: string;
  email: string;
  role: 'User' | 'Admin';
  tier: 'Standard' | 'Plus';
  isBanned: boolean;
  createdAt: string;
}

export interface AdminReport {
  id: string;
  linkId: string;
  reportedByUserId: string | null;
  reason: string;
  description: string | null;
  status: string;
  createdAt: string;
}

export interface AdminLogEntry {
  id: number;
  adminUserId: string;
  action: string;
  targetEntity: string;
  targetId: string;
  createdAt: string;
}

export interface AdminUser {
  id: string;
  email: string;
  role: 'User' | 'Admin';
  tier: 'Standard' | 'Plus';
  isBanned: boolean;
  createdAt: string;
}

export interface AdminReport {
  id: string;
  linkId: string;
  reportedByUserId: string | null;
  reason: string;
  description: string | null;
  status: string;
  createdAt: string;
}

export interface AdminLogEntry {
  id: number;
  adminUserId: string;
  action: string;
  targetEntity: string;
  targetId: string;
  createdAt: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  captchaToken: string;
}

export interface LoginRequest {
  email: string;
  password: string;
  captchaToken: string;
}

export interface VerifyEmailRequest {
  code: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  code: string;
  newPassword: string;
}