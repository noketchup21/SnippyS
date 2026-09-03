/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        background: '#FFFDF5',
        foreground: '#1E293B',
        muted: '#F1F5F9',
        mutedForeground: '#64748B',
        accent: {
          DEFAULT: '#8B5CF6',
          foreground: '#FFFFFF',
        },
        secondary: '#F472B6',
        tertiary: '#FBBF24',
        quaternary: '#34D399',
        border: '#E2E8F0',
      },
      fontFamily: {
        heading: ['Outfit', 'system-ui', 'sans-serif'],
        body: ['"Plus Jakarta Sans"', 'system-ui', 'sans-serif'],
      },
      fontSize: {
        xs: '0.64rem',
        sm: '0.8rem',
        base: '1rem',
        lg: '1.25rem',
        xl: '1.563rem',
        '2xl': '1.953rem',
        '3xl': '2.441rem',
        '4xl': '3.052rem',
      },
      borderRadius: {
        sm: '8px',
        md: '16px',
        lg: '24px',
      },
      borderWidth: {
        DEFAULT: '2px',
      },
      boxShadow: {
        pop: '4px 4px 0px 0px #1E293B',
        'pop-hover': '6px 6px 0px 0px #1E293B',
        'pop-active': '2px 2px 0px 0px #1E293B',
        'pop-pink': '8px 8px 0px 0px #F472B6',
        'pop-soft': '8px 8px 0px 0px #E2E8F0',
      },
      transitionTimingFunction: {
        bounce: 'cubic-bezier(0.34, 1.56, 0.64, 1)',
      },
      keyframes: {
        wiggle: {
          '0%, 100%': { transform: 'rotate(0deg)' },
          '25%': { transform: 'rotate(3deg)' },
          '75%': { transform: 'rotate(-3deg)' },
        },
        popIn: {
          '0%': { transform: 'scale(0)', opacity: '0' },
          '80%': { transform: 'scale(1.05)', opacity: '1' },
          '100%': { transform: 'scale(1)' },
        },
        meteor: {
          '0%': { transform: 'translateY(-10%)', opacity: '0' },
          '10%': { opacity: '1' },
          '90%': { opacity: '1' },
          '100%': { transform: 'translateY(110vh)', opacity: '0' },
        },
        float: {
          '0%, 100%': { transform: 'translateY(0px)' },
          '50%': { transform: 'translateY(-14px)' },
        },
        orbit: {
          '0%': { transform: 'rotate(0deg) translateX(60px) rotate(0deg)' },
          '100%': { transform: 'rotate(360deg) translateX(60px) rotate(-360deg)' },
        },
      },
      animation: {
        wiggle: 'wiggle 0.4s ease-in-out',
        'pop-in': 'popIn 0.4s cubic-bezier(0.34, 1.56, 0.64, 1)',
        meteor: 'meteor linear infinite',
        float: 'float 4s ease-in-out infinite',
        orbit: 'orbit 8s linear infinite',
      },
    },
  },
  plugins: [],
}