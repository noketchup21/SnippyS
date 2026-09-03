import { Link } from 'react-router-dom';
import { Button } from '../components/ui/Button';
import { Compass } from 'lucide-react';

export function NotFound() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-dot-grid px-4">
      <div className="text-center">
        <div className="w-16 h-16 rounded-full bg-tertiary/30 flex items-center justify-center mx-auto mb-4">
          <Compass size={28} />
        </div>
        <h1 className="text-3xl mb-2">404</h1>
        <p className="font-body text-mutedForeground mb-6">This page doesn't exist.</p>
        <Link to="/"><Button>Go home</Button></Link>
      </div>
    </div>
  );
}