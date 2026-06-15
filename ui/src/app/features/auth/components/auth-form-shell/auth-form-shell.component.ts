import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { filter, map, startWith } from 'rxjs';

@Component({
  selector: 'app-auth-form-shell',
  standalone: true,
  imports: [MatCardModule, RouterLink],
  template: `
    <section class="auth-shell">
      <div class="auth-shell__backdrop"></div>
      <mat-card class="auth-card">
        <div class="auth-card__header">
          <p class="auth-card__eyebrow">{{ eyebrow() }}</p>
          <h1 class="auth-card__title">{{ title() }}</h1>
          <p class="auth-card__subtitle">{{ subtitle() }}</p>
        </div>

        <nav class="auth-card__nav" aria-label="Authentication actions">
          <a
            class="auth-card__nav-link"
            [class.auth-card__nav-link--active]="isLoginRoute()"
            routerLink="/login"
            [queryParams]="preservedQueryParams()">
            Sign In
          </a>
          <a
            class="auth-card__nav-link"
            [class.auth-card__nav-link--active]="isSignUpRoute()"
            routerLink="/sign-up"
            [queryParams]="preservedQueryParams()">
            Sign Up
          </a>
        </nav>

        <mat-card-content class="auth-card__content">
          <ng-content></ng-content>
        </mat-card-content>
        <p class="auth-card__footer">{{ footer() }}</p>
      </mat-card>
    </section>
  `,
  styles: [`
    .auth-shell {
      position: relative;
      display: grid;
      place-items: center;
      width: 100%;
      min-height: 100dvh;
      padding: clamp(1rem, 3vw, 2rem);
      overflow-x: hidden;
      overflow-y: auto;
    }
    .auth-shell__backdrop {
      position: absolute;
      inset: 0;
      background:
        radial-gradient(circle at top left, rgba(11, 87, 208, 0.14), transparent 34%),
        radial-gradient(circle at bottom right, rgba(10, 63, 153, 0.1), transparent 28%),
        linear-gradient(180deg, #f7faff 0%, #eef4fb 100%);
    }
    .auth-card {
      position: relative;
      width: min(100%, 480px);
      max-width: 100%;
      padding: 1.25rem;
      border-radius: 28px;
      border: 1px solid rgba(214, 224, 235, 0.9);
      box-shadow: 0 24px 70px rgba(30, 50, 70, 0.12);
      background: rgba(255, 255, 255, 0.96);
    }
    .auth-card__header {
      display: grid;
      gap: 0.5rem;
      margin-bottom: 1.25rem;
    }
    .auth-card__eyebrow {
      margin: 0;
      font-size: 0.77rem;
      font-weight: 700;
      letter-spacing: 0.12em;
      text-transform: uppercase;
      color: var(--app-primary-strong);
    }
    .auth-card__title {
      margin: 0;
      font-size: clamp(1.8rem, 4vw, 2.4rem);
      line-height: 1.1;
      color: var(--app-on-surface);
    }
    .auth-card__subtitle {
      margin: 0;
      color: var(--app-on-muted);
      line-height: 1.5;
    }
    .auth-card__nav {
      display: grid;
      grid-template-columns: repeat(2, minmax(0, 1fr));
      gap: 0.4rem;
      padding: 0.4rem;
      margin-bottom: 1rem;
      border-radius: 18px;
      background: var(--app-surface-alt);
      border: 1px solid var(--app-border);
      box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.92);
    }
    .auth-card__nav-link {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 100%;
      min-height: 48px;
      padding: 0.85rem 1rem;
      border-radius: 14px;
      border: 1px solid transparent;
      background: transparent;
      color: var(--app-on-muted);
      font-weight: 700;
      text-decoration: none;
      line-height: 1;
      transition:
        background-color 180ms ease,
        border-color 180ms ease,
        box-shadow 180ms ease,
        color 180ms ease;
    }
    .auth-card__nav-link:hover {
      background: rgba(255, 255, 255, 0.7);
      color: var(--app-on-surface);
    }
    .auth-card__nav-link--active {
      background: var(--app-surface);
      color: var(--app-on-surface);
      border-color: var(--app-border);
      box-shadow: 0 6px 16px rgba(15, 23, 42, 0.08);
    }
    .auth-card__content {
      padding-top: 0.5rem;
    }
    .auth-card__footer {
      margin: 1rem 0 0;
      font-size: 0.9rem;
      color: var(--app-on-muted);
      text-align: center;
    }
    @media (max-width: 600px) {
      .auth-shell {
        place-items: stretch;
        align-items: center;
        padding: 0.75rem;
      }
      .auth-card {
        width: 100%;
        padding: 1rem;
        border-radius: 22px;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AuthFormShellComponent {
  private readonly router = inject(Router);
  private readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map((event) => event.urlAfterRedirects),
      startWith(this.router.url)
    ),
    { initialValue: this.router.url }
  );

  readonly title = input.required<string>();
  readonly subtitle = input<string>('');
  readonly eyebrow = input('Task Management System');
  readonly footer = input('Use your existing account or create a new one to continue.');
  protected readonly currentPath = computed(() => this.currentUrl().split('?')[0] ?? '');
  protected readonly isLoginRoute = computed(() => this.currentPath() === '/login');
  protected readonly isSignUpRoute = computed(() => this.currentPath() === '/sign-up');
  protected readonly preservedQueryParams = computed(() => this.router.parseUrl(this.currentUrl()).queryParams);
}
