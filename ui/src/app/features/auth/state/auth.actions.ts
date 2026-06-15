import { createActionGroup, emptyProps, props } from '@ngrx/store';

import { ApiErrorState } from '@app/shared/models/api-error.models';
import { AuthenticatedSession, LoginRequest, ManagementUser, SignUpRequest } from '@app/shared/models/auth.models';

export const authActions = createActionGroup({
  source: 'Auth',
  events: {
    'Hydrate Session': emptyProps(),
    'Set Session': props<{ session: AuthenticatedSession }>(),
    'Clear Session': emptyProps(),
    'Sign Up Requested': props<{ request: SignUpRequest }>(),
    'Sign Up Succeeded': props<{ user: ManagementUser }>(),
    'Sign Up Failed': props<{ error: ApiErrorState }>(),
    'Login Requested': props<{ request: LoginRequest }>(),
    'Login Succeeded': props<{ session: AuthenticatedSession }>(),
    'Login Failed': props<{ error: ApiErrorState }>()
  }
});
