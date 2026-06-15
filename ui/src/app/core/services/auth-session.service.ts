import { Injectable, computed, signal } from '@angular/core';

import { AuthenticatedSession } from '@app/shared/models/auth.models';

import { SessionStorageService } from './session-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  private readonly sessionState = signal<AuthenticatedSession | null>(null);
  readonly session = this.sessionState.asReadonly();
  readonly isAuthenticated = computed(() => {
    const session = this.sessionState();
    if (!session) {
      return false;
    }

    return new Date(session.expiresAt).getTime() > Date.now();
  });

  constructor(private readonly storage: SessionStorageService) {
    this.hydrate();
  }

  hydrate(): void {
    this.sessionState.set(this.storage.get<AuthenticatedSession>());
  }

  setSession(session: AuthenticatedSession): void {
    this.storage.set(session);
    this.sessionState.set(session);
  }

  clearSession(): void {
    this.storage.remove();
    this.sessionState.set(null);
  }
}
