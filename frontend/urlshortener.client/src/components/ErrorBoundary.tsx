import { Component } from 'react';
import type { ReactNode } from 'react';
import { Card } from './ui/Card';
import { Button } from './ui/Button';
import { AlertTriangle } from 'lucide-react';

interface Props {
  children: ReactNode;
}

interface State {
  hasError: boolean;
}

export class ErrorBoundary extends Component<Props, State> {
  state: State = { hasError: false };

  static getDerivedStateFromError() {
    return { hasError: true };
  }

  componentDidCatch(error: Error) {
    console.error('Unhandled error:', error);
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="min-h-screen flex items-center justify-center p-6">
          <Card className="max-w-md text-center py-10" shadowColor="pink">
            <div className="w-16 h-16 rounded-full bg-secondary/20 flex items-center justify-center mx-auto mb-4">
              <AlertTriangle size={28} className="text-secondary" />
            </div>
            <h1 className="text-xl mb-2">Something went wrong</h1>
            <p className="font-body text-mutedForeground mb-6">
              Try refreshing the page. If this keeps happening, let us know.
            </p>
            <Button onClick={() => window.location.assign('/')}>Go home</Button>
          </Card>
        </div>
      );
    }
    return this.props.children;
  }
}