import { ChangeDetectionStrategy, Component, DestroyRef, TemplateRef, computed, effect, inject, viewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';

import { PageStateComponent } from '@app/shared/components/page-state/page-state.component';
import { ManagementTask } from '@app/shared/models/task.models';
import { toDateInputValue } from '@app/shared/utils/date-format.util';

import { createTaskForm } from '../../validators/task-form.validators';
import { createTaskQueryForm } from '../../validators/task-query.validators';
import { TaskFormComponent } from '../../components/task-form/task-form.component';
import { TaskListComponent } from '../../components/task-list/task-list.component';
import { TaskQueryBarComponent } from '../../components/task-query-bar/task-query-bar.component';
import { TaskSummaryComponent } from '../../components/task-summary/task-summary.component';
import { tasksActions } from '../../state/tasks.actions';
import { selectSelectedTask, selectTaskLoadStatus, selectTaskMutationStatus, selectTaskQuery, selectTaskSummary, selectTaskTotalCount, selectTasks } from '../../state/tasks.selectors';

@Component({
  selector: 'app-tasks-page',
  standalone: true,
  imports: [ReactiveFormsModule, MatButtonModule, MatCardModule, MatDialogModule, MatDividerModule, PageStateComponent, TaskFormComponent, TaskListComponent, TaskQueryBarComponent, TaskSummaryComponent],
  templateUrl: './tasks-page.component.html',
  styles: [`
    .tasks-layout{display:grid;gap:1.5rem;}
    .tasks-hero,.tasks-panel{
      background:var(--app-surface);
      border:1px solid var(--app-border);
      border-radius:var(--app-radius);
      box-shadow:var(--app-shadow);
      padding:1.25rem;
    }
    .tasks-hero{
      background:
        linear-gradient(135deg, rgba(37, 99, 235, 0.08), rgba(20, 184, 166, 0.08)),
        var(--app-surface);
    }
    .tasks-hero h1{
      margin:0 0 0.5rem;
      font-size:clamp(1.9rem, 4vw, 2.6rem);
      letter-spacing:-0.03em;
    }
    .tasks-hero p{
      margin:0;
      color:var(--app-on-muted);
      line-height:1.55;
      max-width:52rem;
    }
    .tasks-hero__header{
      display:flex;
      align-items:flex-start;
      justify-content:space-between;
      gap:1rem;
    }
    .tasks-hero__actions{
      display:flex;
      align-items:center;
      gap:0.75rem;
      flex-wrap:wrap;
      justify-content:flex-end;
    }
    .tasks-hero__action{
      min-height:44px;
      padding-inline:1rem;
      border-radius:999px;
      font-weight:700;
      white-space:nowrap;
    }
    .tasks-dialog{
      display:grid;
      gap:1.25rem;
      padding:0.5rem;
    }
    .tasks-dialog__header{
      display:flex;
      align-items:flex-start;
      justify-content:space-between;
      gap:1rem;
      padding-bottom:0.25rem;
    }
    .tasks-dialog__title{
      margin:0;
      font-size:1.35rem;
      line-height:1.2;
    }
    .tasks-dialog__copy{
      margin:0.35rem 0 0;
      color:var(--app-on-muted);
      line-height:1.5;
    }
    .tasks-dialog__body{
      display:grid;
      gap:1.25rem;
      padding-block:0.25rem;
    }
    .tasks-dialog__footer{
      display:flex;
      justify-content:flex-end;
      align-items:center;
      gap:0.75rem;
      padding-top:0.5rem;
    }
    .tasks-dialog__close{
      flex:0 0 auto;
      display:inline-flex;
      align-items:center;
      justify-content:center;
      width:42px;
      min-width:42px;
      height:42px;
      padding:0;
      border-radius:999px;
      border:1px solid #fca5a5 !important;
      background:#fef2f2 !important;
      color:#b91c1c !important;
      font-size:1.1rem;
      font-weight:800;
      line-height:1;
    }
    .tasks-dialog__action{
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
    .tasks-dialog__action--secondary{
      background:#ffffff !important;
      border-color:var(--app-border) !important;
      color:var(--app-on-surface) !important;
      box-shadow:0 10px 22px rgba(15, 23, 42, 0.08) !important;
    }
    .tasks-dialog__action--primary{
      border-color:var(--app-primary) !important;
      background:var(--app-primary) !important;
      color:#ffffff !important;
      box-shadow:0 14px 28px rgba(37, 99, 235, 0.18) !important;
    }
    .tasks-dialog__action--secondary:hover{
      background:var(--app-surface-alt) !important;
      box-shadow:0 14px 26px rgba(15, 23, 42, 0.1) !important;
    }
    .tasks-dialog__action--primary:hover{
      background:var(--app-primary-hover) !important;
      box-shadow:0 18px 32px rgba(37, 99, 235, 0.22) !important;
    }
    .tasks-split{display:grid;grid-template-columns:minmax(0,2fr) minmax(320px,1fr);gap:1.5rem;}
    @media (max-width: 900px){
      .tasks-hero__header{
        flex-direction:column;
      }
      .tasks-hero__actions{
        width:100%;
        justify-content:flex-start;
      }
      .tasks-dialog__header,
      .tasks-dialog__footer{
        flex-direction:column;
        align-items:stretch;
      }
      .tasks-split{grid-template-columns:1fr;}
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TasksPageComponent {
  private readonly store = inject(Store);
  private readonly destroyRef = inject(DestroyRef);
  private readonly dialog = inject(MatDialog);
  private readonly filtersDialogTemplate = viewChild.required<TemplateRef<unknown>>('filtersDialog');
  private readonly taskEditorDialogTemplate = viewChild.required<TemplateRef<unknown>>('taskEditorDialog');
  private filtersDialogRef: MatDialogRef<unknown> | null = null;
  private taskEditorDialogRef: MatDialogRef<unknown> | null = null;
  private closeTaskEditorOnSuccess = false;

  protected readonly taskForm = createTaskForm();
  protected readonly queryForm = createTaskQueryForm();
  protected readonly tasks = this.store.selectSignal(selectTasks);
  protected readonly summary = this.store.selectSignal(selectTaskSummary);
  protected readonly query = this.store.selectSignal(selectTaskQuery);
  protected readonly totalCount = this.store.selectSignal(selectTaskTotalCount);
  protected readonly loadStatus = this.store.selectSignal(selectTaskLoadStatus);
  protected readonly mutationStatus = this.store.selectSignal(selectTaskMutationStatus);
  protected readonly selectedTask = this.store.selectSignal(selectSelectedTask);
  protected readonly isEditing = computed(() => !!this.selectedTask());

  constructor() {
    this.store.dispatch(tasksActions.loadRequested({}));
    this.store.dispatch(tasksActions.summaryRequested());

    this.store.select(selectTaskQuery)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((query) => {
        this.queryForm.patchValue({
          search: query.search ?? '',
          status: (query.status ?? '') as '' | 'Pending' | 'InProgress' | 'Completed',
          userId: query.userId ?? '',
          sortBy: (query.sortBy ?? '') as '' | 'title' | 'status' | 'dueDate',
          sortDirection: (query.sortDirection ?? '') as '' | 'asc' | 'desc',
          pageSize: query.pageSize ?? 10,
          includeArchived: query.includeArchived
        }, { emitEvent: false });
      });

    effect(() => {
      const mutationStatus = this.mutationStatus();

      if (!this.closeTaskEditorOnSuccess || mutationStatus.loading) {
        return;
      }

      this.closeTaskEditorOnSuccess = false;

      if (!mutationStatus.error) {
        this.taskEditorDialogRef?.close();
        this.resetEditor();
      }
    });
  }

  openFiltersDialog(): void {
    this.filtersDialogRef = this.dialog.open(this.filtersDialogTemplate(), {
      autoFocus: false,
      width: 'min(760px, calc(100vw - 2rem))',
      maxWidth: '760px'
    });
    this.filtersDialogRef.afterClosed().subscribe(() => {
      this.filtersDialogRef = null;
    });
  }

  closeFiltersDialog(): void {
    this.filtersDialogRef?.close();
    this.filtersDialogRef = null;
  }

  applyFilters(): void {
    const value = this.queryForm.getRawValue();
    this.store.dispatch(tasksActions.loadRequested({
      query: {
        search: value.search || undefined,
        status: value.status || undefined,
        userId: value.userId || undefined,
        sortBy: value.sortBy || undefined,
        sortDirection: undefined,
        pageSize: value.pageSize ?? 10,
        page: 1,
        includeArchived: value.includeArchived
      }
    }));
    this.store.dispatch(tasksActions.summaryRequested());
    this.closeFiltersDialog();
  }

  resetFilters(): void {
    this.queryForm.reset({
      search: '',
      status: '',
      userId: '',
      sortBy: '',
      sortDirection: '',
      pageSize: 10,
      includeArchived: true
    });
  }

  changePage(page: number): void {
    this.store.dispatch(tasksActions.loadRequested({
      query: {
        page,
        pageSize: this.query().pageSize ?? 10
      }
    }));
  }

  changePageSize(pageSize: number): void {
    this.queryForm.patchValue({ pageSize }, { emitEvent: false });
    this.store.dispatch(tasksActions.loadRequested({
      query: {
        page: 1,
        pageSize
      }
    }));
  }

  openCreateTaskDialog(): void {
    this.resetEditor();
    this.openTaskEditorDialog();
  }

  editTask(task: ManagementTask): void {
    this.store.dispatch(tasksActions.selectTask({ task }));
    this.taskForm.patchValue({
      id: task.id,
      title: task.title,
      description: task.description,
      status: task.status,
      dueDate: toDateInputValue(task.dueDate),
      userId: task.userId
    });
    this.openTaskEditorDialog();
  }

  openTaskEditorDialog(): void {
    this.taskEditorDialogRef = this.dialog.open(this.taskEditorDialogTemplate(), {
      autoFocus: false,
      width: 'min(720px, calc(100vw - 2rem))',
      maxWidth: '720px'
    });
    this.taskEditorDialogRef.afterClosed().subscribe(() => {
      this.taskEditorDialogRef = null;
      this.closeTaskEditorOnSuccess = false;
      this.resetEditor();
    });
  }

  saveTask(): void {
    if (this.taskForm.invalid) {
      this.taskForm.markAllAsTouched();
      return;
    }

    const value = this.taskForm.getRawValue();
    const task: ManagementTask = {
      id: value.id,
      title: value.title,
      description: value.description,
      status: value.status,
      dueDate: new Date(value.dueDate).toISOString(),
      userId: value.userId,
      isArchived: false,
      archivedAt: null
    };

    if (this.selectedTask()) {
      this.store.dispatch(tasksActions.updateRequested({ task }));
    } else {
      this.store.dispatch(tasksActions.createRequested({ task }));
    }
    this.closeTaskEditorOnSuccess = true;
  }

  archiveTask(id: string): void {
    this.store.dispatch(tasksActions.archiveRequested({ id }));
  }

  changeStatus(event: { id: string; status: ManagementTask['status'] }): void {
    this.store.dispatch(tasksActions.statusRequested(event));
  }

  cancelTaskEditor(): void {
    this.taskEditorDialogRef?.close();
    this.resetEditor();
    this.closeTaskEditorOnSuccess = false;
  }

  resetEditor(): void {
    this.store.dispatch(tasksActions.selectTask({ task: null }));
    this.taskForm.reset({
      id: '',
      title: '',
      description: '',
      status: 'Pending',
      dueDate: '',
      userId: ''
    });
  }
}
