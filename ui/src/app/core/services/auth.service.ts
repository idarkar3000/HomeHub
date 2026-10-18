import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

const TOKEN_KEY = 'homehub_token';
const EMAIL_KEY = 'homehub_email';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/auth`;

  private readonly token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private readonly email = signal<string | null>(localStorage.getItem(EMAIL_KEY));

  readonly authToken = this.token.asReadonly();
  readonly currentEmail = this.email.asReadonly();
  readonly isAuthenticated = computed(() => this.token() !== null);

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/login`, request)
      .pipe(tap((response) => this.setSession(response)));
  }

  register(request: RegisterRequest): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/register`, request);
  }

  logout(): void {
    this.token.set(null);
    this.email.set(null);
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EMAIL_KEY);
  }

  private setSession(response: AuthResponse): void {
    this.token.set(response.token);
    this.email.set(response.email);
    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(EMAIL_KEY, response.email);
  }
}
