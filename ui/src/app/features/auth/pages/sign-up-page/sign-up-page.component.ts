import { NgIf } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { InlineFeedbackComponent } from '@app/shared/components/inline-feedback/inline-feedback.component';

import { applyApiErrors } from '@app/shared/utils/form-error-applier.util';

import { AuthFormShellComponent } from '../../components/auth-form-shell/auth-form-shell.component';
import { PasswordFieldComponent } from '../../components/password-field/password-field.component';
import { authActions } from '../../state/auth.actions';
import { authFeature } from '../../state/auth.reducer';
import { createSignUpForm } from '../../validators/sign-up-form.validators';

@Component({
  selector: 'app-sign-up-page',
  standalone: true,
  imports: [
    NgIf,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    AuthFormShellComponent,
    PasswordFieldComponent,
    InlineFeedbackComponent
  ],
  templateUrl: './sign-up-page.component.html',
  styles: [`
    .auth-form {
      display: grid;
      gap: 1rem;
    }
    .full-width {
      width: 100%;
    }
    .auth-form__field .mat-mdc-form-field-infix{
      min-height:68px;
      padding-top:1rem;
      padding-bottom:0.8rem;
    }
    .auth-form__field .mdc-notched-outline__leading,
    .auth-form__field .mdc-notched-outline__notch,
    .auth-form__field .mdc-notched-outline__trailing{
      border-width:1px !important;
      border-color:#334155 !important;
      border-radius:0 !important;
    }
    .auth-form__field .mat-mdc-floating-label{
      color:#334155 !important;
      font-size:0.9rem !important;
    }
    .auth-form__field input.mat-mdc-input-element::placeholder{
      color:#64748b;
      opacity:1;
    }
    .auth-form__intro {
      margin: 0;
      color: var(--app-on-muted);
      line-height: 1.5;
    }
    .auth-form__actions {
      display: flex;
      justify-content: flex-end;
      gap: 0.75rem;
      margin-top: 0.25rem;
    }
    .auth-form__button {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      min-width: 170px;
      min-height: 48px;
      padding: 0.85rem 1.35rem;
      border-radius: 14px;
      border: 1px solid var(--app-primary);
      background: var(--app-primary);
      color: #ffffff;
      font-weight: 600;
      font-family: inherit;
      font-size: 0.95rem;
      box-shadow: 0 14px 28px rgba(37, 99, 235, 0.18);
      transition:
        box-shadow 180ms ease,
        background-color 180ms ease;
    }
    .auth-form__button:hover:not(:disabled) {
      box-shadow: 0 18px 32px rgba(37, 99, 235, 0.22);
      background: var(--app-primary-hover);
    }
    .auth-form__button:disabled {
      border-color: var(--app-border);
      background: #e2e8f0;
      color: var(--app-on-muted);
      opacity: 1;
      box-shadow: none;
      cursor: not-allowed;
    }
    .auth-form__button-content {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 0.65rem;
      width: 100%;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SignUpPageComponent {
  private readonly store = inject(Store);
  protected readonly form = createSignUpForm();
  protected readonly status = this.store.selectSignal(authFeature.selectSignUpStatus);
  protected readonly submitting = computed(() => this.status().loading);
  protected readonly formMessage = computed(() => this.status().error?.message ?? null);

  constructor() {
    effect(() => {
      applyApiErrors(this.form, this.status().error);
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.store.dispatch(authActions.signUpRequested({ request: this.form.getRawValue() }));
  }
}
