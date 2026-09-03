import { apiClient } from './client';
import type { RegisterRequest, LoginRequest, CurrentUser, ResetPasswordRequest, ForgotPasswordRequest, VerifyEmailRequest } from './types';

export const authApi = {
  register: (data: RegisterRequest) =>
    apiClient.post('/api/auth/register', data),

  login: (data: LoginRequest) =>
    apiClient.post('/api/auth/login', data),

  logout: () =>
    apiClient.post('/api/auth/logout'),

  getMe: () =>
    apiClient.get<CurrentUser>('/api/auth/me'),

  sendVerificationCode: () => apiClient.post('/api/auth/send-verification-code'),
  verifyEmail: (data: VerifyEmailRequest) => apiClient.post('/api/auth/verify-email', data),
  forgotPassword: (data: ForgotPasswordRequest) => apiClient.post('/api/auth/forgot-password', data),
  resetPassword: (data: ResetPasswordRequest) => apiClient.post('/api/auth/reset-password', data),
};