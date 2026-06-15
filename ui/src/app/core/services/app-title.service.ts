import { Injectable, inject } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Injectable({ providedIn: 'root' })
export class AppTitleService {
  private readonly title = inject(Title);

  set(title: string): void {
    this.title.setTitle(`Task Management System | ${title}`);
  }
}
