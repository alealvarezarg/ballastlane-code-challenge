import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-page-state',
  standalone: true,
  imports: [NgIf, MatProgressSpinnerModule],
  template: `
    <section class="page-state">
      <mat-progress-spinner *ngIf="loading()" diameter="48" mode="indeterminate"></mat-progress-spinner>
      <h2 *ngIf="title()">{{ title() }}</h2>
      <p *ngIf="message()">{{ message() }}</p>
    </section>
  `,
  styles: [`
    .page-state {
      min-height: 240px;
      display: grid;
      place-items: center;
      gap: 0.75rem;
      padding: 2rem;
      text-align: center;
      color: var(--app-on-muted);
    }
    h2 { margin: 0; color: var(--app-on-surface); }
    p { margin: 0; max-width: 36rem; }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PageStateComponent {
  readonly loading = input(false);
  readonly title = input<string>('');
  readonly message = input<string>('');
}
