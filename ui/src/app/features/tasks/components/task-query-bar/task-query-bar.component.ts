import { ChangeDetectionStrategy, Component, EventEmitter, Output, input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-task-query-bar',
  standalone: true,
  imports: [ReactiveFormsModule, MatCheckboxModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  template: `
    <form class="query-grid" [formGroup]="form()">
      <mat-form-field appearance="outline" floatLabel="always">
        <mat-label>Search</mat-label>
        <input matInput formControlName="search" placeholder="Search" />
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Status</mat-label>
        <mat-select formControlName="status">
          <mat-option value="">All</mat-option>
          <mat-option value="Pending">Pending</mat-option>
          <mat-option value="InProgress">In progress</mat-option>
          <mat-option value="Completed">Completed</mat-option>
        </mat-select>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Sort by</mat-label>
        <mat-select formControlName="sortBy">
          <mat-option value="">&nbsp;</mat-option>
          <mat-option value="dueDate">Due date</mat-option>
          <mat-option value="title">Title</mat-option>
          <mat-option value="status">Status</mat-option>
        </mat-select>
      </mat-form-field>
      <div class="query-grid__footer">
        <mat-checkbox class="query-grid__checkbox" formControlName="includeArchived">Include archived</mat-checkbox>
      </div>
    </form>
  `,
  styles: [`
    .query-grid{
      display:grid;
      grid-template-columns:repeat(3, minmax(0, 1fr));
      gap:1.25rem 1rem;
      align-items:start;
    }
    .query-grid mat-form-field{width:100%;}
    .query-grid .mat-mdc-input-element::placeholder{color:var(--app-on-muted);opacity:1;}
    .query-grid .mat-mdc-form-field-infix{padding-top:1rem;}
    .query-grid__footer{
      grid-column:1 / -1;
      display:flex;
      align-items:center;
      min-height:44px;
      padding-top:0.5rem;
      border-top:1px solid rgba(226, 232, 240, 0.8);
      margin-top:0.25rem;
    }
    .query-grid__checkbox{display:flex;align-items:center;}
    @media (max-width: 900px){
      .query-grid{grid-template-columns:repeat(2, minmax(0, 1fr));}
    }
    @media (max-width: 640px){
      .query-grid{grid-template-columns:1fr;}
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskQueryBarComponent {
  readonly form = input.required<any>();
  @Output() readonly apply = new EventEmitter<void>();
}
