import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { APP_ENVIRONMENT } from '@app/core/providers/api-config.provider';

import { AuthApiService } from './auth-api.service';

describe('AuthApiService', () => {
  let service: AuthApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: APP_ENVIRONMENT,
          useValue: { apiBaseUrl: 'http://localhost:5000' }
        }
      ]
    });

    service = TestBed.inject(AuthApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('posts login requests to the login endpoint', () => {
    service.login({ email: 'manager@example.com', password: 'password' }).subscribe();

    const request = httpMock.expectOne('http://localhost:5000/management-users/login');
    expect(request.request.method).toBe('POST');
  });
});
