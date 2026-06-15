import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { AuthSessionService } from '@app/core/services/auth-session.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const session = inject(AuthSessionService).session();

  if (!session) {
    return next(request);
  }

  return next(request.clone({
    setHeaders: {
      Authorization: `${session.tokenType} ${session.accessToken}`
    }
  }));
};
