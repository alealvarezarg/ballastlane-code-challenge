import { EnvironmentProviders, Provider } from '@angular/core';
import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';

import { TasksEffects } from './state/tasks.effects';
import { tasksFeature } from './state/tasks.reducer';

export const taskFeatureProviders: Array<Provider | EnvironmentProviders> = [
  provideState(tasksFeature),
  provideEffects(TasksEffects)
];
