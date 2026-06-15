import { createSelector } from '@ngrx/store';

import { authFeature } from './auth.reducer';

export const selectAuthSession = authFeature.selectSession;
export const selectLoginStatus = authFeature.selectLoginStatus;
export const selectSignUpStatus = authFeature.selectSignUpStatus;
export const selectIsAuthenticated = createSelector(selectAuthSession, (session) => {
  if (!session) {
    return false;
  }

  return new Date(session.expiresAt).getTime() > Date.now();
});
