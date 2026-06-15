import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

import { ManagementTask, ManagementTaskSummary } from '@app/shared/models/task.models';

@Component({
  selector: 'app-task-summary',
  standalone: true,
  imports: [MatCardModule],
  template: `
    <div class="summary-grid">
      @for (item of summaryItems(); track item.label) {
        <mat-card class="summary-card" [class]="item.className">
          <mat-card-title>{{ item.label }}</mat-card-title>
          <mat-card-content>{{ item.count }}</mat-card-content>
        </mat-card>
      }
    </div>
  `,
  styles: [`
    .summary-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(160px,1fr));gap:1rem;}
    .summary-card{
      text-align:center;
      padding:0.25rem;
      border-width:1px;
    }
    .summary-card mat-card-title{
      font-size:0.95rem;
      font-weight:700;
    }
    .summary-card mat-card-content{
      font-size:2rem;
      font-weight:800;
      letter-spacing:-0.04em;
      padding-bottom:0.25rem;
    }
    .summary-card--pending{
      background:#fff7ed !important;
      border-color:#fdba74 !important;
    }
    .summary-card--pending mat-card-title,
    .summary-card--pending mat-card-content{
      color:#c2410c;
    }
    .summary-card--inprogress{
      background:#eff6ff !important;
      border-color:#93c5fd !important;
    }
    .summary-card--inprogress mat-card-title,
    .summary-card--inprogress mat-card-content{
      color:#1d4ed8;
    }
    .summary-card--completed{
      background:#ecfdf5 !important;
      border-color:#86efac !important;
    }
    .summary-card--completed mat-card-title,
    .summary-card--completed mat-card-content{
      color:#15803d;
    }
    .summary-card--archived{
      background:#f8fafc !important;
      border-color:#cbd5e1 !important;
    }
    .summary-card--archived mat-card-title,
    .summary-card--archived mat-card-content{
      color:#475569;
    }
    .summary-card--total{
      background:#f5f3ff !important;
      border-color:#c4b5fd !important;
    }
    .summary-card--total mat-card-title,
    .summary-card--total mat-card-content{
      color:#6d28d9;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskSummaryComponent {
  readonly summary = input<ManagementTaskSummary | null>(null);
  readonly tasks = input<ManagementTask[]>([]);

  protected readonly summaryItems = computed(() => {
    const summary = this.summary();
    const tasks = this.tasks();
    const findCount = (status: 'Pending' | 'InProgress' | 'Completed') =>
      summary?.statuses.find((item) => item.status === status)?.count ?? 0;
    const archivedCount = tasks.filter((task) => task.isArchived).length;

    return [
      { label: 'InProgress', count: findCount('InProgress'), className: 'summary-card--inprogress' },
      { label: 'Pending', count: findCount('Pending'), className: 'summary-card--pending' },
      { label: 'Completed', count: findCount('Completed'), className: 'summary-card--completed' },
      { label: 'Archived', count: archivedCount, className: 'summary-card--archived' },
      { label: 'Total', count: findCount('Pending') + findCount('InProgress') + findCount('Completed') + archivedCount, className: 'summary-card--total' }
    ];
  });
}
