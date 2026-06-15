import { authActions } from './auth.actions';
import { authFeature } from './auth.reducer';

describe('auth reducer', () => {
  it('stores the session on login success', () => {
    const state = authFeature.reducer(undefined, authActions.loginSucceeded({
      session: {
        accessToken: 'token',
        tokenType: 'Bearer',
        expiresAt: '2099-01-01T00:00:00Z',
        user: { id: '1', username: 'manager', email: 'manager@example.com' }
      }
    }));

    expect(state.session?.accessToken).toBe('token');
    expect(state.loginStatus.successMessage).toBeTruthy();
  });
});
