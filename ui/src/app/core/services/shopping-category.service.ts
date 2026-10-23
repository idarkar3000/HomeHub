import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ShoppingCategory } from '../models/shopping-category.model';

@Injectable({ providedIn: 'root' })
export class ShoppingCategoryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/shopping-categories`;

  readonly categories = signal<ShoppingCategory[]>([]);

  loadAll(): Observable<ShoppingCategory[]> {
    return this.http
      .get<ShoppingCategory[]>(this.baseUrl)
      .pipe(tap((categories) => this.categories.set(categories)));
  }

  create(name: string): Observable<ShoppingCategory> {
    return this.http.post<ShoppingCategory>(this.baseUrl, { name });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
