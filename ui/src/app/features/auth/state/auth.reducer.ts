import { createFeature, createReducer, on } from '@ngrx/store';

import { AuthenticatedSession } from '@app/shared/models/auth.models';
import { RequestStatus, initialRequestStatus } from '@app/shared/models/request-status.model';

import { authActions } from './auth.actions';

export interface AuthState {
  session: AuthenticatedSession | null;
  signUpStatus: RequestStatus;
  loginStatus: RequestStatus;
}

const initialState: AuthState = {
  session: null,
  signUpStatus: initialRequestStatus(),
  loginStatus: initialRequestStatus()
};

export const authFeature = createFeature({
  name: 'auth',
  reducer: createReducer(
    initialState,
    on(authActions.setSession, authActions.loginSucceeded, (state, { session }) => ({
      ...state,
      session,
      loginStatus: {
        loading: false,
        successMessage: 'Login successful.',
        error: null
      }
    })),
    on(authActions.clearSession, (state) => ({
      ...state,
      session: null
    })),
    on(authActions.signUpRequested, (state) => ({
      ...state,
      signUpStatus: { loading: true, successMessage: null, error: null }
    })),
    on(authActions.signUpSucceeded, (state) => ({
      ...state,
      signUpStatus: { loading: false, successMessage: 'Account created successfully.', error: null }
    })),
    on(authActions.signUpFailed, (state, { error }) => ({
      ...state,
      signUpStatus: { loading: false, successMessage: null, error }
    })),
    on(authActions.loginRequested, (state) => ({
      ...state,
      loginStatus: { loading: true, successMessage: null, error: null }
    })),
    on(authActions.loginFailed, (state, { error }) => ({
      ...state,
      loginStatus: { loading: false, successMessage: null, error }
    }))
  )
});
