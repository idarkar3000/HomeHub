import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TaskItem, TaskRequest } from '../models/task.model';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/tasks`;

  readonly tasks = signal<TaskItem[]>([]);

  getAll(): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(this.baseUrl);
  }

  loadByRoom(roomId: number): Observable<TaskItem[]> {
    return this.http
      .get<TaskItem[]>(`${environment.apiUrl}/api/rooms/${roomId}/tasks`)
      .pipe(tap((tasks) => this.tasks.set(tasks)));
  }

  create(roomId: number, request: TaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(`${environment.apiUrl}/api/rooms/${roomId}/tasks`, request);
  }

  update(id: number, request: TaskRequest): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.baseUrl}/${id}`, request);
  }

  toggle(id: number): Observable<TaskItem> {
    return this.http.patch<TaskItem>(`${this.baseUrl}/${id}/toggle`, {});
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
