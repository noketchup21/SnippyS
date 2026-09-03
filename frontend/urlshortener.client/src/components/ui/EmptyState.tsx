import type { ReactNode } from 'react';

export function EmptyState({ icon, title, description, action }: {
  icon: ReactNode;
  title: string;
  description: string;
  action?: ReactNode;
}) {
  return (
    <div className="flex flex-col items-center text-center py-16 px-6">
      <div className="w-16 h-16 rounded-full bg-muted flex items-center justify-center mb-4 border-2 border-foreground">
        {icon}
      </div>
      <h3 className="text-lg mb-1">{title}</h3>
      <p className="text-mutedForeground font-body mb-4 max-w-sm">{description}</p>
      {action}
    </div>
  );
}