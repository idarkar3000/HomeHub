import { DOCUMENT } from '@angular/common';
import { Injectable, computed, effect, inject, signal } from '@angular/core';

const THEME_KEY = 'homehub_theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private readonly systemDark = window.matchMedia('(prefers-color-scheme: dark)').matches;

  private readonly preference = signal<'light' | 'dark' | null>(
    (localStorage.getItem(THEME_KEY) as 'light' | 'dark' | null) ?? null,
  );

  readonly isDark = computed(() =>
    this.preference() === null ? this.systemDark : this.preference() === 'dark',
  );

  constructor() {
    effect(() => {
      const preference = this.preference();
      if (preference === null) {
        this.document.body.style.removeProperty('color-scheme');
      } else {
        this.document.body.style.colorScheme = preference;
        localStorage.setItem(THEME_KEY, preference);
      }
    });
  }

  toggle(): void {
    this.preference.set(this.isDark() ? 'light' : 'dark');
  }
}
