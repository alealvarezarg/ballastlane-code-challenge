import { InjectionToken } from '@angular/core';

import { AppEnvironment } from '@app/shared/models/app-environment.model';

export const APP_ENVIRONMENT = new InjectionToken<AppEnvironment>('app.environment');
