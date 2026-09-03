import type { ReactNode, HTMLAttributes } from 'react';
import { clsx } from 'clsx';

interface CardProps extends HTMLAttributes<HTMLDivElement> {
  children: ReactNode;
  className?: string;
  shadowColor?: 'default' | 'pink';
}

export function Card({ children, className, shadowColor = 'default', ...props }: CardProps) {
  return (
    <div
      className={clsx(
        'bg-white border-2 border-foreground rounded-lg p-6',
        'transition-transform duration-300 ease-bounce hover:-rotate-1 hover:scale-[1.02]',
        shadowColor === 'pink' ? 'shadow-pop-pink' : 'shadow-pop-soft',
        className
      )}
      {...props}
    >
      {children}
    </div>
  );
}