import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'rooms' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.LoginPage),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then((m) => m.RegisterPage),
  },
  {
    path: 'rooms',
    canActivate: [authGuard],
    loadComponent: () => import('./features/rooms/rooms-list/rooms-list').then((m) => m.RoomsList),
  },
  {
    path: 'rooms/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/rooms/room-detail/room-detail').then((m) => m.RoomDetail),
  },
  {
    path: 'calendar',
    canActivate: [authGuard],
    loadComponent: () => import('./features/calendar/calendar').then((m) => m.CalendarPage),
  },
  {
    path: 'notes',
    canActivate: [authGuard],
    loadComponent: () => import('./features/notes/notes').then((m) => m.NotesPage),
  },
  { path: '**', redirectTo: 'rooms' },
];
