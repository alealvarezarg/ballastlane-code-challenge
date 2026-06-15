import { createFeature, createReducer, on } from '@ngrx/store';

import { initialRequestStatus } from '@app/shared/models/request-status.model';

import { initialTasksState } from '../models/task-view.models';
import { tasksActions } from './tasks.actions';

export const tasksFeature = createFeature({
  name: 'tasks',
  reducer: createReducer(
    initialTasksState(),
    on(tasksActions.loadRequested, (state, { query }) => ({
      ...state,
      query: { ...state.query, ...query },
      loadStatus: { loading: true, successMessage: null, error: null }
    })),
    on(tasksActions.loadSucceeded, (state, { items, totalCount }) => ({
      ...state,
      tasks: items,
      totalCount,
      loadStatus: { loading: false, successMessage: null, error: null }
    })),
    on(tasksActions.loadFailed, (state, { error }) => ({
      ...state,
      loadStatus: { loading: false, successMessage: null, error }
    })),
    on(tasksActions.summarySucceeded, (state, { summary }) => ({
      ...state,
      summary
    })),
    on(tasksActions.selectTask, (state, { task }) => ({
      ...state,
      selectedTask: task
    })),
    on(tasksActions.createRequested, tasksActions.updateRequested, tasksActions.statusRequested, tasksActions.archiveRequested, (state) => ({
      ...state,
      mutationStatus: { loading: true, successMessage: null, error: null }
    })),
    on(tasksActions.createSucceeded, (state, { task }) => ({
      ...state,
      tasks: [task, ...state.tasks],
      mutationStatus: { loading: false, successMessage: 'Task created successfully.', error: null }
    })),
    on(tasksActions.updateSucceeded, tasksActions.statusSucceeded, (state, { task }) => ({
      ...state,
      tasks: state.tasks.map((item) => item.id === task.id ? task : item),
      selectedTask: null,
      mutationStatus: { loading: false, successMessage: 'Task updated successfully.', error: null }
    })),
    on(tasksActions.archiveSucceeded, (state, { id }) => ({
      ...state,
      tasks: state.tasks.map((item) =>
        item.id === id
          ? {
              ...item,
              isArchived: true,
              archivedAt: new Date().toISOString()
            }
          : item
      ),
      selectedTask: null,
      mutationStatus: { loading: false, successMessage: 'Task archived successfully.', error: null }
    })),
    on(tasksActions.createFailed, tasksActions.updateFailed, tasksActions.statusFailed, tasksActions.archiveFailed, tasksActions.summaryFailed, (state, { error }) => ({
      ...state,
      mutationStatus: { loading: false, successMessage: null, error }
    })),
    on(tasksActions.summaryRequested, (state) => ({
      ...state,
      mutationStatus: initialRequestStatus()
    }))
  )
});
