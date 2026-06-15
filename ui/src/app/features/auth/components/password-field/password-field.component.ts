import { NgIf } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-password-field',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, MatFormFieldModule, MatInputModule],
  template: `
    <mat-form-field appearance="outline" floatLabel="always" class="full-width auth-password-field">
      <mat-label>{{ label() }}</mat-label>
      <input matInput [formControl]="control()" type="password" autocomplete="current-password" [placeholder]="label()" />
      <mat-hint>Minimum 8 characters</mat-hint>
      <mat-error *ngIf="control().errors?.['required']">Password is required.</mat-error>
      <mat-error *ngIf="control().errors?.['minlength']">Password must be at least 8 characters.</mat-error>
      <mat-error *ngIf="control().errors?.['api']">{{ control().errors?.['api'] }}</mat-error>
    </mat-form-field>
  `,
  styles: [`
    .full-width{width:100%;}
    .auth-password-field{
      width:100%;
    }
    .auth-password-field .mat-mdc-form-field-infix{
      min-height:68px;
      padding-top:1rem;
      padding-bottom:0.8rem;
    }
    .auth-password-field .mdc-notched-outline__leading,
    .auth-password-field .mdc-notched-outline__notch,
    .auth-password-field .mdc-notched-outline__trailing{
      border-width:1px !important;
      border-color:#334155 !important;
    }
    .auth-password-field .mat-mdc-floating-label{
      color:#334155 !important;
      font-size:0.9rem !important;
    }
    .auth-password-field input.mat-mdc-input-element::placeholder{
      color:#64748b;
      opacity:1;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PasswordFieldComponent {
  readonly label = input('Password');
  readonly control = input.required<any>();
}
