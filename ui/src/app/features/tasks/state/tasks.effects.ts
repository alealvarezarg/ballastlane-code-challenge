import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { catchError, exhaustMap, map, of, tap, withLatestFrom } from 'rxjs';

import { NotificationService } from '@app/core/services/notification.service';
import { mapHttpError } from '@app/shared/utils/api-error.mapper';

import { TaskRequestMapperService } from '../services/task-request-mapper.service';
import { TasksApiService } from '../services/tasks-api.service';
import { tasksActions } from './tasks.actions';
import { selectTaskQuery } from './tasks.selectors';

@Injectable()
export class TasksEffects {
  private readonly actions$ = inject(Actions);
  private readonly store = inject(Store);
  private readonly tasksApiService = inject(TasksApiService);
  private readonly requestMapper = inject(TaskRequestMapperService);
  private readonly notificationService = inject(NotificationService);

  readonly loadTasks$ = createEffect(() =>
    this.actions$.pipe(
      ofType(tasksActions.loadRequested),
      withLatestFrom(this.store.select(selectTaskQuery)),
      exhaustMap(([{ query }, currentQuery]) =>
        this.tasksApiService.queryTasks({ ...currentQuery, ...query }).pipe(
          map((response) => tasksActions.loadSucceeded({ items: response.items, totalCount: response.totalCount })),
          catchError((error) => of(tasksActions.loadFailed({ error: mapHttpError(error) })))
        )
      )
    )
  );

  readonly createTask$ = createEffect(() =>
    this.actions$.pipe(
      ofType(tasksActions.createRequested),
      exhaustMap(({ task }) =>
        this.tasksApiService.createTask(this.requestMapper.toCreateRequest(task)).pipe(
          map((createdTask) => tasksActions.createSucceeded({ task: createdTask })),
          catchError((error) => of(tasksActions.createFailed({ error: mapHttpError(error) })))
        )
      )
    )
  );

  readonly updateTask$ = createEffect(() =>
    this.actions$.pipe(
      ofType(tasksActions.updateRequested),
      exhaustMap(({ task }) =>
        this.tasksApiService.updateTask(task.id, this.requestMapper.toUpdateRequest(task)).pipe(
          map((updatedTask) => tasksActions.updateSucceeded({ task: updatedTask })),
          catchError((error) => of(tasksActions.updateFailed({ error: mapHttpError(error) })))
        )
      )
    )
  );

  readonly updateTaskStatus$ = createEffect(() =>
    this.actions$.pipe(
      ofType(tasksActions.statusRequested),
      exhaustMap(({ id, status }) =>
        this.tasksApiService.updateTaskStatus(id, { status }).pipe(
          map((updatedTask) => tasksActions.statusSucceeded({ task: updatedTask })),
          catchError((error) => of(tasksActions.statusFailed({ error: mapHttpError(error) })))
        )
      )
    )
  );

  readonly archiveTask$ = createEffect(() =>
    this.actions$.pipe(
      ofType(tasksActions.archiveRequested),
      exhaustMap(({ id }) =>
        this.tasksApiService.archiveTask(id).pipe(
          map(() => tasksActions.archiveSucceeded({ id })),
          catchError((error) => of(tasksActions.archiveFailed({ error: mapHttpError(error) })))
        )
      )
    )
  );

  readonly refreshTasksView$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        tasksActions.createSucceeded,
        tasksActions.updateSucceeded,
        tasksActions.statusSucceeded,
        tasksActions.archiveSucceeded
      ),
      exhaustMap(() => of(tasksActions.loadRequested({})))
    )
  );

  readonly mutationFeedback$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(tasksActions.createSucceeded, tasksActions.updateSucceeded, tasksActions.statusSucceeded, tasksActions.archiveSucceeded),
        tap(() => this.notificationService.success('Task change saved.'))
      ),
    { dispatch: false }
  );

  readonly mutationFailure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(tasksActions.createFailed, tasksActions.updateFailed, tasksActions.statusFailed, tasksActions.archiveFailed, tasksActions.loadFailed),
        tap(({ error }) => this.notificationService.error(error.message))
      ),
    { dispatch: false }
  );
}
