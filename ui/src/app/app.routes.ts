import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'rooms' },
  {
    path: 'rooms',
    loadComponent: () => import('./features/rooms/rooms-list/rooms-list').then((m) => m.RoomsList),
  },
  {
    path: 'rooms/:id',
    loadComponent: () =>
      import('./features/rooms/room-detail/room-detail').then((m) => m.RoomDetail),
  },
  {
    path: 'calendar',
    loadComponent: () => import('./features/calendar/calendar').then((m) => m.CalendarPage),
  },
  {
    path: 'notes',
    loadComponent: () => import('./features/notes/notes').then((m) => m.NotesPage),
  },
  { path: '**', redirectTo: 'rooms' },
];
