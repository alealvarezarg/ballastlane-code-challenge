import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { APP_ENVIRONMENT } from '@app/core/providers/api-config.provider';
import { AppEnvironment } from '@app/shared/models/app-environment.model';
import { AuthenticatedSession, LoginRequest, ManagementUser, SignUpRequest } from '@app/shared/models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  constructor(
    private readonly httpClient: HttpClient,
    @Inject(APP_ENVIRONMENT) private readonly environment: AppEnvironment
  ) {}

  signUp(request: SignUpRequest): Observable<ManagementUser> {
    return this.httpClient.post<ManagementUser>(`${this.environment.apiBaseUrl}/management-users`, request);
  }

  login(request: LoginRequest): Observable<AuthenticatedSession> {
    return this.httpClient.post<AuthenticatedSession>(`${this.environment.apiBaseUrl}/management-users/login`, request);
  }
}
