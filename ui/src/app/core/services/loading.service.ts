import { Injectable, computed, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly active = signal(0);
  readonly loading = computed(() => this.active() > 0);

  start(): void {
    this.active.update((value) => value + 1);
  }

  stop(): void {
    this.active.update((value) => Math.max(0, value - 1));
  }
}
