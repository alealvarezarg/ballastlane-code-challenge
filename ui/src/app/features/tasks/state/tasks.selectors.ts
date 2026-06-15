import { createSelector } from '@ngrx/store';

import { tasksFeature } from './tasks.reducer';

export const selectTasks = tasksFeature.selectTasks;
export const selectTaskSummary = tasksFeature.selectSummary;
export const selectTaskQuery = tasksFeature.selectQuery;
export const selectTaskTotalCount = tasksFeature.selectTotalCount;
export const selectSelectedTask = tasksFeature.selectSelectedTask;
export const selectTaskLoadStatus = tasksFeature.selectLoadStatus;
export const selectTaskMutationStatus = tasksFeature.selectMutationStatus;
export const selectTaskCountLabel = createSelector(selectTasks, (tasks) => `${tasks.length} tasks loaded`);
