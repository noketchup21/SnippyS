import { Turnstile } from '@marsidev/react-turnstile';
import type { TurnstileInstance } from '@marsidev/react-turnstile';
import { forwardRef } from 'react';

interface CaptchaWidgetProps {
  onVerify: (token: string) => void;
  onExpire?: () => void;
}

export const CaptchaWidget = forwardRef<TurnstileInstance, CaptchaWidgetProps>(
  ({ onVerify, onExpire }, ref) => (
    <Turnstile
      ref={ref}
      siteKey={import.meta.env.VITE_TURNSTILE_SITE_KEY}
      onSuccess={onVerify}
      onExpire={onExpire}
      options={{ theme: 'light' }}
    />
  )
);
CaptchaWidget.displayName = 'CaptchaWidget';