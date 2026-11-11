import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let http: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
    localStorage.clear();
  });

  it('empieza sin sesión', () => {
    expect(service.isAuthenticated()).toBe(false);
    expect(service.authToken()).toBeNull();
    expect(service.currentEmail()).toBeNull();
  });

  it('guarda el token y el email al iniciar sesión', () => {
    service.login({ email: 'ana@homehub.com', password: 'secreta123' }).subscribe();

    const request = http.expectOne(`${environment.apiUrl}/api/auth/login`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      email: 'ana@homehub.com',
      password: 'secreta123',
    });

    request.flush({ token: 'jwt-token', email: 'ana@homehub.com' });

    expect(service.authToken()).toBe('jwt-token');
    expect(service.currentEmail()).toBe('ana@homehub.com');
    expect(service.isAuthenticated()).toBe(true);
    expect(localStorage.getItem('homehub_token')).toBe('jwt-token');
    expect(localStorage.getItem('homehub_email')).toBe('ana@homehub.com');
  });

  it('envía el registro a la API', () => {
    service.register({ email: 'nueva@homehub.com', password: 'secreta123' }).subscribe();

    const request = http.expectOne(`${environment.apiUrl}/api/auth/register`);
    expect(request.request.method).toBe('POST');
    request.flush({ id: 1, email: 'nueva@homehub.com' });
  });

  it('limpia la sesión al cerrar sesión', () => {
    service.login({ email: 'ana@homehub.com', password: 'secreta123' }).subscribe();
    http
      .expectOne(`${environment.apiUrl}/api/auth/login`)
      .flush({ token: 'jwt-token', email: 'ana@homehub.com' });

    service.logout();

    expect(service.isAuthenticated()).toBe(false);
    expect(service.authToken()).toBeNull();
    expect(service.currentEmail()).toBeNull();
    expect(localStorage.getItem('homehub_token')).toBeNull();
    expect(localStorage.getItem('homehub_email')).toBeNull();
  });
});
