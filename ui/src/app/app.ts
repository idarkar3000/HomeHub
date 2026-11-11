import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from './core/services/auth.service';
import { LoadingService } from './core/services/loading.service';
import { RealtimeService } from './core/services/realtime.service';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatProgressBarModule,
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly breakpointObserver = inject(BreakpointObserver);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly realtime = inject(RealtimeService);
  private readonly themeService = inject(ThemeService);

  readonly isAuthenticated = this.auth.isAuthenticated;
  readonly loading = inject(LoadingService).loading;
  readonly sidenavOpen = signal(true);
  readonly isDark = this.themeService.isDark;
  readonly liveConnected = this.realtime.connected;
  readonly liveStatus = this.realtime.status;
  readonly liveLabel = computed(() => {
    switch (this.realtime.status()) {
      case 'connected':
        return 'Sincronización en directo';
      case 'connecting':
        return 'Conectando…';
      case 'reconnecting':
        return 'Reconectando…';
      default:
        return 'Sin conexión en directo';
    }
  });

  readonly isHandset = toSignal(
    this.breakpointObserver.observe(Breakpoints.Handset).pipe(map((result) => result.matches)),
    { initialValue: false },
  );

  readonly navItems = [
    { label: 'Hogar', icon: 'home', path: '/rooms' },
    { label: 'Calendario', icon: 'calendar_month', path: '/calendar' },
    { label: 'Notas', icon: 'sticky_note_2', path: '/notes' },
  ];

  toggleSidenav(): void {
    this.sidenavOpen.update((open) => !open);
  }

  constructor() {
    effect(() => {
      if (this.isAuthenticated()) {
        this.realtime.start();
      } else {
        this.realtime.stop();
      }
    });
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  toggleTheme(): void {
    this.themeService.toggle();
  }
}
