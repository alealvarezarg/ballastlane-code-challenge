import { ChangeDetectionStrategy, Component, EventEmitter, Output, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';

import { ManagementTaskStatus } from '@app/shared/models/task.models';

@Component({
  selector: 'app-task-status-menu',
  standalone: true,
  imports: [MatButtonModule, MatMenuModule],
  template: `
    <button mat-button [matMenuTriggerFor]="menu">{{ status() }}</button>
    <mat-menu #menu="matMenu">
      <button mat-menu-item (click)="change.emit('Pending')">Pending</button>
      <button mat-menu-item (click)="change.emit('InProgress')">In progress</button>
      <button mat-menu-item (click)="change.emit('Completed')">Completed</button>
    </mat-menu>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskStatusMenuComponent {
  readonly status = input.required<ManagementTaskStatus>();
  @Output() readonly change = new EventEmitter<ManagementTaskStatus>();
}
