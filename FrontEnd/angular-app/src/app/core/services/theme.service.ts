import { Injectable, signal, effect, inject, PLATFORM_ID} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type Theme = 'light' | 'dark';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly THEME_KEY = 'app-theme';
  private readonly themes: Theme[] = ['light', 'dark'];
  private platformId = inject(PLATFORM_ID);
  currentTheme = signal<Theme>(this.getInitialTheme());

  constructor() {
    // Effect to update the DOM when theme changes
    effect(() => {
      this.applyTheme(this.currentTheme());
    });
  }

  private getInitialTheme(): Theme {
    // Check localStorage
    if (!isPlatformBrowser(this.platformId)) {
      return 'light';
    }

    const stored = localStorage.getItem(this.THEME_KEY) as Theme | null;
    if (stored && this.themes.includes(stored)) {
      return stored;
    }

    // Check system preference
    if (
      typeof window !== 'undefined' &&
      window.matchMedia('(prefers-color-scheme: dark)').matches
    ) {
      return 'dark';
    }

    return 'light';
  }

  private applyTheme(theme: Theme): void {
    if (typeof document === 'undefined') return;

    const htmlElement = document.documentElement;

    if (theme === 'dark') {
      htmlElement.classList.add('dark');
    } else {
      htmlElement.classList.remove('dark');
    }

    localStorage.setItem(this.THEME_KEY, theme);
  }

  toggleTheme(): void {
    const newTheme = this.currentTheme() === 'light' ? 'dark' : 'light';
    this.currentTheme.set(newTheme);
  }

  setTheme(theme: Theme): void {
    if (this.themes.includes(theme)) {
      this.currentTheme.set(theme);
    }
  }
}
