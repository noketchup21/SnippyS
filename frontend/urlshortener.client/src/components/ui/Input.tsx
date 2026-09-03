import type { InputHTMLAttributes } from 'react';
import { forwardRef, useState } from 'react';
import { clsx } from 'clsx';
import { Eye, EyeOff } from 'lucide-react';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, className, type, ...props }, ref) => {
    const [showPassword, setShowPassword] = useState(false);
    const isPasswordField = type === 'password';
    const resolvedType = isPasswordField && showPassword ? 'text' : type;

    return (
      <div className="flex flex-col gap-1.5">
        {label && (
          <label className="text-xs font-bold uppercase tracking-wide text-mutedForeground">
            {label}
          </label>
        )}
        <div className="relative">
          <input
            ref={ref}
            type={resolvedType}
            className={clsx(
              'w-full bg-white border-2 border-border rounded-md px-4 py-3 font-body',
              isPasswordField && 'pr-11',
              'focus:outline-none focus:border-accent focus:shadow-[4px_4px_0px_0px_#8B5CF6]',
              'transition-all duration-200',
              className
            )}
            {...props}
          />
          {isPasswordField && (
            <button
              type="button"
              onClick={() => setShowPassword((s) => !s)}
              tabIndex={-1}
              className="absolute right-3 top-1/2 -translate-y-1/2 text-mutedForeground hover:text-foreground transition-colors"
            >
              {showPassword ? <EyeOff size={17} /> : <Eye size={17} />}
            </button>
          )}
        </div>
      </div>
    );
  }
);
Input.displayName = 'Input';