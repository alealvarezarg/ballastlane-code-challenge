import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-inline-feedback',
  standalone: true,
  imports: [NgIf],
  template: `
    <p *ngIf="message()" class="inline-feedback" [class.error]="tone() === 'error'">
      {{ message() }}
    </p>
  `,
  styles: [`
    .inline-feedback { margin: 0; font-size: 0.875rem; color: var(--app-on-muted); }
    .inline-feedback.error { color: var(--app-danger); }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InlineFeedbackComponent {
  readonly message = input<string | null>(null);
  readonly tone = input<'error' | 'info'>('info');
}
