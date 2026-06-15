import { Routes } from '@angular/router';

import { LoginPageComponent } from './pages/login-page/login-page.component';
import { SignUpPageComponent } from './pages/sign-up-page/sign-up-page.component';

export const AUTH_ROUTES: Routes = [
  {
    path: '',
    component: LoginPageComponent
  }
];

export const SIGN_UP_ROUTES: Routes = [
  {
    path: '',
    component: SignUpPageComponent
  }
];
