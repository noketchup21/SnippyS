import type { ButtonHTMLAttributes, ReactNode } from 'react';
import { clsx } from 'clsx';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary';
  icon?: ReactNode;
}

export function Button({ variant = 'primary', icon, children, className, ...props }: ButtonProps) {
  const base = 'inline-flex items-center gap-2 rounded-full border-2 border-foreground font-heading font-bold px-6 py-3 transition-all duration-300 ease-bounce';

  const variants = {
    primary: 'bg-accent text-accent-foreground shadow-pop hover:shadow-pop-hover hover:-translate-x-0.5 hover:-translate-y-0.5 active:shadow-pop-active active:translate-x-0.5 active:translate-y-0.5',
    secondary: 'bg-transparent text-foreground hover:bg-tertiary',
  };

  return (
    <button className={clsx(base, variants[variant], className)} {...props}>
      {children}
      {icon && <span className="rounded-full bg-white/20 p-1">{icon}</span>}
    </button>
  );
}