import { ChangeDetectionStrategy, Component, EventEmitter, Output, input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  template: `
    <form class="task-form" [formGroup]="form()" (ngSubmit)="save.emit()">
      <mat-form-field appearance="outline"><mat-label>Title</mat-label><input matInput formControlName="title"></mat-form-field>
      <mat-form-field appearance="outline"><mat-label>Description</mat-label><textarea matInput rows="3" formControlName="description"></textarea></mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Status</mat-label>
        <mat-select formControlName="status">
          <mat-option value="Pending">Pending</mat-option>
          <mat-option value="InProgress">In progress</mat-option>
          <mat-option value="Completed">Completed</mat-option>
        </mat-select>
      </mat-form-field>
      <mat-form-field appearance="outline"><mat-label>Due date</mat-label><input matInput type="datetime-local" formControlName="dueDate"></mat-form-field>
      <mat-form-field appearance="outline"><mat-label>User Id</mat-label><input matInput formControlName="userId"></mat-form-field>
      <div class="task-form__actions">
        <button mat-button type="button" (click)="cancel.emit()">Cancel</button>
        <button mat-flat-button color="primary" type="submit">{{ submitLabel() }}</button>
      </div>
    </form>
  `,
  styles: [`
    .task-form{display:grid;gap:1rem;}
    .task-form mat-form-field{width:100%;}
    .task-form__actions{display:flex;justify-content:flex-end;gap:0.75rem;flex-wrap:wrap;}
    @media (max-width: 640px){.task-form__actions{justify-content:stretch;}.task-form__actions button{flex:1 1 100%;}}
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskFormComponent {
  readonly form = input.required<any>();
  readonly submitLabel = input('Save task');
  @Output() readonly save = new EventEmitter<void>();
  @Output() readonly cancel = new EventEmitter<void>();
}
