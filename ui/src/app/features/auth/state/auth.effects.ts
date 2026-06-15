import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, tap, exhaustMap } from 'rxjs';

import { NotificationService } from '@app/core/services/notification.service';
import { AuthSessionService } from '@app/core/services/auth-session.service';
import { mapHttpError } from '@app/shared/utils/api-error.mapper';

import { AuthApiService } from '../services/auth-api.service';
import { authActions } from './auth.actions';

@Injectable()
export class AuthEffects {
  private readonly actions$ = inject(Actions);
  private readonly authApiService = inject(AuthApiService);
  private readonly notificationService = inject(NotificationService);
  private readonly authSessionService = inject(AuthSessionService);
  private readonly router = inject(Router);

  readonly signUp$ = createEffect(() =>
    this.actions$.pipe(
      ofType(authActions.signUpRequested),
      exhaustMap(({ request }) =>
        this.authApiService.signUp(request).pipe(
          map((user) => authActions.signUpSucceeded({ user })),
          catchError((error) => of(authActions.signUpFailed({ error: mapHttpError(error) })))
        )
      )
    )
  );

  readonly signUpSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(authActions.signUpSucceeded),
        tap(() => {
          this.notificationService.success('Account created. You can sign in now.');
          void this.router.navigate(['/login']);
        })
      ),
    { dispatch: false }
  );

  readonly login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(authActions.loginRequested),
      exhaustMap(({ request }) =>
        this.authApiService.login(request).pipe(
          map((session) => authActions.loginSucceeded({ session })),
          catchError((error) => of(authActions.loginFailed({ error: mapHttpError(error) })))
        )
      )
    )
  );

  readonly loginSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(authActions.loginSucceeded),
        tap(({ session }) => {
          this.authSessionService.setSession(session);
          this.notificationService.success('Welcome back.');
          void this.router.navigate(['/tasks']);
        })
      ),
    { dispatch: false }
  );

  readonly loginFailed$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(authActions.loginFailed),
        tap(({ error }) => this.notificationService.error(error.message))
      ),
    { dispatch: false }
  );
}
