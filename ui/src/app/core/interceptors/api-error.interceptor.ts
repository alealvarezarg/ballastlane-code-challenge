import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { AuthSessionService } from '@app/core/services/auth-session.service';

export const apiErrorInterceptor: HttpInterceptorFn = (request, next) => {
  const authSession = inject(AuthSessionService);
  const router = inject(Router);

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        authSession.clearSession();
        void router.navigate(['/login'], {
          queryParams: { redirectUrl: router.url }
        });
      }

      return throwError(() => error);
    })
  );
};
