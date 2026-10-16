import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Room, RoomRequest } from '../models/room.model';

@Injectable({ providedIn: 'root' })
export class RoomService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/rooms`;

  readonly rooms = signal<Room[]>([]);

  loadAll(): Observable<Room[]> {
    return this.http.get<Room[]>(this.baseUrl).pipe(tap((rooms) => this.rooms.set(rooms)));
  }

  getById(id: number): Observable<Room> {
    return this.http.get<Room>(`${this.baseUrl}/${id}`);
  }

  create(request: RoomRequest): Observable<Room> {
    return this.http.post<Room>(this.baseUrl, request);
  }

  update(id: number, request: RoomRequest): Observable<Room> {
    return this.http.put<Room>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
