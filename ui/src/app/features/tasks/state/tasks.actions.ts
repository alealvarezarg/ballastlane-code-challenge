import { createActionGroup, emptyProps, props } from '@ngrx/store';

import { ApiErrorState } from '@app/shared/models/api-error.models';
import { ManagementTask, ManagementTaskSummary, ManagementTaskStatus, TaskQueryState } from '@app/shared/models/task.models';

export const tasksActions = createActionGroup({
  source: 'Tasks',
  events: {
    'Load Requested': props<{ query?: Partial<TaskQueryState> }>(),
    'Load Succeeded': props<{ items: ManagementTask[]; totalCount: number }>(),
    'Load Failed': props<{ error: ApiErrorState }>(),
    'Summary Requested': emptyProps(),
    'Summary Succeeded': props<{ summary: ManagementTaskSummary }>(),
    'Summary Failed': props<{ error: ApiErrorState }>(),
    'Select Task': props<{ task: ManagementTask | null }>(),
    'Create Requested': props<{ task: Partial<ManagementTask> }>(),
    'Create Succeeded': props<{ task: ManagementTask }>(),
    'Create Failed': props<{ error: ApiErrorState }>(),
    'Update Requested': props<{ task: ManagementTask }>(),
    'Update Succeeded': props<{ task: ManagementTask }>(),
    'Update Failed': props<{ error: ApiErrorState }>(),
    'Status Requested': props<{ id: string; status: ManagementTaskStatus }>(),
    'Status Succeeded': props<{ task: ManagementTask }>(),
    'Status Failed': props<{ error: ApiErrorState }>(),
    'Archive Requested': props<{ id: string }>(),
    'Archive Succeeded': props<{ id: string }>(),
    'Archive Failed': props<{ error: ApiErrorState }>()
  }
});
