import { Injectable } from '@angular/core';

const SESSION_KEY = 'tms.ui.session';

@Injectable({ providedIn: 'root' })
export class SessionStorageService {
  get<T>(key: string = SESSION_KEY): T | null {
    const value = globalThis.localStorage?.getItem(key);

    return value ? (JSON.parse(value) as T) : null;
  }

  set<T>(value: T, key: string = SESSION_KEY): void {
    globalThis.localStorage?.setItem(key, JSON.stringify(value));
  }

  remove(key: string = SESSION_KEY): void {
    globalThis.localStorage?.removeItem(key);
  }
}
