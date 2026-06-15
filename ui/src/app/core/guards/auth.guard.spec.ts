import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';

import { AuthSessionService } from '@app/core/services/auth-session.service';

import { authGuard } from './auth.guard';

describe('authGuard', () => {
  it('redirects unauthenticated users to login', () => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: AuthSessionService,
          useValue: { isAuthenticated: () => false }
        },
        {
          provide: Router,
          useValue: { createUrlTree: (...args: unknown[]) => args }
        }
      ]
    });

    const result = TestBed.runInInjectionContext(() => authGuard({} as never, { url: '/tasks' } as never)) as unknown;
    expect(result).toEqual([['/login'], { queryParams: { redirectUrl: '/tasks' } }]);
  });
});
