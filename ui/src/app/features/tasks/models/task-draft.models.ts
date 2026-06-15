import { ManagementTaskStatus, TaskQueryState } from '@app/shared/models/task.models';

export interface TaskDraftFormValue {
  id: string;
  title: string;
  description: string;
  status: ManagementTaskStatus;
  dueDate: string;
  userId: string;
}

export interface TaskQueryFormValue extends TaskQueryState {}
