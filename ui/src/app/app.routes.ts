import { Routes } from '@angular/router';

import { authGuard, authMatchGuard } from '@app/core/guards/auth.guard';
import { publicOnlyGuard } from '@app/core/guards/public-only.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'tasks'
  },
  {
    path: 'login',
    canActivate: [publicOnlyGuard],
    loadChildren: () => import('@app/features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
    title: 'Login'
  },
  {
    path: 'sign-up',
    canActivate: [publicOnlyGuard],
    loadChildren: () => import('@app/features/auth/auth.routes').then((m) => m.SIGN_UP_ROUTES),
    title: 'Sign Up'
  },
  {
    path: 'tasks',
    canMatch: [authMatchGuard],
    canActivate: [authGuard],
    loadChildren: () => import('@app/features/tasks/tasks.routes').then((m) => m.TASK_ROUTES),
    title: 'Tasks'
  },
  {
    path: '**',
    redirectTo: 'tasks'
  }
];
