export const REDIRECT_BASE_URL = import.meta.env.VITE_REDIRECT_BASE_URL;

export function buildShortUrl(shortCode: string): string {
  return `${REDIRECT_BASE_URL}/${shortCode}`;
}