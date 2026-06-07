import { DOCUMENT } from '@angular/common';
import { Injectable, inject, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

const storageKey = 'movie-journal.theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  readonly currentTheme = signal<ThemeMode>(this.readTheme());

  constructor() {
    this.applyTheme(this.currentTheme());
  }

  toggleTheme(): void {
    this.setTheme(this.currentTheme() === 'dark' ? 'light' : 'dark');
  }

  setTheme(theme: ThemeMode): void {
    localStorage.setItem(storageKey, theme);
    this.currentTheme.set(theme);
    this.applyTheme(theme);
  }

  private readTheme(): ThemeMode {
    return localStorage.getItem(storageKey) === 'dark' ? 'dark' : 'light';
  }

  private applyTheme(theme: ThemeMode): void {
    this.document.documentElement.classList.toggle('dark', theme === 'dark');
    this.document.documentElement.style.colorScheme = theme;
  }
}
