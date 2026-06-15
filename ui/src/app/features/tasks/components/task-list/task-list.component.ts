import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, EventEmitter, Output, computed, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';

import { ManagementTask } from '@app/shared/models/task.models';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [DatePipe, MatButtonModule, MatTooltipModule],
  template: `
    <div class="task-list">
      <div class="task-list__toolbar">
        <div class="task-list__page-size">
          <label class="task-list__label" for="pageSizeTop">Page size</label>
          <select id="pageSizeTop" class="task-list__select" (change)="onPageSizeChange($any($event.target).value)">
            @for (size of pageSizeOptions; track size) {
              <option [value]="size.toString()" [selected]="size === pageSize()">{{ size }}</option>
            }
          </select>
        </div>
        <div class="task-list__pager">
          <span class="task-list__page-info">Page {{ currentPage() }} of {{ totalPages() }}</span>
          <button class="task-list__nav" mat-icon-button type="button" [disabled]="!canGoPrevious()" (click)="goToFirst()" matTooltip="First page" aria-label="First page">
            <span class="task-list__nav-icon" aria-hidden="true">|&lt;</span>
          </button>
          <button class="task-list__nav" mat-icon-button type="button" [disabled]="!canGoPrevious()" (click)="goToPrevious()" matTooltip="Previous page" aria-label="Previous page">
            <span class="task-list__nav-icon" aria-hidden="true">&lt;</span>
          </button>
          <button class="task-list__nav" mat-icon-button type="button" [disabled]="!canGoNext()" (click)="goToNext()" matTooltip="Next page" aria-label="Next page">
            <span class="task-list__nav-icon" aria-hidden="true">&gt;</span>
          </button>
          <button class="task-list__nav" mat-icon-button type="button" [disabled]="!canGoNext()" (click)="goToLast()" matTooltip="Last page" aria-label="Last page">
            <span class="task-list__nav-icon" aria-hidden="true">&gt;|</span>
          </button>
        </div>
      </div>

      @if (!tasks().length) {
        <div class="task-list__empty">No records found.</div>
      } @else {
        <div class="task-table-wrap">
          <table class="task-table">
            <thead>
              <tr>
                <th>Title</th>
                <th>Description</th>
                <th>Due date</th>
                <th>User Id</th>
                <th>Status</th>
                <th>Archived</th>
                <th class="task-table__actions-col">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (task of tasks(); track task.id) {
                <tr>
                  <td class="task-table__title">{{ task.title }}</td>
                  <td class="task-table__description">{{ task.description }}</td>
                  <td>{{ task.dueDate | date:'medium' }}</td>
                  <td>{{ task.userId }}</td>
                  <td>
                    <span class="status-badge" [class]="statusClass(task.status)">{{ task.status }}</span>
                  </td>
                  <td>
                    @if (task.isArchived) {
                      <span class="status-badge status-badge--archived">Yes</span>
                    } @else {
                      <span class="task-table__archived-no">No</span>
                    }
                  </td>
                  <td>
                    <div class="task-table__actions">
                      <button
                        class="task-table__icon-action task-table__icon-action--primary"
                        mat-icon-button
                        type="button"
                        [disabled]="!nextStatus(task)"
                        (click)="advanceToNextStage(task)"
                        matTooltip="Next Stage"
                        aria-label="Next Stage">
                        <svg class="task-table__icon-svg" viewBox="0 0 24 24" aria-hidden="true">
                          <path d="M5 12h11"></path>
                          <path d="M12 6l6 6-6 6"></path>
                        </svg>
                      </button>
                      <button
                        class="task-table__icon-action"
                        mat-icon-button
                        type="button"
                        [disabled]="task.isArchived"
                        (click)="archive.emit(task.id)"
                        matTooltip="Archive"
                        aria-label="Archive">
                        <svg class="task-table__icon-svg" viewBox="0 0 24 24" aria-hidden="true">
                          <path d="M4 7h16"></path>
                          <path d="M6 7l1 11h10l1-11"></path>
                          <path d="M9 7V5h6v2"></path>
                        </svg>
                      </button>
                      <button
                        class="task-table__icon-action task-table__icon-action--danger"
                        mat-icon-button
                        type="button"
                        [disabled]="task.isArchived"
                        (click)="delete.emit(task.id)"
                        matTooltip="Delete"
                        aria-label="Delete">
                        <svg class="task-table__icon-svg" viewBox="0 0 24 24" aria-hidden="true">
                          <path d="M7 7l10 10"></path>
                          <path d="M17 7L7 17"></path>
                        </svg>
                      </button>
                    </div>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }

      <div class="task-list__toolbar task-list__toolbar--bottom">
        <div class="task-list__page-size">
          <label class="task-list__label" for="pageSizeBottom">Page size</label>
          <select id="pageSizeBottom" class="task-list__select" (change)="onPageSizeChange($any($event.target).value)">
            @for (size of pageSizeOptions; track size) {
              <option [value]="size.toString()" [selected]="size === pageSize()">{{ size }}</option>
            }
          </select>
        </div>
        <div class="task-list__pager">
          <span class="task-list__page-info">Page {{ currentPage() }} of {{ totalPages() }}</span>
          <button class="task-list__nav" mat-icon-button type="button" [disabled]="!canGoPrevious()" (click)="goToFirst()" matTooltip="First page" aria-label="First page">
            <span class="task-list__nav-icon" aria-hidden="true">|&lt;</span>
          </button>
          <button class="task-list__nav" mat-icon-button type="button" [disabled]="!canGoPrevious()" (click)="goToPrevious()" matTooltip="Previous page" aria-label="Previous page">
            <span class="task-list__nav-icon" aria-hidden="true">&lt;</span>
          </button>
          <button class="task-list__nav" mat-icon-button type="button" [disabled]="!canGoNext()" (click)="goToNext()" matTooltip="Next page" aria-label="Next page">
            <span class="task-list__nav-icon" aria-hidden="true">&gt;</span>
          </button>
          <button class="task-list__nav" mat-icon-button type="button" [disabled]="!canGoNext()" (click)="goToLast()" matTooltip="Last page" aria-label="Last page">
            <span class="task-list__nav-icon" aria-hidden="true">&gt;|</span>
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .task-list{
      display:grid;
      gap:1rem;
    }
    .task-list__toolbar{
      display:flex;
      align-items:center;
      justify-content:space-between;
      gap:1rem;
      flex-wrap:wrap;
    }
    .task-list__toolbar--bottom{
      padding-top:0.25rem;
    }
    .task-list__page-size,
    .task-list__pager{
      display:flex;
      align-items:center;
      gap:0.75rem;
      flex-wrap:wrap;
    }
    .task-list__label,
    .task-list__page-info{
      color:var(--app-on-muted);
      font-weight:700;
    }
    .task-list__select{
      min-width:96px;
      min-height:44px;
      padding:0 1rem;
      border:1px solid var(--app-border);
      border-radius:999px;
      background:var(--app-surface);
      color:var(--app-on-surface);
      font:inherit;
      font-weight:700;
    }
    .task-list__nav{
      width:42px;
      height:42px;
      border:1px solid var(--app-border) !important;
      background:var(--app-surface) !important;
      color:var(--app-on-surface) !important;
      box-shadow:var(--app-shadow-soft) !important;
      cursor:pointer;
    }
    .task-list__nav:not(:disabled):hover{
      background:var(--app-surface-alt) !important;
      box-shadow:0 14px 24px rgba(15, 23, 42, 0.1) !important;
    }
    .task-list__nav:disabled{
      opacity:0.45;
      box-shadow:none !important;
      cursor:not-allowed;
    }
    .task-list__nav-icon{
      font-size:0.95rem;
      font-weight:800;
      line-height:1;
      letter-spacing:-0.04em;
    }
    .task-list__empty{
      padding:2.5rem 1.5rem;
      text-align:center;
      border:1px dashed var(--app-border);
      border-radius:var(--app-radius);
      background:var(--app-surface-alt);
      color:var(--app-on-muted);
      font-weight:700;
    }
    .task-table-wrap{
      overflow:auto;
      border:1px solid var(--app-border);
      border-radius:var(--app-radius);
      background:var(--app-surface);
    }
    .task-table{
      width:100%;
      border-collapse:separate;
      border-spacing:0;
      min-width:980px;
    }
    th, td{
      padding:1rem;
      text-align:left;
      vertical-align:top;
      border-bottom:1px solid var(--app-border);
    }
    thead th{
      position:sticky;
      top:0;
      z-index:1;
      background:#f8fafc;
      color:var(--app-on-muted);
      font-size:0.84rem;
      font-weight:800;
      letter-spacing:0.02em;
      text-transform:uppercase;
    }
    tbody tr:hover{
      background:rgba(37, 99, 235, 0.03);
    }
    tbody tr:last-child td{
      border-bottom:none;
    }
    .task-table__title{
      min-width:180px;
      font-weight:700;
      color:var(--app-on-surface);
    }
    .task-table__description{
      min-width:220px;
      color:var(--app-on-muted);
      line-height:1.55;
    }
    .task-table__archived-no{
      color:var(--app-on-muted);
      font-weight:600;
    }
    .task-table__actions-col{
      min-width:170px;
    }
    .task-table__actions{
      display:flex;
      gap:0.5rem;
      flex-wrap:wrap;
      justify-content:flex-end;
    }
    .task-table__icon-action{
      width:40px;
      height:40px;
      border:1px solid var(--app-border) !important;
      border-radius:999px;
      background:var(--app-surface) !important;
      color:var(--app-on-surface) !important;
      box-shadow:var(--app-shadow-soft) !important;
      cursor:pointer;
    }
    .task-table__icon-action:not(:disabled):hover{
      background:var(--app-surface-alt) !important;
      box-shadow:0 12px 24px rgba(15, 23, 42, 0.1) !important;
      transform:translateY(-1px);
    }
    .task-table__icon-action:disabled{
      opacity:0.38;
      box-shadow:none !important;
      cursor:not-allowed;
    }
    .task-table__icon-action--primary{
      border-color:rgba(37, 99, 235, 0.2) !important;
      color:var(--app-primary) !important;
      background:rgba(37, 99, 235, 0.08) !important;
    }
    .task-table__icon-action--danger{
      border-color:#fecaca !important;
      color:#b91c1c !important;
      background:#fef2f2 !important;
    }
    .task-table__icon-svg{
      width:18px;
      height:18px;
      stroke:currentColor;
      fill:none;
      stroke-width:2;
      stroke-linecap:round;
      stroke-linejoin:round;
    }
    @media (max-width: 720px){
      .task-list__toolbar{
        flex-direction:column;
        align-items:stretch;
      }
      .task-list__page-size,
      .task-list__pager{
        justify-content:space-between;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskListComponent {
  readonly tasks = input<ManagementTask[]>([]);
  readonly currentPage = input(1);
  readonly pageSize = input(10);
  readonly totalCount = input(0);
  readonly pageSizeOptions = [10, 25, 50, 100];
  @Output() readonly archive = new EventEmitter<string>();
  @Output() readonly delete = new EventEmitter<string>();
  @Output() readonly statusChange = new EventEmitter<{ id: string; status: ManagementTask['status'] }>();
  @Output() readonly pageChange = new EventEmitter<number>();
  @Output() readonly pageSizeChange = new EventEmitter<number>();

  protected readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));
  protected readonly canGoPrevious = computed(() => this.currentPage() > 1);
  protected readonly canGoNext = computed(() => this.currentPage() < this.totalPages());

  statusClass(status: ManagementTask['status']): string {
    return `status-badge--${status.toLowerCase()}`;
  }

  onPageSizeChange(value: string): void {
    const pageSize = Number(value);
    if (!Number.isNaN(pageSize)) {
      this.pageSizeChange.emit(pageSize);
    }
  }

  goToFirst(): void {
    if (this.canGoPrevious()) {
      this.pageChange.emit(1);
    }
  }

  goToPrevious(): void {
    if (this.canGoPrevious()) {
      this.pageChange.emit(this.currentPage() - 1);
    }
  }

  goToNext(): void {
    if (this.canGoNext()) {
      this.pageChange.emit(this.currentPage() + 1);
    }
  }

  goToLast(): void {
    if (this.canGoNext()) {
      this.pageChange.emit(this.totalPages());
    }
  }

  nextStatus(task: ManagementTask): ManagementTask['status'] | null {
    if (task.isArchived) {
      return null;
    }

    switch (task.status) {
      case 'Pending':
        return 'InProgress';
      case 'InProgress':
        return 'Completed';
      default:
        return null;
    }
  }

  advanceToNextStage(task: ManagementTask): void {
    const nextStatus = this.nextStatus(task);

    if (!nextStatus) {
      return;
    }

    this.statusChange.emit({ id: task.id, status: nextStatus });
  }
}
