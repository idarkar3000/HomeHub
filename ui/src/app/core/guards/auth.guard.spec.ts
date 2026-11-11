import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { AuthService } from '../services/auth.service';
import { authGuard } from './auth.guard';

describe('authGuard', () => {
  const navigate = vi.fn();

  beforeEach(() => {
    navigate.mockClear();
  });

  function runGuard(isAuthenticated: boolean): unknown {
    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: { isAuthenticated: () => isAuthenticated } },
        { provide: Router, useValue: { navigate } },
      ],
    });

    return TestBed.runInInjectionContext(() => authGuard({} as never, {} as never));
  }

  it('permite el acceso cuando hay sesión', () => {
    expect(runGuard(true)).toBe(true);
    expect(navigate).not.toHaveBeenCalled();
  });

  it('redirige al login cuando no hay sesión', () => {
    expect(runGuard(false)).toBe(false);
    expect(navigate).toHaveBeenCalledWith(['/login']);
  });
});
