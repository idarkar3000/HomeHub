import { Injectable, computed, inject, signal } from '@angular/core';
import type { HubConnection } from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

export type RealtimeStatus = 'idle' | 'connecting' | 'connected' | 'reconnecting' | 'offline';

const MAX_RETRY_DELAY_MS = 30_000;
const BASE_RETRY_DELAY_MS = 2_000;

@Injectable({ providedIn: 'root' })
export class RealtimeService {
  private readonly auth = inject(AuthService);
  private connection: HubConnection | null = null;
  private starting = false;
  private stopped = true;
  private retryAttempt = 0;
  private retryTimer: ReturnType<typeof setTimeout> | null = null;
  private readonly listeners = new Set<() => void>();
  private readonly statusState = signal<RealtimeStatus>('idle');

  readonly status = this.statusState.asReadonly();
  readonly connected = computed(() => this.statusState() === 'connected');

  start(): void {
    if (this.connection || this.starting || !this.auth.isAuthenticated()) {
      return;
    }

    this.stopped = false;
    this.retryAttempt = 0;
    void this.connect();
  }

  stop(): void {
    this.stopped = true;
    this.clearRetry();

    const connection = this.connection;
    this.connection = null;
    this.statusState.set('idle');
    connection?.stop().catch(() => {});
  }

  onNotesChanged(listener: () => void): () => void {
    this.listeners.add(listener);
    return () => this.listeners.delete(listener);
  }

  private async connect(): Promise<void> {
    if (this.stopped || this.starting || this.connection) {
      return;
    }

    if (!this.auth.isAuthenticated()) {
      this.statusState.set('idle');
      return;
    }

    this.starting = true;
    this.statusState.set(this.retryAttempt === 0 ? 'connecting' : 'reconnecting');

    try {
      const { HubConnectionBuilder } = await import('@microsoft/signalr');

      if (this.stopped || !this.auth.isAuthenticated()) {
        return;
      }

      const connection = new HubConnectionBuilder()
        .withUrl(`${environment.apiUrl}/hubs/notes`, {
          accessTokenFactory: () => this.auth.authToken() ?? '',
        })
        .withAutomaticReconnect([0, 2_000, 5_000, 10_000, 30_000])
        .build();

      connection.on('notesChanged', () => this.emit());
      connection.onreconnecting(() => this.statusState.set('reconnecting'));
      connection.onreconnected(() => {
        this.retryAttempt = 0;
        this.statusState.set('connected');
        this.emit();
      });
      connection.onclose(() => {
        this.connection = null;

        if (this.stopped) {
          return;
        }

        this.statusState.set('offline');
        this.scheduleRetry();
      });

      await connection.start();

      if (this.stopped) {
        await connection.stop().catch(() => {});
        return;
      }

      this.connection = connection;
      this.retryAttempt = 0;
      this.statusState.set('connected');
    } catch {
      this.statusState.set('offline');
      this.scheduleRetry();
    } finally {
      this.starting = false;
    }
  }

  private scheduleRetry(): void {
    if (this.stopped || this.retryTimer || !this.auth.isAuthenticated()) {
      return;
    }

    const delay = Math.min(MAX_RETRY_DELAY_MS, BASE_RETRY_DELAY_MS * 2 ** this.retryAttempt);
    this.retryAttempt += 1;

    this.retryTimer = setTimeout(() => {
      this.retryTimer = null;
      void this.connect();
    }, delay);
  }

  private clearRetry(): void {
    if (this.retryTimer) {
      clearTimeout(this.retryTimer);
      this.retryTimer = null;
    }
  }

  private emit(): void {
    for (const listener of this.listeners) {
      listener();
    }
  }
}
