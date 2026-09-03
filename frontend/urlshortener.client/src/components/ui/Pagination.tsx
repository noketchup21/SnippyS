import { ChevronLeft, ChevronRight } from 'lucide-react';

export function Pagination({ page, totalPages, onPageChange }: {
  page: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}) {
  if (totalPages <= 1) return null;

  return (
    <div className="flex items-center justify-center gap-3 mt-6">
      <button
        onClick={() => onPageChange(page - 1)}
        disabled={page <= 1}
        className="p-2 rounded-full border-2 border-foreground bg-white hover:bg-tertiary disabled:opacity-30 disabled:hover:bg-white transition-colors"
      >
        <ChevronLeft size={16} strokeWidth={2.5} />
      </button>
      <span className="font-bold text-sm">
        Page {page} of {totalPages}
      </span>
      <button
        onClick={() => onPageChange(page + 1)}
        disabled={page >= totalPages}
        className="p-2 rounded-full border-2 border-foreground bg-white hover:bg-tertiary disabled:opacity-30 disabled:hover:bg-white transition-colors"
      >
        <ChevronRight size={16} strokeWidth={2.5} />
      </button>
    </div>
  );
}