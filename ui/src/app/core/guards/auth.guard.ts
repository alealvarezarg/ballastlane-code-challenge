import { inject } from '@angular/core';
import { CanActivateFn, CanMatchFn, Route, Router, UrlSegment } from '@angular/router';

import { AuthSessionService } from '@app/core/services/auth-session.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const authSession = inject(AuthSessionService);
  const router = inject(Router);

  if (authSession.isAuthenticated()) {
    return true;
  }

  return router.createUrlTree(['/login'], {
    queryParams: { redirectUrl: state.url }
  });
};

export const authMatchGuard: CanMatchFn = (route: Route, segments: UrlSegment[]) => {
  const authSession = inject(AuthSessionService);
  const router = inject(Router);

  if (authSession.isAuthenticated()) {
    return true;
  }

  const attemptedUrl = `/${segments.map((segment) => segment.path).join('/') || route.path || ''}`;

  return router.createUrlTree(['/login'], {
    queryParams: { redirectUrl: attemptedUrl }
  });
};
