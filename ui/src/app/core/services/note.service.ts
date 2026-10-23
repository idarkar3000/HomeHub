import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Note, NoteKind, NoteRequest } from '../models/note.model';

@Injectable({ providedIn: 'root' })
export class NoteService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/notes`;

  readonly notes = signal<Note[]>([]);

  loadAll(kind?: NoteKind): Observable<Note[]> {
    const params = kind ? new HttpParams().set('kind', kind) : undefined;
    return this.http
      .get<Note[]>(this.baseUrl, { params })
      .pipe(tap((notes) => this.notes.set(notes)));
  }

  create(request: NoteRequest): Observable<Note> {
    return this.http.post<Note>(this.baseUrl, request);
  }

  update(id: number, request: NoteRequest): Observable<Note> {
    return this.http.put<Note>(`${this.baseUrl}/${id}`, request);
  }

  toggle(id: number): Observable<Note> {
    return this.http.patch<Note>(`${this.baseUrl}/${id}/toggle`, {});
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
