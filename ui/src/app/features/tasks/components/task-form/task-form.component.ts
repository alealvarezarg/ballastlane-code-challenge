import { ChangeDetectionStrategy, Component, ElementRef, EventEmitter, Output, input, viewChild } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  template: `
    <form class="task-form" [formGroup]="form()" (ngSubmit)="save.emit()">
      <mat-form-field appearance="outline" floatLabel="always" class="task-form__field">
        <mat-label>Title</mat-label>
        <input matInput formControlName="title" placeholder="Title" />
      </mat-form-field>
      <mat-form-field appearance="outline" floatLabel="always" class="task-form__field task-form__field--textarea">
        <mat-label>Description</mat-label>
        <textarea matInput rows="4" formControlName="description" placeholder="Description"></textarea>
      </mat-form-field>
      <mat-form-field appearance="outline" floatLabel="always" class="task-form__field">
        <mat-label>Due date</mat-label>
        <input #dueDateInput matInput type="datetime-local" formControlName="dueDate" placeholder="Due date" step="60" />
        <button class="task-form__picker-trigger" mat-icon-button matSuffix type="button" (click)="openDueDatePicker()" aria-label="Choose due date and time">
          <span aria-hidden="true">📅</span>
        </button>
      </mat-form-field>
      <div class="task-form__actions">
        <button class="task-form__action task-form__action--secondary" mat-stroked-button type="button" (click)="clear.emit()">{{ clearLabel() }}</button>
        <button class="task-form__action task-form__action--primary" mat-flat-button color="primary" type="submit">{{ submitLabel() }}</button>
      </div>
    </form>
  `,
  styles: [`
    .task-form{display:grid;gap:1.2rem;}
    .task-form mat-form-field{width:100%;}
    .task-form__field .mat-mdc-form-field-infix{
      min-height:68px;
      padding-top:1rem;
      padding-bottom:0.8rem;
    }
    .task-form__field--textarea .mat-mdc-form-field-infix{
      min-height:120px;
    }
    .task-form__field .mdc-notched-outline__leading,
    .task-form__field .mdc-notched-outline__notch,
    .task-form__field .mdc-notched-outline__trailing{
      border-width:1px !important;
      border-color:#334155 !important;
      border-radius:0 !important;
    }
    .task-form__field .mat-mdc-floating-label{
      color:#334155 !important;
      font-size:0.9rem !important;
    }
    .task-form__field .mat-mdc-input-element::placeholder{
      color:#64748b;
      opacity:1;
    }
    .task-form__picker-trigger{
      color:var(--app-primary) !important;
    }
    .task-form__actions{
      display:flex;
      justify-content:flex-end;
      gap:0.75rem;
      flex-wrap:wrap;
      padding-top:0.25rem;
    }
    .task-form__action{
      display:inline-flex;
      align-items:center;
      justify-content:center;
      min-height:44px;
      min-width:132px;
      border-radius:999px;
      padding-inline:1.15rem;
      font-weight:700;
      font-family:inherit;
      font-size:0.95rem;
      border-width:1px !important;
      transition:
        box-shadow 180ms ease,
        background-color 180ms ease,
        border-color 180ms ease,
        color 180ms ease;
    }
    .task-form__action--secondary{
      background:#ffffff !important;
      border-color:var(--app-border) !important;
      color:var(--app-on-surface) !important;
      box-shadow:0 10px 22px rgba(15, 23, 42, 0.08) !important;
    }
    .task-form__action--primary{
      border-color:var(--app-primary) !important;
      background:var(--app-primary) !important;
      color:#ffffff !important;
      box-shadow:0 14px 28px rgba(37, 99, 235, 0.18) !important;
    }
    .task-form__action--secondary:hover{
      background:var(--app-surface-alt) !important;
      box-shadow:0 14px 26px rgba(15, 23, 42, 0.1) !important;
    }
    .task-form__action--primary:hover{
      background:var(--app-primary-hover) !important;
      box-shadow:0 18px 32px rgba(37, 99, 235, 0.22) !important;
    }
    @media (max-width: 640px){
      .task-form__actions{justify-content:stretch;}
      .task-form__action{flex:1 1 100%;}
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskFormComponent {
  private readonly dueDateInput = viewChild<ElementRef<HTMLInputElement>>('dueDateInput');
  readonly form = input.required<any>();
  readonly submitLabel = input('Create');
  readonly clearLabel = input('Clear');
  @Output() readonly save = new EventEmitter<void>();
  @Output() readonly clear = new EventEmitter<void>();
  @Output() readonly cancel = new EventEmitter<void>();

  openDueDatePicker(): void {
    const input = this.dueDateInput()?.nativeElement as HTMLInputElement | undefined;

    if (!input) {
      return;
    }

    const pickerInput = input as HTMLInputElement & { showPicker?: () => void };

    if (typeof pickerInput.showPicker === 'function') {
      pickerInput.showPicker();
      return;
    }

    input.focus();
    input.click();
  }
}
