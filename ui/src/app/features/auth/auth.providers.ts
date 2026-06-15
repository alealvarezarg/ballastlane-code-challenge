import { EnvironmentProviders, Provider } from '@angular/core';
import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';

import { AuthEffects } from './state/auth.effects';
import { authFeature } from './state/auth.reducer';

export const authFeatureProviders: Array<Provider | EnvironmentProviders> = [
  provideState(authFeature),
  provideEffects(AuthEffects)
];
